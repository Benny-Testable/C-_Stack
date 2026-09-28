-- Scholarship CMGroups schema for SQL Server.
-- Synthetic test database. No production secrets.

IF OBJECT_ID(N'dbo.Documents', N'U') IS NOT NULL ALTER TABLE dbo.Documents DROP CONSTRAINT IF EXISTS FK_Documents_Applications;
IF OBJECT_ID(N'dbo.Applications', N'U') IS NOT NULL
BEGIN
    ALTER TABLE dbo.Applications DROP CONSTRAINT IF EXISTS FK_Applications_Students;
    ALTER TABLE dbo.Applications DROP CONSTRAINT IF EXISTS FK_Applications_Scholarships;
    ALTER TABLE dbo.Applications DROP CONSTRAINT IF EXISTS FK_Applications_Status;
END
IF OBJECT_ID(N'dbo.Scholarships', N'U') IS NOT NULL ALTER TABLE dbo.Scholarships DROP CONSTRAINT IF EXISTS FK_Scholarships_Categories;

IF OBJECT_ID(N'dbo.Documents', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Documents
    (
        DocumentId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        ApplicationId INT NOT NULL,
        FileName NVARCHAR(200) NOT NULL,
        DocumentType NVARCHAR(80) NOT NULL,
        Status NVARCHAR(40) NOT NULL,
        IsRequired BIT NOT NULL,
        Notes NVARCHAR(1000) NOT NULL CONSTRAINT DF_Documents_Notes DEFAULT (N''),
        UploadedAt DATETIME2 NOT NULL CONSTRAINT DF_Documents_UploadedAt DEFAULT (SYSUTCDATETIME())
    );
END

IF OBJECT_ID(N'dbo.Applications', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Applications
    (
        ApplicationId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        StudentId INT NOT NULL,
        ScholarshipId INT NOT NULL,
        ApplicationStatusId INT NOT NULL,
        EssayText NVARCHAR(4000) NOT NULL CONSTRAINT DF_Applications_Essay DEFAULT (N''),
        ReviewerNote NVARCHAR(1000) NOT NULL CONSTRAINT DF_Applications_Note DEFAULT (N''),
        SubmittedAt DATETIME2 NOT NULL CONSTRAINT DF_Applications_Submitted DEFAULT (SYSUTCDATETIME()),
        ReviewedAt DATETIME2 NULL,
        ReviewedByAdminId INT NULL,
        RequestedAmount DECIMAL(12,2) NOT NULL,
        HistoryNote NVARCHAR(2000) NOT NULL CONSTRAINT DF_Applications_History DEFAULT (N'')
    );
END

IF OBJECT_ID(N'dbo.Scholarships', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Scholarships
    (
        ScholarshipId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        Name NVARCHAR(160) NOT NULL,
        Sponsor NVARCHAR(160) NOT NULL,
        Description NVARCHAR(2000) NOT NULL,
        CategoryId INT NOT NULL,
        AwardAmount DECIMAL(12,2) NOT NULL,
        MinimumGpa DECIMAL(4,2) NOT NULL,
        MinimumCreditHours INT NOT NULL,
        MaximumIncome DECIMAL(12,2) NOT NULL,
        RequiredMajor NVARCHAR(80) NOT NULL,
        RequiredResidency NVARCHAR(40) NOT NULL,
        RequiresEssay BIT NOT NULL,
        RequiresTranscript BIT NOT NULL,
        OpenDate DATETIME2 NOT NULL,
        Deadline DATETIME2 NOT NULL,
        Seats INT NOT NULL,
        IsActive BIT NOT NULL,
        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Scholarships_Created DEFAULT (SYSUTCDATETIME()),
        UpdatedAt DATETIME2 NOT NULL CONSTRAINT DF_Scholarships_Updated DEFAULT (SYSUTCDATETIME())
    );
END

IF OBJECT_ID(N'dbo.ScholarshipCategories', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ScholarshipCategories
    (
        CategoryId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        CategoryCode NVARCHAR(40) NOT NULL,
        CategoryName NVARCHAR(80) NOT NULL,
        Description NVARCHAR(400) NOT NULL,
        IsActive BIT NOT NULL
    );
END

IF OBJECT_ID(N'dbo.ApplicationStatus', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ApplicationStatus
    (
        ApplicationStatusId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        StatusName NVARCHAR(40) NOT NULL,
        Description NVARCHAR(400) NOT NULL,
        SortOrder INT NOT NULL
    );
END

IF OBJECT_ID(N'dbo.Students', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Students
    (
        StudentId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        FirstName NVARCHAR(60) NOT NULL,
        LastName NVARCHAR(60) NOT NULL,
        Email NVARCHAR(120) NOT NULL,
        PasswordHash NVARCHAR(200) NOT NULL,
        Phone NVARCHAR(30) NOT NULL,
        DateOfBirth DATETIME2 NOT NULL,
        Address NVARCHAR(200) NOT NULL,
        City NVARCHAR(80) NOT NULL,
        Residency NVARCHAR(40) NOT NULL,
        Gpa DECIMAL(4,2) NOT NULL,
        Major NVARCHAR(80) NOT NULL,
        EnrollmentYear INT NOT NULL,
        CreditHours INT NOT NULL,
        AnnualIncome DECIMAL(12,2) NOT NULL,
        IsActive BIT NOT NULL,
        Notes NVARCHAR(2000) NOT NULL CONSTRAINT DF_Students_Notes DEFAULT (N''),
        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Students_Created DEFAULT (SYSUTCDATETIME()),
        UpdatedAt DATETIME2 NOT NULL CONSTRAINT DF_Students_Updated DEFAULT (SYSUTCDATETIME())
    );
END

IF OBJECT_ID(N'dbo.Admins', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Admins
    (
        AdminId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        FullName NVARCHAR(120) NOT NULL,
        Email NVARCHAR(120) NOT NULL,
        PasswordHash NVARCHAR(200) NOT NULL,
        RoleName NVARCHAR(40) NOT NULL,
        IsActive BIT NOT NULL,
        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Admins_Created DEFAULT (SYSUTCDATETIME())
    );
END

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Scholarships_Categories')
    ALTER TABLE dbo.Scholarships ADD CONSTRAINT FK_Scholarships_Categories FOREIGN KEY (CategoryId) REFERENCES dbo.ScholarshipCategories(CategoryId);

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Applications_Students')
    ALTER TABLE dbo.Applications ADD CONSTRAINT FK_Applications_Students FOREIGN KEY (StudentId) REFERENCES dbo.Students(StudentId);

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Applications_Scholarships')
    ALTER TABLE dbo.Applications ADD CONSTRAINT FK_Applications_Scholarships FOREIGN KEY (ScholarshipId) REFERENCES dbo.Scholarships(ScholarshipId);

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Applications_Status')
    ALTER TABLE dbo.Applications ADD CONSTRAINT FK_Applications_Status FOREIGN KEY (ApplicationStatusId) REFERENCES dbo.ApplicationStatus(ApplicationStatusId);

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Documents_Applications')
    ALTER TABLE dbo.Documents ADD CONSTRAINT FK_Documents_Applications FOREIGN KEY (ApplicationId) REFERENCES dbo.Applications(ApplicationId);
