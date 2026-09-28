-- ============================================================================
-- 9-Block Talent Matrix Platform - Stored Procedures
-- Includes standard analytical distribution and intentional SQL SAST test fixtures
-- ============================================================================

USE NineBlockDb;
GO

-- 1. Standard Production Procedure: Calculate Nine-Box Distribution
IF OBJECT_ID('dbo.sp_GetNineBoxDistribution', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_GetNineBoxDistribution;
GO

CREATE PROCEDURE dbo.sp_GetNineBoxDistribution
    @ReviewCycleId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @ActiveCycleId INT = @ReviewCycleId;

    IF @ActiveCycleId IS NULL
    BEGIN
        SELECT TOP 1 @ActiveCycleId = CycleId 
        FROM dbo.ReviewCycles 
        WHERE IsActive = 1;
    END

    SELECT 
        q.BlockNumber,
        q.QuadrantName,
        q.ColorHex,
        COUNT(a.AssessmentId) AS EmployeeCount,
        CAST(
            CASE 
                WHEN (SELECT COUNT(*) FROM dbo.Assessments WHERE ReviewCycleId = @ActiveCycleId) = 0 THEN 0.0
                ELSE (COUNT(a.AssessmentId) * 100.0) / (SELECT COUNT(*) FROM dbo.Assessments WHERE ReviewCycleId = @ActiveCycleId)
            END AS DECIMAL(5,2)
        ) AS PercentageOfTotal
    FROM dbo.NineBoxQuadrants q
    LEFT JOIN dbo.Assessments a 
        ON q.BlockNumber = a.AssignedBlockNumber 
        AND a.ReviewCycleId = @ActiveCycleId
    GROUP BY q.BlockNumber, q.QuadrantName, q.ColorHex
    ORDER BY q.BlockNumber ASC;
END;
GO

-- 2. [NEGATIVE SAST FIXTURE]: Dynamic Unparameterized SQL Concatenation (CWE-89)
-- Detectable by database static analysis tools (T-SQL analyzers / SonarQube / Semgrep SQL rules)
IF OBJECT_ID('dbo.sp_SearchEmployeeAssessmentsRaw', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_SearchEmployeeAssessmentsRaw;
GO

CREATE PROCEDURE dbo.sp_SearchEmployeeAssessmentsRaw
    @DepartmentFilter NVARCHAR(100),
    @MinScore DECIMAL(3,2) = 1.00
AS
BEGIN
    SET NOCOUNT ON;

    -- VULNERABILITY FIXTURE: Dynamically concatenating unescaped user parameter directly into SQL execution string
    DECLARE @DynamicSql NVARCHAR(MAX);
    SET @DynamicSql = N'SELECT e.EmployeeCode, e.FullName, e.Department, a.PerformanceScore, a.PotentialScore, a.AssignedQuadrantName ' +
                      N'FROM dbo.Employees e ' +
                      N'INNER JOIN dbo.Assessments a ON e.EmployeeId = a.EmployeeId ' +
                      N'WHERE e.Department = ''' + @DepartmentFilter + N''' AND a.PerformanceScore >= ' + CAST(@MinScore AS NVARCHAR(10));

    -- Execution of tainted dynamic SQL
    EXEC(@DynamicSql);
END;
GO
