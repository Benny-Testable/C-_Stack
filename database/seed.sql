-- ============================================================================
-- 9-Block Talent Matrix Platform - Seed Data (SQL Server / T-SQL)
-- Reference Quadrants, Active Review Cycles, Sample Employees & Baseline Assessments
-- (Positive Case: Normalized, Clean Relational Integrity)
-- ============================================================================

USE NineBlockDb;
GO

-- 1. Populate Nine Box Quadrants Reference
IF NOT EXISTS (SELECT 1 FROM dbo.NineBoxQuadrants)
BEGIN
    INSERT INTO dbo.NineBoxQuadrants (BlockNumber, QuadrantName, Description, ColorHex, MinPerformance, MaxPerformance, MinPotential, MaxPotential)
    VALUES
        (1, 'Enigma',             'High Potential, Low Performance. Needs direction, root cause exploration.', '#F39C12', 1.00, 2.99, 4.00, 5.00),
        (2, 'Growth Potential',   'High Potential, Medium Performance. Ready for stretch assignments.',        '#27AE60', 3.00, 3.99, 4.00, 5.00),
        (3, 'Star',               'High Potential, High Performance. Future executive leadership pipeline.',   '#2ECC71', 4.00, 5.00, 4.00, 5.00),
        (4, 'Dilemma',            'Medium Potential, Low Performance. Requires performance improvement plan.', '#E67E22', 1.00, 2.99, 3.00, 3.99),
        (5, 'Core Player',        'Medium Potential, Medium Performance. Solid backbone of operational teams.', '#3498DB', 3.00, 3.99, 3.00, 3.99),
        (6, 'High Performer',     'Medium Potential, High Performance. High delivery, specialized impact.',    '#1ABC9C', 4.00, 5.00, 3.00, 3.99),
        (7, 'Risk',               'Low Potential, Low Performance. Critical action required or exit path.',    '#E74C3C', 1.00, 2.99, 1.00, 2.99),
        (8, 'Effective',          'Low Potential, Medium Performance. Reliable in current role, low mobility.','#95A5A6', 3.00, 3.99, 1.00, 2.99),
        (9, 'Solid Professional', 'Low Potential, High Performance. Subject matter expert, domain anchor.',     '#34495E', 4.00, 5.00, 1.00, 2.99);
END
GO

-- 2. Populate Review Cycles
IF NOT EXISTS (SELECT 1 FROM dbo.ReviewCycles)
BEGIN
    INSERT INTO dbo.ReviewCycles (CycleName, Year, Quarter, IsActive, StartDate, EndDate)
    VALUES
        ('FY2025 Annual Talent Calibration', 2025, 'Q4', 0, '2025-10-01', '2025-12-15'),
        ('FY2026 Mid-Year Talent Review',    2026, 'Q2', 1, '2026-04-01', '2026-06-30');
END
GO

-- 3. Populate Sample Employees
IF NOT EXISTS (SELECT 1 FROM dbo.Employees)
BEGIN
    INSERT INTO dbo.Employees (EmployeeCode, FullName, Email, Department, JobTitle, TenureYears, IsKeyRole, CompaRatio, IsActive)
    VALUES
        ('EMP-001', 'Sophia Martinez',  'sophia.martinez@company.internal',  'Engineering', 'Principal Architect', 6, 1, 1.15, 1),
        ('EMP-002', 'Marcus Vance',     'marcus.vance@company.internal',     'Engineering', 'Senior Staff Engineer', 4, 1, 1.08, 1),
        ('EMP-003', 'Elena Rostova',    'elena.rostova@company.internal',    'Product',     'Director of Product',  5, 1, 1.12, 1),
        ('EMP-004', 'David Kim',        'david.kim@company.internal',        'Product',     'Product Manager II',   2, 0, 0.98, 1),
        ('EMP-005', 'Amara Okafor',     'amara.okafor@company.internal',     'Design',      'Lead UX Researcher',   3, 0, 1.02, 1),
        ('EMP-006', 'Liam O''Connor',   'liam.oconnor@company.internal',     'Operations',  'Site Reliability Lead', 5, 1, 1.05, 1),
        ('EMP-007', 'Chloe Bennett',    'chloe.bennett@company.internal',    'Marketing',   'Growth Marketing Mgr', 1, 0, 0.95, 1),
        ('EMP-008', 'Rajesh Patel',     'rajesh.patel@company.internal',     'Engineering', 'Backend Engineer II',  2, 0, 0.96, 1),
        ('EMP-009', 'Hannah Schmidt',   'hannah.schmidt@company.internal',   'Sales',       'Enterprise Account Dir', 4, 1, 1.10, 1);
END
GO

-- 4. Baseline Assessments (Covering diverse quadrants)
IF NOT EXISTS (SELECT 1 FROM dbo.Assessments)
BEGIN
    DECLARE @CycleId INT = (SELECT TOP 1 CycleId FROM dbo.ReviewCycles WHERE IsActive = 1);

    INSERT INTO dbo.Assessments (EmployeeId, ReviewCycleId, PerformanceScore, PotentialScore, AssignedBlockNumber, AssignedQuadrantName, EvaluatorNotes)
    SELECT e.EmployeeId, @CycleId, 
           scores.Perf, scores.Pot, scores.Block, scores.QName, scores.Notes
    FROM dbo.Employees e
    CROSS APPLY (
        SELECT CASE e.EmployeeCode
            WHEN 'EMP-001' THEN 4.80 WHEN 'EMP-002' THEN 4.20 WHEN 'EMP-003' THEN 3.60
            WHEN 'EMP-004' THEN 2.40 WHEN 'EMP-005' THEN 3.40 WHEN 'EMP-006' THEN 4.50
            WHEN 'EMP-007' THEN 1.80 WHEN 'EMP-008' THEN 3.20 WHEN 'EMP-009' THEN 4.10
        END AS Perf,
        CASE e.EmployeeCode
            WHEN 'EMP-001' THEN 4.90 WHEN 'EMP-002' THEN 3.50 WHEN 'EMP-003' THEN 4.50
            WHEN 'EMP-004' THEN 4.20 WHEN 'EMP-005' THEN 3.40 WHEN 'EMP-006' THEN 2.20
            WHEN 'EMP-007' THEN 1.90 WHEN 'EMP-008' THEN 2.10 WHEN 'EMP-009' THEN 4.60
        END AS Pot,
        CASE e.EmployeeCode
            WHEN 'EMP-001' THEN 3 WHEN 'EMP-002' THEN 6 WHEN 'EMP-003' THEN 2
            WHEN 'EMP-004' THEN 1 WHEN 'EMP-005' THEN 5 WHEN 'EMP-006' THEN 9
            WHEN 'EMP-007' THEN 7 WHEN 'EMP-008' THEN 8 WHEN 'EMP-009' THEN 3
        END AS Block,
        CASE e.EmployeeCode
            WHEN 'EMP-001' THEN 'Star'               WHEN 'EMP-002' THEN 'High Performer'
            WHEN 'EMP-003' THEN 'Growth Potential'   WHEN 'EMP-004' THEN 'Enigma'
            WHEN 'EMP-005' THEN 'Core Player'        WHEN 'EMP-006' THEN 'Solid Professional'
            WHEN 'EMP-007' THEN 'Risk'               WHEN 'EMP-008' THEN 'Effective'
            WHEN 'EMP-009' THEN 'Star'
        END AS QName,
        'Talent Calibration FY26 review baseline assessment' AS Notes
    ) scores;
END
GO
