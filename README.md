# Job Marketplace API

A backend API for a job marketplace — employers post jobs, candidates apply, employers review and decide. Built as a portfolio project to practice .NET patterns I wanted to get comfortable with.

## Tech stack

- **.NET 10** — Minimal APIs, no controllers
- **MediatR** — CQRS, every request goes through a command or query handler
- **FluentValidation** — request validation as a MediatR pipeline behaviour
- **EFCore 10 + Npgsql** — PostgreSQL via Supabase
- **Supabase Auth** — JWT-based auth, the backend just validates the token
- **xUnit + FluentAssertions** — domain unit tests

## How it's structured

Three bounded contexts, each with its own Domain / Application / Infrastructure stack:

- **Identity** — maps a Supabase user to a local profile with a role (Employer or Candidate)
- **Jobs** — employers create and manage job listings
- **Applications** — candidates apply to published jobs, employers move them through a workflow

They share a `SharedKernel` for base types (Entity, ValueObject, Result pattern, typed IDs).

Each bounded context gets its own EFCore DbContext and its own schema in the database (`identity`, `jobs`, `applications`). They don't share tables or navigate across each other's data — if Applications needs to check something in the Jobs schema, it reads a raw column via a read model, not a domain object.

## Patterns used

**DDD** — aggregates own their invariants. `Job.Publish()` checks ownership and current status itself; the handler just calls it and saves.

**CQRS** — commands mutate state through the domain model, queries bypass it and project directly from the database. No loading an aggregate just to read a list.

**Outbox pattern** — domain events are written to an `outbox_messages` table in the same transaction as the state change. A background service picks them up and publishes via MediatR. Events don't get lost if something crashes mid-request.

**Result pattern** — no exceptions for business failures. Handlers return `Result` or `Result<T>`, endpoints map that to the right HTTP status code.

## Auth

Supabase handles sign-up and login. The API validates the JWT on every request, looks up the local `UserProfile` by the `sub` claim, then injects an `app_role` claim (`Employer` or `Candidate`) into the principal. Endpoint authorization uses standard ASP.NET Core policies against that claim.

First-time users call `POST /api/identity/profile` to pick their role — that's the only endpoint that skips the profile lookup.

## Running locally

Fill in `appsettings.json` with your Supabase connection string and JWT secret, run the EFCore migrations, then:

```bash
dotnet run --project src/JobMarketplace.Api
```

See `docs/plans/` for the step-by-step build guide.
