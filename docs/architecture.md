# Architecture

```text
React + Vite + TypeScript
        |  JSON / HTTPS
ASP.NET Core 8 API
        |  application services
Domain rules (no infrastructure references)
        |
Infrastructure: EF Core 8, Npgsql, PostgreSQL
```

| Layer | Project |
| --- | --- |
| Domain | `WestCoastFitness.Domain` — entities, enums, `ClubRuleException` |
| Application | `WestCoastFitness.Application` — booking, cancellation, renewal, registration, workout advice |
| Infrastructure | `WestCoastFitness.Infrastructure` — `FitnessDbContext`, repositories, password hasher, mock payments |
| API | `WestCoastFitness.Api` — controllers, JWT, security headers |
| UI | `frontend/src` |

Business decisions that tools measure live in the application project, not in controllers. `ClassBookingRules.Evaluate` is the high-branch method (account state, subscription window, capacity, weekly cap, overlap, trainer, medical clearance). `MembershipWindowText` and `StaffMembershipWindowText` are intentionally parallel descriptions so duplication scanners have a real clone. Registration, cancellation, renewal, and workout advice are smaller branch sets.

PostgreSQL table names are explicit (`members`, `membership_plans`, `class_sessions`, and the rest). The unique reservation index allows only one `Reserved` booking per member and class.
