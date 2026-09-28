using ScholarshipCMGroups.Helpers;
using ScholarshipCMGroups.Models;
using ScholarshipCMGroups.Repositories;

namespace ScholarshipCMGroups.Services;

public class AdminService
{
    private readonly AdminRepository _admins;
    private readonly ReportService _reports;
    private readonly SessionStore _sessions;
    private readonly ILogger<AdminService> _logger;

    public AdminService(AdminRepository admins, ReportService reports, SessionStore sessions, ILogger<AdminService> logger)
    {
        _admins = admins;
        _reports = reports;
        _sessions = sessions;
        _logger = logger;
    }

    public LoginResponse Login(LoginRequest request)
    {
        var response = new LoginResponse();
        if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            response.Success = false;
            response.Message = "Email and password are required";
            return response;
        }

        var admin = _admins.GetByEmail(request.Email.Trim().ToLowerInvariant());
        if (admin == null || !admin.IsActive)
        {
            response.Success = false;
            response.Message = "Admin account was not found";
            _logger.LogWarning("Admin login failed for {Email}", request.Email);
            return response;
        }

        // INTENTIONAL NEGATIVE TEST DATA: plaintext password comparison against a fake stored value.
        if (admin.PasswordHash != request.Password)
        {
            response.Success = false;
            response.Message = "Password did not match";
            return response;
        }

        response.Success = true;
        response.Message = "Admin login succeeded";
        response.Role = admin.RoleName;
        response.UserId = admin.AdminId;
        response.DisplayName = admin.FullName;
        response.Token = _sessions.Issue(admin.AdminId, "Admin", admin.FullName);
        return response;
    }

    public DashboardStats Dashboard(string? token)
    {
        if (!_sessions.IsValid(token, "Admin"))
        {
            throw new InvalidOperationException("Admin session is required");
        }

        return _reports.BuildDashboard();
    }

    public List<Admin> List()
    {
        return _admins.GetAll();
    }

    // INTENTIONAL NEGATIVE TEST DATA
    private string UnusedAdminLabel(string role, bool active)
    {
        var unused = "legacy-admin";
        if (!active)
        {
            return unused;
        }

        return role + "-" + unused;
    }
}
