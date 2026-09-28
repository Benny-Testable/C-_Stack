-- Synthetic seed data. Emails use example.test. Passwords are the obvious fake value TEST_ONLY_FAKE_PASSWORD.
-- INTENTIONAL NEGATIVE TEST DATA: passwords are stored in plaintext for metric validation.

IF NOT EXISTS (SELECT 1 FROM dbo.ApplicationStatus WHERE StatusName = N'Pending')
INSERT INTO dbo.ApplicationStatus (StatusName, Description, SortOrder) VALUES
(N'Pending', N'Waiting for submission review', 1),
(N'Submitted', N'Received and waiting for a reviewer', 2),
(N'NeedsInfo', N'Reviewer asked for more information', 3),
(N'Approved', N'Award approved', 4),
(N'Rejected', N'Award declined', 5),
(N'Withdrawn', N'Student withdrew the application', 6);

IF NOT EXISTS (SELECT 1 FROM dbo.ScholarshipCategories WHERE CategoryCode = N'MERIT')
INSERT INTO dbo.ScholarshipCategories (CategoryCode, CategoryName, Description, IsActive) VALUES
(N'MERIT', N'Merit award', N'Recognizes a high GPA', 1),
(N'NEED', N'Need award', N'Supports students below an income ceiling', 1),
(N'STEM', N'STEM award', N'Supports science and engineering majors', 1),
(N'COMMUNITY', N'Community award', N'Supports local students', 1),
(N'ATHLETIC', N'Athletic award', N'Supports full-time students', 1);

IF NOT EXISTS (SELECT 1 FROM dbo.Admins WHERE Email = N'riley.morgan@example.test')
INSERT INTO dbo.Admins (FullName, Email, PasswordHash, RoleName, IsActive) VALUES
(N'Riley Morgan', N'riley.morgan@example.test', N'TEST_ONLY_FAKE_PASSWORD', N'Admin', 1),
(N'Quinn Harper', N'quinn.harper@example.test', N'TEST_ONLY_FAKE_PASSWORD', N'Reviewer', 1);

IF NOT EXISTS (SELECT 1 FROM dbo.Students WHERE Email = N'ava.nguyen@example.test')
INSERT INTO dbo.Students
(FirstName, LastName, Email, PasswordHash, Phone, DateOfBirth, Address, City, Residency, Gpa, Major, EnrollmentYear, CreditHours, AnnualIncome, IsActive, Notes)
VALUES
(N'Ava', N'Nguyen', N'ava.nguyen@example.test', N'TEST_ONLY_FAKE_PASSWORD', N'555-0101', '2004-03-12', N'10 Campus Way', N'Riverdale', N'InState', 3.80, N'Biology', 2023, 64, 22000, 1, N'Honor roll synthetic profile'),
(N'Noah', N'Patel', N'noah.patel@example.test', N'TEST_ONLY_FAKE_PASSWORD', N'555-0102', '2005-07-02', N'18 Library Lane', N'Riverdale', N'InState', 3.40, N'Computer Science', 2024, 36, 18000, 1, N'Lab assistant'),
(N'Mia', N'Johnson', N'mia.johnson@example.test', N'TEST_ONLY_FAKE_PASSWORD', N'555-0103', '2003-11-19', N'4 Oak Street', N'Lakeside', N'OutOfState', 3.10, N'Nursing', 2022, 78, 41000, 1, N'Clinical rotation'),
(N'Leo', N'Garcia', N'leo.garcia@example.test', N'TEST_ONLY_FAKE_PASSWORD', N'555-0104', '2004-01-30', N'77 Market Road', N'Harbor', N'InState', 2.70, N'Business', 2023, 48, 52000, 1, N'Part-time work'),
(N'Sofia', N'Kim', N'sofia.kim@example.test', N'TEST_ONLY_FAKE_PASSWORD', N'555-0105', '2006-05-08', N'9 Station Court', N'Harbor', N'International', 3.90, N'Chemistry', 2025, 18, 15000, 1, N'Research interest'),
(N'Ethan', N'Brooks', N'ethan.brooks@example.test', N'TEST_ONLY_FAKE_PASSWORD', N'555-0106', '2002-09-14', N'21 Field Drive', N'Riverdale', N'InState', 3.60, N'Engineering', 2021, 96, 27000, 1, N'Senior design'),
(N'Lila', N'Singh', N'lila.singh@example.test', N'TEST_ONLY_FAKE_PASSWORD', N'555-0107', '2005-12-01', N'6 Pine Avenue', N'Lakeside', N'OutOfState', 2.40, N'Education', 2024, 24, 30000, 0, N'Inactive term'),
(N'Omar', N'Haddad', N'omar.haddad@example.test', N'TEST_ONLY_FAKE_PASSWORD', N'555-0108', '2004-08-22', N'14 Cedar Place', N'Riverdale', N'InState', 3.55, N'Computer Science', 2023, 60, 19000, 1, N'Tutoring cohort');

IF NOT EXISTS (SELECT 1 FROM dbo.Scholarships WHERE Name = N'Riverdale Merit Award')
INSERT INTO dbo.Scholarships
(Name, Sponsor, Description, CategoryId, AwardAmount, MinimumGpa, MinimumCreditHours, MaximumIncome, RequiredMajor, RequiredResidency, RequiresEssay, RequiresTranscript, OpenDate, Deadline, Seats, IsActive)
VALUES
(N'Riverdale Merit Award', N'Riverdale Civic Fund', N'Recognizes students with a sustained high GPA.', 1, 5000, 3.50, 30, 0, N'Any', N'Any', 1, 1, '2026-01-01', '2026-12-15', 8, 1),
(N'Harbor Need Grant', N'Harbor Community Trust', N'Supports students under the published income ceiling.', 2, 3500, 2.50, 12, 45000, N'Any', N'InState', 1, 1, '2026-01-01', '2026-11-30', 12, 1),
(N'STEM Lab Scholarship', N'North Lab Collective', N'Supports biology, chemistry, computing, and engineering students.', 3, 6000, 3.20, 24, 80000, N'Any', N'Any', 1, 1, '2026-02-01', '2026-12-01', 6, 1),
(N'Lakeside Community Award', N'Lakeside Neighbors', N'Supports students who list a local city.', 4, 2000, 2.80, 12, 70000, N'Any', N'Any', 0, 1, '2026-03-01', '2026-10-31', 10, 1),
(N'Full Course Athletic Grant', N'Campus Activity Board', N'Supports students carrying a full course load.', 5, 2500, 2.50, 12, 0, N'Any', N'Any', 0, 1, '2026-01-15', '2026-09-30', 4, 1);

IF NOT EXISTS (SELECT 1 FROM dbo.Applications)
INSERT INTO dbo.Applications
(StudentId, ScholarshipId, ApplicationStatusId, EssayText, ReviewerNote, RequestedAmount, HistoryNote)
VALUES
(1, 1, 2, N'I plan to continue biology research next term.', N'', 5000, N'Submitted'),
(2, 3, 2, N'I am seeking support for computing coursework.', N'', 6000, N'Submitted'),
(3, 2, 1, N'Nursing costs increased this year.', N'', 3500, N'Pending'),
(6, 1, 4, N'Senior design needs materials funding.', N'Approved in review', 5000, N'Approved'),
(8, 3, 5, N'Computing lab fees are due this term.', N'Incomplete packet', 6000, N'Rejected');

IF NOT EXISTS (SELECT 1 FROM dbo.Documents)
INSERT INTO dbo.Documents (ApplicationId, FileName, DocumentType, Status, IsRequired, Notes)
VALUES
(1, N'ava-transcript.pdf', N'Transcript', N'Validated', 1, N'Synthetic transcript'),
(1, N'ava-essay.pdf', N'Essay', N'Validated', 1, N'Synthetic essay'),
(2, N'noah-transcript.pdf', N'Transcript', N'Pending', 1, N'Awaiting check'),
(2, N'noah-essay.pdf', N'Essay', N'Pending', 1, N'Awaiting check'),
(3, N'mia-transcript.pdf', N'Transcript', N'Accepted', 1, N'Synthetic transcript'),
(4, N'ethan-transcript.pdf', N'Transcript', N'Validated', 1, N'Synthetic transcript'),
(5, N'omar-transcript.pdf', N'Transcript', N'Rejected', 1, N'Missing pages');
