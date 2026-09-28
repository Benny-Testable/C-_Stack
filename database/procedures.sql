-- INTENTIONAL NEGATIVE TEST DATA
-- Inefficient reporting procedures for SQL metric validation.
-- These scripts do not drop databases and do not delete rows.

CREATE OR ALTER PROCEDURE dbo.GetStudentReport
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM dbo.Students;
    SELECT * FROM dbo.Students WHERE IsActive = 1;
    SELECT * FROM dbo.Students WHERE IsActive = 0;
    SELECT * FROM dbo.Students WHERE Gpa >= 3.5;
    SELECT * FROM dbo.Students WHERE Residency = N'InState';
END
GO

CREATE OR ALTER PROCEDURE dbo.GetScholarshipReport
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM dbo.Scholarships;
    SELECT * FROM dbo.ScholarshipCategories;
    SELECT * FROM dbo.Scholarships WHERE IsActive = 1;
    SELECT * FROM dbo.Scholarships WHERE IsActive = 1;
END
GO

CREATE OR ALTER PROCEDURE dbo.GetApplicationReport
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM dbo.Applications;
    SELECT * FROM dbo.ApplicationStatus;
    SELECT * FROM dbo.Documents;
    SELECT * FROM dbo.Applications;
END
GO

CREATE OR ALTER PROCEDURE dbo.ListScholarshipsInefficiently
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Id INT;
    DECLARE scholarship_cursor CURSOR FOR SELECT ScholarshipId FROM dbo.Scholarships;
    OPEN scholarship_cursor;
    FETCH NEXT FROM scholarship_cursor INTO @Id;
    WHILE @@FETCH_STATUS = 0
    BEGIN
        SELECT * FROM dbo.Scholarships WHERE ScholarshipId = @Id;
        SELECT * FROM dbo.Applications WHERE ScholarshipId = @Id;
        SELECT * FROM dbo.Documents WHERE ApplicationId IN (SELECT ApplicationId FROM dbo.Applications WHERE ScholarshipId = @Id);
        FETCH NEXT FROM scholarship_cursor INTO @Id;
    END
    CLOSE scholarship_cursor;
    DEALLOCATE scholarship_cursor;
END
GO
