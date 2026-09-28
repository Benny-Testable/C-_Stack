-- ============================================================================
-- 9-Block Talent Matrix Platform - Stored Procedures (SQL Server / T-SQL)
-- Positive Case: 100% Parameterized, Strict Type Safety, Zero Dynamic SQL Flaws
-- ============================================================================

USE NineBlockDb;
GO

-- 1. Secure Parameterized Query: Get Assessments by Cycle & Department
IF OBJECT_ID('dbo.usp_GetEmployeeAssessmentMatrix', 'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_GetEmployeeAssessmentMatrix;
GO

CREATE PROCEDURE dbo.usp_GetEmployeeAssessmentMatrix
    @ReviewCycleId INT,
    @Department NVARCHAR(100) = NULL,
    @BlockNumber INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Clean, parameterized SELECT preventing any SQL Injection vulnerability
    SELECT 
        e.EmployeeId,
        e.EmployeeCode,
        e.FullName,
        e.Email,
        e.Department,
        e.JobTitle,
        e.TenureYears,
        e.IsKeyRole,
        a.AssessmentId,
        a.PerformanceScore,
        a.PotentialScore,
        a.AssignedBlockNumber,
        a.AssignedQuadrantName,
        q.ColorHex,
        q.TalentTier,
        a.EvaluatorNotes,
        a.CreatedDate
    FROM dbo.Assessments a
    INNER JOIN dbo.Employees e ON a.EmployeeId = e.EmployeeId
    LEFT JOIN dbo.NineBoxQuadrants q ON a.AssignedBlockNumber = q.BlockNumber
    WHERE a.ReviewCycleId = @ReviewCycleId
      AND (@Department IS NULL OR e.Department = @Department)
      AND (@BlockNumber IS NULL OR a.AssignedBlockNumber = @BlockNumber)
      AND e.IsActive = 1
    ORDER BY a.AssignedBlockNumber ASC, e.FullName ASC;
END
GO

-- 2. Secure Transactional Procedure: Record or Update Employee Assessment
IF OBJECT_ID('dbo.usp_RecordAssessment', 'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_RecordAssessment;
GO

CREATE PROCEDURE dbo.usp_RecordAssessment
    @EmployeeId INT,
    @ReviewCycleId INT,
    @PerformanceScore DECIMAL(3,2),
    @PotentialScore DECIMAL(3,2),
    @EvaluatorNotes NVARCHAR(MAX) = NULL,
    @NewAssessmentId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    -- Defensive Parameter Validation
    IF @PerformanceScore < 1.00 OR @PerformanceScore > 5.00
    BEGIN
        RAISERROR('Performance score must be between 1.00 and 5.00', 16, 1);
        RETURN -1;
    END

    IF @PotentialScore < 1.00 OR @PotentialScore > 5.00
    BEGIN
        RAISERROR('Potential score must be between 1.00 and 5.00', 16, 1);
        RETURN -2;
    END

    -- Determine 9-Box Coordinates & Quadrant Deterministically
    DECLARE @X INT = CASE WHEN @PerformanceScore < 3.00 THEN 1 WHEN @PerformanceScore < 4.00 THEN 2 ELSE 3 END;
    DECLARE @Y INT = CASE WHEN @PotentialScore   < 3.00 THEN 1 WHEN @PotentialScore   < 4.00 THEN 2 ELSE 3 END;

    DECLARE @BlockNumber INT;
    DECLARE @QuadrantName NVARCHAR(100);

    SELECT 
        @BlockNumber = CASE 
            WHEN @X = 1 AND @Y = 3 THEN 1
            WHEN @X = 2 AND @Y = 3 THEN 2
            WHEN @X = 3 AND @Y = 3 THEN 3
            WHEN @X = 1 AND @Y = 2 THEN 4
            WHEN @X = 2 AND @Y = 2 THEN 5
            WHEN @X = 3 AND @Y = 2 THEN 6
            WHEN @X = 1 AND @Y = 1 THEN 7
            WHEN @X = 2 AND @Y = 1 THEN 8
            WHEN @X = 3 AND @Y = 1 THEN 9
            ELSE 5
        END;

    SELECT @QuadrantName = QuadrantName 
    FROM dbo.NineBoxQuadrants 
    WHERE BlockNumber = @BlockNumber;

    IF @QuadrantName IS NULL
        SET @QuadrantName = 'Core Player';

    BEGIN TRANSACTION;

    -- Upsert Assessment Record
    IF EXISTS (SELECT 1 FROM dbo.Assessments WHERE EmployeeId = @EmployeeId AND ReviewCycleId = @ReviewCycleId)
    BEGIN
        UPDATE dbo.Assessments
        SET PerformanceScore    = @PerformanceScore,
            PotentialScore      = @PotentialScore,
            AssignedBlockNumber = @BlockNumber,
            AssignedQuadrantName = @QuadrantName,
            EvaluatorNotes      = @EvaluatorNotes,
            CreatedDate         = SYSUTCDATETIME()
        WHERE EmployeeId = @EmployeeId AND ReviewCycleId = @ReviewCycleId;

        SELECT @NewAssessmentId = AssessmentId 
        FROM dbo.Assessments 
        WHERE EmployeeId = @EmployeeId AND ReviewCycleId = @ReviewCycleId;
    END
    ELSE
    BEGIN
        INSERT INTO dbo.Assessments (
            EmployeeId, ReviewCycleId, PerformanceScore, PotentialScore,
            AssignedBlockNumber, AssignedQuadrantName, EvaluatorNotes, CreatedDate
        )
        VALUES (
            @EmployeeId, @ReviewCycleId, @PerformanceScore, @PotentialScore,
            @BlockNumber, @QuadrantName, @EvaluatorNotes, SYSUTCDATETIME()
        );

        SET @NewAssessmentId = SCOPE_IDENTITY();
    END

    COMMIT TRANSACTION;
    RETURN 0;
END
GO

-- 3. Department Talent Distribution Aggregation
IF OBJECT_ID('dbo.usp_GetDepartmentTalentDistribution', 'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_GetDepartmentTalentDistribution;
GO

CREATE PROCEDURE dbo.usp_GetDepartmentTalentDistribution
    @ReviewCycleId INT,
    @Department NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        q.BlockNumber,
        q.QuadrantName,
        q.ColorHex,
        COUNT(a.AssessmentId) AS EmployeeCount,
        CAST(
            CASE 
                WHEN (SELECT COUNT(*) FROM dbo.Assessments a2 INNER JOIN dbo.Employees e2 ON a2.EmployeeId = e2.EmployeeId WHERE a2.ReviewCycleId = @ReviewCycleId AND (@Department IS NULL OR e2.Department = @Department)) = 0 
                THEN 0 
                ELSE (COUNT(a.AssessmentId) * 100.0) / (SELECT COUNT(*) FROM dbo.Assessments a2 INNER JOIN dbo.Employees e2 ON a2.EmployeeId = e2.EmployeeId WHERE a2.ReviewCycleId = @ReviewCycleId AND (@Department IS NULL OR e2.Department = @Department))
            END 
            AS DECIMAL(5,2)
        ) AS PercentageOfTotal
    FROM dbo.NineBoxQuadrants q
    LEFT JOIN dbo.Assessments a 
        ON q.BlockNumber = a.AssignedBlockNumber AND a.ReviewCycleId = @ReviewCycleId
    LEFT JOIN dbo.Employees e 
        ON a.EmployeeId = e.EmployeeId AND (@Department IS NULL OR e.Department = @Department) AND e.IsActive = 1
    GROUP BY q.BlockNumber, q.QuadrantName, q.ColorHex
    ORDER BY q.BlockNumber ASC;
END
GO
