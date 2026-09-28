-- ============================================================================
-- 9-Block Talent Matrix Platform - Seed Data
-- Initial reference quadrants, active review cycle, and calibration employees
-- ============================================================================

USE NineBlockDb;
GO

SET NOCOUNT ON;

-- 1. Populate the 9 Standard Quadrants
INSERT INTO dbo.NineBoxQuadrants 
    (BlockNumber, QuadrantName, Description, ColorHex, MinPerformance, MaxPerformance, MinPotential, MaxPotential)
VALUES
    (1, N'Enigma', N'High Potential, Low Performance - Rough Diamond requiring coaching', N'#F39C12', 1.00, 2.99, 4.00, 5.00),
    (2, N'Growth Potential', N'High Potential, Medium Performance - Emerging Leader candidate', N'#27AE60', 3.00, 3.99, 4.00, 5.00),
    (3, N'Star', N'High Potential, High Performance - Top Talent & Primary Succession Candidate', N'#2ECC71', 4.00, 5.00, 4.00, 5.00),
    (4, N'Dilemma', N'Medium Potential, Low Performance - Underperforming, Needs Assessment', N'#E67E22', 1.00, 2.99, 3.00, 3.99),
    (5, N'Core Player', N'Medium Potential, Medium Performance - Steady Contributor', N'#3498DB', 3.00, 3.99, 3.00, 3.99),
    (6, N'High Performer', N'Medium Potential, High Performance - Strong Execution Specialist', N'#1ABC9C', 4.00, 5.00, 3.00, 3.99),
    (7, N'Risk', N'Low Potential, Low Performance - Action Required / Reassignment', N'#E74C3C', 1.00, 2.99, 1.00, 2.99),
    (8, N'Effective', N'Low Potential, Medium Performance - Solid Performer in current role', N'#95A5A6', 3.00, 3.99, 1.00, 2.99),
    (9, N'Solid Professional', N'Low Potential, High Performance - Master of domain, low mobility', N'#34495E', 4.00, 5.00, 1.00, 2.99);
GO

-- 2. Populate Review Cycles
INSERT INTO dbo.ReviewCycles (CycleName, Year, Quarter, IsActive, StartDate, EndDate)
VALUES
    (N'2026-Q1 Talent Calibration', 2026, N'Q1', 1, '2026-01-01', '2026-03-31'),
    (N'2026-Q2 Midyear Review', 2026, N'Q2', 0, '2026-04-01', '2026-06-30');
GO

-- 3. Populate Sample Employees
INSERT INTO dbo.Employees (EmployeeCode, FullName, Email, Department, JobTitle, TenureYears, IsKeyRole, CompaRatio, IsActive)
VALUES
    (N'EMP-101', N'Alice Johnson', N'alice.johnson@example.com', N'Engineering', N'Senior Software Engineer', 3, 1, 0.95, 1),
    (N'EMP-102', N'Bob Smith', N'bob.smith@example.com', N'Product', N'Product Manager', 4, 1, 1.05, 1),
    (N'EMP-103', N'Charlie Brown', N'charlie.brown@example.com', N'Engineering', N'Staff Systems Architect', 6, 1, 1.15, 1),
    (N'EMP-104', N'Dana Scully', N'dana.scully@example.com', N'Operations', N'Operations Analyst', 2, 0, 0.88, 1),
    (N'EMP-105', N'Evan Wright', N'evan.wright@example.com', N'Sales', N'Account Executive', 5, 0, 1.00, 1),
    (N'EMP-106', N'Fiona Gallagher', N'fiona.gallagher@example.com', N'Marketing', N'Marketing Director', 7, 1, 1.10, 1),
    (N'EMP-107', N'George Costanza', N'george.costanza@example.com', N'HR', N'HR Coordinator', 1, 0, 0.82, 1),
    (N'EMP-108', N'Hannah Montana', N'hannah.montana@example.com', N'Sales', N'Regional Sales Lead', 4, 0, 0.98, 1),
    (N'EMP-109', N'Ian Malcolm', N'ian.malcolm@example.com', N'Engineering', N'Principal Data Scientist', 8, 1, 1.20, 1);
GO

-- 4. Initial Calibration Assessments
INSERT INTO dbo.Assessments (EmployeeId, ReviewCycleId, PerformanceScore, PotentialScore, AssignedBlockNumber, AssignedQuadrantName, EvaluatorNotes)
VALUES
    (1, 1, 2.40, 4.80, 1, N'Enigma', N'High innovation capabilities, needs delivery discipline.'),
    (2, 1, 3.60, 4.20, 2, N'Growth Potential', N'Strong product sense, candidate for group product lead.'),
    (3, 1, 4.90, 4.90, 3, N'Star', N'Consistently exceeds high standards; primary tech lead candidate.'),
    (4, 1, 2.10, 3.20, 4, N'Dilemma', N'Transitioning roles, requires 90-day performance milestones.'),
    (5, 1, 3.40, 3.50, 5, N'Core Player', N'Solid quota attainment and consistent team player.'),
    (6, 1, 4.60, 3.80, 6, N'High Performer', N'Exceptional campaign execution; strategic scope expanding.'),
    (7, 1, 1.50, 1.80, 7, N'Risk', N'Performance improvement plan initiated.'),
    (8, 1, 3.20, 2.10, 8, N'Effective', N'Reliable delivery within current territorial scope.'),
    (9, 1, 4.80, 2.40, 9, N'Solid Professional', N'World-class subject matter expertise in data infrastructure.');
GO
