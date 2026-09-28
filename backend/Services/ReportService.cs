using ScholarshipCMGroups.Models;
using ScholarshipCMGroups.Repositories;

namespace ScholarshipCMGroups.Services;

public class ReportService
{
    private readonly StudentRepository _students;
    private readonly ScholarshipRepository _scholarships;
    private readonly ApplicationRepository _applications;
    private readonly DocumentRepository _documents;
    private readonly ILogger<ReportService> _logger;

    public ReportService(
        StudentRepository students,
        ScholarshipRepository scholarships,
        ApplicationRepository applications,
        DocumentRepository documents,
        ILogger<ReportService> logger)
    {
        _students = students;
        _scholarships = scholarships;
        _applications = applications;
        _documents = documents;
        _logger = logger;
    }

    public List<ReportRow> StudentReport()
    {
        var rows = _students.GetAll();
        var report = new List<ReportRow>();
        var groups = new Dictionary<string, ReportRow>();
        foreach (var student in rows)
        {
            var key = string.IsNullOrWhiteSpace(student.Major) ? "Undeclared" : student.Major;
            if (!groups.ContainsKey(key))
            {
                groups[key] = new ReportRow { Label = key, Status = "Student" };
            }

            groups[key].Count = groups[key].Count + 1;
            groups[key].Amount = groups[key].Amount + student.AnnualIncome;
        }

        foreach (var pair in groups)
        {
            report.Add(pair.Value);
        }

        return report;
    }

    public List<ReportRow> GenerateScholarshipReport()
    {
        var scholarships = _scholarships.GetAll();
        var applications = _applications.GetAll();
        var documents = _documents.GetAll();
        var report = new List<ReportRow>();
        _logger.LogInformation("Generating scholarship report for {Count} awards", scholarships.Count);

        foreach (var scholarship in scholarships)
        {
            var row = new ReportRow
            {
                Label = scholarship.Name,
                Amount = scholarship.AwardAmount,
                Status = scholarship.IsActive ? "Open" : "Closed"
            };
            var submitted = 0;
            var approved = 0;
            var rejected = 0;
            var pending = 0;
            var withdrawn = 0;
            var needsInfo = 0;
            var awarded = 0m;
            var documentProblems = 0;

            foreach (var application in applications)
            {
                if (application.ScholarshipId != scholarship.ScholarshipId)
                {
                    continue;
                }

                submitted = submitted + 1;
                var statusName = application.Status?.StatusName ?? string.Empty;
                if (string.IsNullOrWhiteSpace(statusName))
                {
                    var full = _applications.GetById(application.ApplicationId);
                    statusName = full?.Status?.StatusName ?? "Pending";
                }

                if (statusName == "Approved")
                {
                    approved = approved + 1;
                    awarded = awarded + application.RequestedAmount;
                }
                else if (statusName == "Rejected")
                {
                    rejected = rejected + 1;
                }
                else if (statusName == "Withdrawn")
                {
                    withdrawn = withdrawn + 1;
                }
                else if (statusName == "NeedsInfo")
                {
                    needsInfo = needsInfo + 1;
                }
                else if (statusName == "Submitted" || statusName == "Pending")
                {
                    pending = pending + 1;
                }
                else
                {
                    pending = pending + 1;
                }

                foreach (var document in documents)
                {
                    if (document.ApplicationId != application.ApplicationId)
                    {
                        continue;
                    }

                    if (document.IsRequired && (document.Status == "Rejected" || document.Status == "Missing" || document.Status == "Pending"))
                    {
                        documentProblems = documentProblems + 1;
                    }
                }
            }

            row.Count = submitted;
            if (scholarship.Seats <= 0 && approved > 0)
            {
                row.Status = "Filled";
            }
            else if (!scholarship.IsActive && pending > 0)
            {
                row.Status = "Closing";
            }
            else if (documentProblems > 3)
            {
                row.Status = "DocumentRisk";
            }
            else if (approved > rejected && approved > 0)
            {
                row.Status = "Healthy";
            }
            else if (rejected > approved && submitted > 2)
            {
                row.Status = "HighRejection";
            }

            if (scholarship.AwardAmount > 10000m && approved > scholarship.Seats)
            {
                row.Amount = awarded;
                row.Status = "OverAllocated";
            }
            else if (awarded > 0)
            {
                row.Amount = awarded;
            }

            if (scholarship.MinimumGpa >= 3.5m && approved == 0 && submitted > 0)
            {
                row.Status = row.Status + "-Competitive";
            }

            report.Add(row);
            report.Add(new ReportRow
            {
                Label = scholarship.Name + " pending",
                Count = pending,
                Amount = 0,
                Status = "Pending"
            });
            report.Add(new ReportRow
            {
                Label = scholarship.Name + " approved",
                Count = approved,
                Amount = awarded,
                Status = "Approved"
            });
            report.Add(new ReportRow
            {
                Label = scholarship.Name + " rejected",
                Count = rejected,
                Amount = 0,
                Status = "Rejected"
            });
            if (withdrawn > 0 || needsInfo > 0 || documentProblems > 0)
            {
                report.Add(new ReportRow
                {
                    Label = scholarship.Name + " follow-up",
                    Count = withdrawn + needsInfo + documentProblems,
                    Amount = scholarship.AwardAmount,
                    Status = "FollowUp"
                });
            }
        }

        return report;
    }

    public List<ReportRow> GenerateReport()
    {
        var combined = new List<ReportRow>();
        combined.AddRange(StudentReport());
        combined.AddRange(GenerateScholarshipReport());
        combined.AddRange(ApplicationReport());
        combined.AddRange(DecisionStatistics());
        return combined;
    }

    public List<ReportRow> ApplicationReport()
    {
        var applications = _applications.GetAll();
        var groups = new Dictionary<string, ReportRow>();
        foreach (var application in applications)
        {
            var key = application.Status?.StatusName;
            if (string.IsNullOrWhiteSpace(key))
            {
                key = "Unknown";
            }

            if (!groups.ContainsKey(key))
            {
                groups[key] = new ReportRow { Label = key, Status = key };
            }

            groups[key].Count = groups[key].Count + 1;
            groups[key].Amount = groups[key].Amount + application.RequestedAmount;
        }

        return groups.Values.ToList();
    }

    public List<ReportRow> DecisionStatistics()
    {
        var applications = _applications.GetAll();
        var approved = 0;
        var rejected = 0;
        var pending = 0;
        decimal approvedAmount = 0;
        decimal rejectedAmount = 0;
        foreach (var application in applications)
        {
            var statusName = application.Status?.StatusName ?? "Pending";
            if (statusName == "Approved")
            {
                approved = approved + 1;
                approvedAmount = approvedAmount + application.RequestedAmount;
            }
            else if (statusName == "Rejected")
            {
                rejected = rejected + 1;
                rejectedAmount = rejectedAmount + application.RequestedAmount;
            }
            else
            {
                pending = pending + 1;
            }
        }

        return new List<ReportRow>
        {
            new ReportRow { Label = "Approved", Count = approved, Amount = approvedAmount, Status = "Approved" },
            new ReportRow { Label = "Rejected", Count = rejected, Amount = rejectedAmount, Status = "Rejected" },
            new ReportRow { Label = "Pending", Count = pending, Amount = 0, Status = "Pending" }
        };
    }

    public DashboardStats BuildDashboard()
    {
        var students = _students.GetAll();
        var scholarships = _scholarships.GetAll();
        var applications = _applications.GetAll();
        var documents = _documents.GetAll();
        var stats = new DashboardStats
        {
            StudentCount = students.Count,
            ScholarshipCount = scholarships.Count,
            ApplicationCount = applications.Count,
            DocumentCount = documents.Count
        };

        foreach (var application in applications)
        {
            var statusName = application.Status?.StatusName ?? "Pending";
            if (statusName == "Approved")
            {
                stats.ApprovedCount = stats.ApprovedCount + 1;
                stats.TotalAwarded = stats.TotalAwarded + application.RequestedAmount;
            }
            else if (statusName == "Rejected")
            {
                stats.RejectedCount = stats.RejectedCount + 1;
            }
            else
            {
                stats.PendingCount = stats.PendingCount + 1;
            }
        }

        return stats;
    }
}
