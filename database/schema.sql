-- ============================================================================
-- 9-Block Talent Matrix Platform - Database Schema (SQL Server / T-SQL)
-- DDL Definitions for Core Entities, Constraints, and Indexes
-- ============================================================================

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'NineBlockDb')
BEGIN
    CREATE DATABASE NineBlockDb;
END
GO

USE NineBlockDb;
GO

-- 1. Nine Box Quadrants Reference Table
IF OBJECT_ID('dbo.NineBoxQuadrants', 'U') IS NOT NULL
    DROP TABLE dbo.NineBoxQuadrants;
GO

CREATE TABLE dbo.NineBoxQuadrants (
    QuadrantId INT IDENTITY(1,1) PRIMARY KEY,
    BlockNumber INT NOT NULL UNIQUE,
    QuadrantName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NOT NULL,
    ColorHex NVARCHAR(10) NOT NULL,
    MinPerformance DECIMAL(3,2) NOT NULL,
    MaxPerformance DECIMAL(3,2) NOT NULL,
    MinPotential DECIMAL(3,2) NOT NULL,
    MaxPotential DECIMAL(3,2) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);
GO

-- 2. Employees Master Table
IF OBJECT_ID('dbo.Employees', 'U') IS NOT NULL
    DROP TABLE dbo.Employees;
GO

CREATE TABLE dbo.Employees (
    EmployeeId INT IDENTITY(1,1) PRIMARY KEY,
    EmployeeCode NVARCHAR(50) NOT NULL UNIQUE,
    FullName NVARCHAR(200) NOT NULL,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    Department NVARCHAR(100) NOT NULL,
    JobTitle NVARCHAR(150) NOT NULL,
    TenureYears INT NOT NULL DEFAULT 0,
    IsKeyRole BIT NOT NULL DEFAULT 0,
    CompaRatio DECIMAL(4,2) NOT NULL DEFAULT 1.00,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt DATETIME2 NULL
);
GO

-- 3. Review Cycles Table
IF OBJECT_ID('dbo.ReviewCycles', 'U') IS NOT NULL
    DROP TABLE dbo.ReviewCycles;
GO

CREATE TABLE dbo.ReviewCycles (
    CycleId INT IDENTITY(1,1) PRIMARY KEY,
    CycleName NVARCHAR(100) NOT NULL UNIQUE,
    Year INT NOT NULL,
    Quarter NVARCHAR(10) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 0,
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);
GO

-- 4. Assessments Transactional Table
IF OBJECT_ID('dbo.Assessments', 'U') IS NOT NULL
    DROP TABLE dbo.Assessments;
GO

CREATE TABLE dbo.Assessments (
    AssessmentId INT IDENTITY(1,1) PRIMARY KEY,
    EmployeeId INT NOT NULL,
    ReviewCycleId INT NOT NULL,
    PerformanceScore DECIMAL(3,2) NOT NULL,
    PotentialScore DECIMAL(3,2) NOT NULL,
    AssignedBlockNumber INT NOT NULL,
    AssignedQuadrantName NVARCHAR(100) NOT NULL,
    EvaluatorNotes NVARCHAR(MAX) NULL,
    CreatedDate DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    
    CONSTRAINT FK_Assessments_Employees FOREIGN KEY (EmployeeId) 
        REFERENCES dbo.Employees(EmployeeId) ON DELETE CASCADE,
    CONSTRAINT FK_Assessments_ReviewCycles FOREIGN KEY (ReviewCycleId) 
        REFERENCES dbo.ReviewCycles(CycleId) ON DELETE CASCADE,
    CONSTRAINT CK_Assessments_PerformanceScore CHECK (PerformanceScore >= 1.00 AND PerformanceScore <= 5.00),
    CONSTRAINT CK_Assessments_PotentialScore CHECK (PotentialScore >= 1.00 AND PotentialScore <= 5.00)
);
GO

-- Indexes for Query Performance & Analytical Filtering
CREATE NONCLUSTERED INDEX IX_Assessments_Employee_Cycle 
    ON dbo.Assessments(EmployeeId, ReviewCycleId);
GO

CREATE NONCLUSTERED INDEX IX_Assessments_Scores 
    ON dbo.Assessments(PerformanceScore, PotentialScore, AssignedBlockNumber);
GO

CREATE NONCLUSTERED INDEX IX_Employees_Department 
    ON dbo.Employees(Department, IsActive);
GO
