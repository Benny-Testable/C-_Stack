/*
  Reference data: scholarship catalogue.

  Purpose
    The application is unusable until at least one scholarship exists, because an
    application row requires a ScholarshipId foreign key. This script therefore seeds
    catalogue rows only.

  Deliberately NOT seeded
    - Applicants and ScholarshipApplications. Those tables hold personal data
      (name, email, institution). Shipping sample people in source control would put
      PII in the repository, which the FERPA / GDPR data-minimisation metrics check for.
    - UserAccounts. Credentials are never committed. The administrator account is
      provisioned at first start from Bootstrap:AdministratorEmail and
      Bootstrap:AdministratorPassword, which are supplied through user secrets or
      environment variables. See docs/setup.md.

  Idempotency
    Guarded by NOT EXISTS on the unique Name key so the script can be re-run safely
    against an already seeded database.

  Run order
    database/schema/001-initial-schema.sql must be applied first.
*/

SET NOCOUNT ON;
GO

BEGIN TRANSACTION;
GO

INSERT INTO [Scholarships]
    ([Name], [Description], [SponsorName], [AwardAmount], [TotalSlots],
     [ApplicationOpensOn], [ApplicationClosesOn], [IsActive], [CreatedAtUtc], [UpdatedAtUtc])
SELECT
    seed.[Name], seed.[Description], seed.[SponsorName], seed.[AwardAmount], seed.[TotalSlots],
    seed.[ApplicationOpensOn], seed.[ApplicationClosesOn], seed.[IsActive], SYSUTCDATETIME(), NULL
FROM (VALUES
    (N'CMGroups Undergraduate Merit Award',
     N'Awarded to undergraduate students demonstrating sustained academic achievement.',
     N'CMGroups Foundation', CAST(5000.00 AS decimal(18,2)), 25,
     CAST('2026-01-01' AS date), CAST('2026-12-31' AS date), CAST(1 AS bit)),

    (N'CMGroups Engineering Access Grant',
     N'Supports students entering accredited engineering programmes.',
     N'CMGroups Foundation', CAST(7500.00 AS decimal(18,2)), 15,
     CAST('2026-02-01' AS date), CAST('2026-11-30' AS date), CAST(1 AS bit)),

    (N'CMGroups Postgraduate Research Bursary',
     N'Contributes toward research costs for postgraduate candidates.',
     N'CMGroups Research Office', CAST(12000.00 AS decimal(18,2)), 8,
     CAST('2026-03-01' AS date), CAST('2026-10-31' AS date), CAST(1 AS bit)),

    (N'CMGroups Community Service Scholarship',
     N'Recognises documented, sustained voluntary community contribution.',
     N'CMGroups Community Trust', CAST(3000.00 AS decimal(18,2)), 40,
     CAST('2026-01-15' AS date), CAST('2026-09-30' AS date), CAST(1 AS bit)),

    (N'CMGroups Legacy Award (Closed)',
     N'Retired programme retained for historical reporting only.',
     N'CMGroups Foundation', CAST(2000.00 AS decimal(18,2)), 10,
     CAST('2024-01-01' AS date), CAST('2024-06-30' AS date), CAST(0 AS bit))
) AS seed ([Name], [Description], [SponsorName], [AwardAmount], [TotalSlots],
           [ApplicationOpensOn], [ApplicationClosesOn], [IsActive])
WHERE NOT EXISTS (
    SELECT 1 FROM [Scholarships] AS existing WHERE existing.[Name] = seed.[Name]
);
GO

COMMIT;
GO
