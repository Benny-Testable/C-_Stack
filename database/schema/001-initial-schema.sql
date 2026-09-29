CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

CREATE TABLE members (
    "Id" uuid NOT NULL,
    "Email" character varying(256) NOT NULL,
    "PasswordHash" character varying(500) NOT NULL,
    "FullName" character varying(200) NOT NULL,
    "Role" integer NOT NULL,
    "Status" integer NOT NULL,
    "HasMedicalClearance" boolean NOT NULL,
    "CreatedAtUtc" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_members" PRIMARY KEY ("Id")
);

CREATE TABLE membership_plans (
    "Id" uuid NOT NULL,
    "Name" character varying(120) NOT NULL,
    "MonthlyPrice" numeric(8,2) NOT NULL,
    "DurationDays" integer NOT NULL,
    "MaxClassesPerWeek" integer NOT NULL,
    "IsActive" boolean NOT NULL,
    CONSTRAINT "PK_membership_plans" PRIMARY KEY ("Id")
);

CREATE TABLE trainers (
    "Id" uuid NOT NULL,
    "FullName" character varying(200) NOT NULL,
    "Specialty" character varying(120) NOT NULL,
    "IsActive" boolean NOT NULL,
    CONSTRAINT "PK_trainers" PRIMARY KEY ("Id")
);

CREATE TABLE fitness_assessments (
    "Id" uuid NOT NULL,
    "MemberId" uuid NOT NULL,
    "AssessedOn" date NOT NULL,
    "RestingHeartRate" integer NOT NULL,
    "BodyMassIndex" numeric(5,2) NOT NULL,
    "Notes" character varying(1000) NOT NULL,
    CONSTRAINT "PK_fitness_assessments" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_fitness_assessments_members_MemberId" FOREIGN KEY ("MemberId") REFERENCES members ("Id") ON DELETE CASCADE
);

CREATE TABLE workout_plans (
    "Id" uuid NOT NULL,
    "MemberId" uuid NOT NULL,
    "Title" character varying(160) NOT NULL,
    "SessionsPerWeek" integer NOT NULL,
    "CreatedOn" date NOT NULL,
    CONSTRAINT "PK_workout_plans" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_workout_plans_members_MemberId" FOREIGN KEY ("MemberId") REFERENCES members ("Id") ON DELETE CASCADE
);

CREATE TABLE subscriptions (
    "Id" uuid NOT NULL,
    "MemberId" uuid NOT NULL,
    "PlanId" uuid NOT NULL,
    "StartsOn" date NOT NULL,
    "EndsOn" date NOT NULL,
    "Status" integer NOT NULL,
    "AutoRenew" boolean NOT NULL,
    CONSTRAINT "PK_subscriptions" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_subscriptions_members_MemberId" FOREIGN KEY ("MemberId") REFERENCES members ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_subscriptions_membership_plans_PlanId" FOREIGN KEY ("PlanId") REFERENCES membership_plans ("Id") ON DELETE RESTRICT
);

CREATE TABLE class_sessions (
    "Id" uuid NOT NULL,
    "TrainerId" uuid NOT NULL,
    "Title" character varying(160) NOT NULL,
    "StartsAtUtc" timestamp with time zone NOT NULL,
    "DurationMinutes" integer NOT NULL,
    "Capacity" integer NOT NULL,
    "RequiresMedicalClearance" boolean NOT NULL,
    CONSTRAINT "PK_class_sessions" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_class_sessions_trainers_TrainerId" FOREIGN KEY ("TrainerId") REFERENCES trainers ("Id") ON DELETE RESTRICT
);

CREATE TABLE payments (
    "Id" uuid NOT NULL,
    "SubscriptionId" uuid NOT NULL,
    "Amount" numeric(8,2) NOT NULL,
    "Status" integer NOT NULL,
    "ProcessedAtUtc" timestamp with time zone NOT NULL,
    "ProviderReference" character varying(80) NOT NULL,
    CONSTRAINT "PK_payments" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_payments_subscriptions_SubscriptionId" FOREIGN KEY ("SubscriptionId") REFERENCES subscriptions ("Id") ON DELETE CASCADE
);

CREATE TABLE bookings (
    "Id" uuid NOT NULL,
    "MemberId" uuid NOT NULL,
    "ClassSessionId" uuid NOT NULL,
    "Status" integer NOT NULL,
    "BookedAtUtc" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_bookings" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_bookings_class_sessions_ClassSessionId" FOREIGN KEY ("ClassSessionId") REFERENCES class_sessions ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_bookings_members_MemberId" FOREIGN KEY ("MemberId") REFERENCES members ("Id") ON DELETE CASCADE
);

CREATE TABLE attendance_records (
    "Id" uuid NOT NULL,
    "BookingId" uuid NOT NULL,
    "CheckedInAtUtc" timestamp with time zone NOT NULL,
    "Outcome" integer NOT NULL,
    CONSTRAINT "PK_attendance_records" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_attendance_records_bookings_BookingId" FOREIGN KEY ("BookingId") REFERENCES bookings ("Id") ON DELETE CASCADE
);

INSERT INTO membership_plans ("Id", "DurationDays", "IsActive", "MaxClassesPerWeek", "MonthlyPrice", "Name")
VALUES ('11111111-1111-1111-1111-111111111111', 30, TRUE, 3, 49.0, 'Desert Dawn');
INSERT INTO membership_plans ("Id", "DurationDays", "IsActive", "MaxClassesPerWeek", "MonthlyPrice", "Name")
VALUES ('22222222-2222-2222-2222-222222222222', 30, TRUE, 5, 79.0, 'Canyon Peak');
INSERT INTO membership_plans ("Id", "DurationDays", "IsActive", "MaxClassesPerWeek", "MonthlyPrice", "Name")
VALUES ('33333333-3333-3333-3333-333333333333', 30, TRUE, 14, 119.0, 'Saguaro Unlimited');

INSERT INTO trainers ("Id", "FullName", "IsActive", "Specialty")
VALUES ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1', 'Maya Chen', TRUE, 'Strength');
INSERT INTO trainers ("Id", "FullName", "IsActive", "Specialty")
VALUES ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2', 'Luis Ortega', TRUE, 'Conditioning');

INSERT INTO class_sessions ("Id", "Capacity", "DurationMinutes", "RequiresMedicalClearance", "StartsAtUtc", "Title", "TrainerId")
VALUES ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb1', 16, 50, FALSE, TIMESTAMPTZ '2027-03-02T15:00:00Z', 'Sunrise strength', 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1');

CREATE UNIQUE INDEX "IX_attendance_records_BookingId" ON attendance_records ("BookingId");

CREATE INDEX "IX_bookings_ClassSessionId" ON bookings ("ClassSessionId");

CREATE UNIQUE INDEX "IX_bookings_MemberId_ClassSessionId" ON bookings ("MemberId", "ClassSessionId") WHERE "Status" = 0;

CREATE INDEX "IX_class_sessions_TrainerId" ON class_sessions ("TrainerId");

CREATE INDEX "IX_fitness_assessments_MemberId" ON fitness_assessments ("MemberId");

CREATE UNIQUE INDEX "IX_members_Email" ON members ("Email");

CREATE INDEX "IX_payments_SubscriptionId" ON payments ("SubscriptionId");

CREATE INDEX "IX_subscriptions_MemberId" ON subscriptions ("MemberId");

CREATE INDEX "IX_subscriptions_PlanId" ON subscriptions ("PlanId");

CREATE INDEX "IX_workout_plans_MemberId" ON workout_plans ("MemberId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260929065326_InitialClub', '8.0.11');

COMMIT;

