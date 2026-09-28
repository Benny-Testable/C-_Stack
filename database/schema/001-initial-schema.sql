IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928072642_InitialSchema'
)
BEGIN
    CREATE TABLE [Applicants] (
        [Id] int NOT NULL IDENTITY,
        [FullName] nvarchar(200) NOT NULL,
        [Email] nvarchar(256) NOT NULL,
        [InstitutionName] nvarchar(200) NULL,
        [CreatedAtUtc] datetimeoffset NOT NULL,
        [UpdatedAtUtc] datetimeoffset NULL,
        CONSTRAINT [PK_Applicants] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928072642_InitialSchema'
)
BEGIN
    CREATE TABLE [Scholarships] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(200) NOT NULL,
        [Description] nvarchar(2000) NULL,
        [SponsorName] nvarchar(200) NOT NULL,
        [AwardAmount] decimal(18,2) NOT NULL,
        [TotalSlots] int NOT NULL,
        [ApplicationOpensOn] date NOT NULL,
        [ApplicationClosesOn] date NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAtUtc] datetimeoffset NOT NULL,
        [UpdatedAtUtc] datetimeoffset NULL,
        CONSTRAINT [PK_Scholarships] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928072642_InitialSchema'
)
BEGIN
    CREATE TABLE [UserAccounts] (
        [Id] int NOT NULL IDENTITY,
        [Email] nvarchar(256) NOT NULL,
        [PasswordHash] nvarchar(512) NOT NULL,
        [Role] int NOT NULL,
        [ApplicantId] int NULL,
        [IsActive] bit NOT NULL,
        [CreatedAtUtc] datetimeoffset NOT NULL,
        CONSTRAINT [PK_UserAccounts] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_UserAccounts_Applicants_ApplicantId] FOREIGN KEY ([ApplicantId]) REFERENCES [Applicants] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928072642_InitialSchema'
)
BEGIN
    CREATE TABLE [ScholarshipApplications] (
        [Id] int NOT NULL IDENTITY,
        [ScholarshipId] int NOT NULL,
        [ApplicantId] int NOT NULL,
        [Status] int NOT NULL,
        [Motivation] nvarchar(2000) NULL,
        [ReviewerNotes] nvarchar(2000) NULL,
        [SubmittedAtUtc] datetimeoffset NULL,
        [DecidedAtUtc] datetimeoffset NULL,
        [CreatedAtUtc] datetimeoffset NOT NULL,
        [UpdatedAtUtc] datetimeoffset NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_ScholarshipApplications] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ScholarshipApplications_Applicants_ApplicantId] FOREIGN KEY ([ApplicantId]) REFERENCES [Applicants] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_ScholarshipApplications_Scholarships_ScholarshipId] FOREIGN KEY ([ScholarshipId]) REFERENCES [Scholarships] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928072642_InitialSchema'
)
BEGIN
    CREATE UNIQUE INDEX [UX_Applicants_Email] ON [Applicants] ([Email]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928072642_InitialSchema'
)
BEGIN
    CREATE INDEX [IX_ScholarshipApplications_ApplicantId] ON [ScholarshipApplications] ([ApplicantId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928072642_InitialSchema'
)
BEGIN
    CREATE INDEX [IX_ScholarshipApplications_Status] ON [ScholarshipApplications] ([Status]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928072642_InitialSchema'
)
BEGIN
    CREATE UNIQUE INDEX [UX_ScholarshipApplications_Scholarship_Applicant] ON [ScholarshipApplications] ([ScholarshipId], [ApplicantId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928072642_InitialSchema'
)
BEGIN
    CREATE INDEX [IX_Scholarships_IsActive_ApplicationClosesOn] ON [Scholarships] ([IsActive], [ApplicationClosesOn]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928072642_InitialSchema'
)
BEGIN
    CREATE UNIQUE INDEX [UX_Scholarships_Name] ON [Scholarships] ([Name]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928072642_InitialSchema'
)
BEGIN
    CREATE INDEX [IX_UserAccounts_ApplicantId] ON [UserAccounts] ([ApplicantId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928072642_InitialSchema'
)
BEGIN
    CREATE UNIQUE INDEX [UX_UserAccounts_Email] ON [UserAccounts] ([Email]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928072642_InitialSchema'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260928072642_InitialSchema', N'8.0.31');
END;
GO

COMMIT;
GO

