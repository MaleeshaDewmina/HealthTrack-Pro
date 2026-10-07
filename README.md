# HealthTrack Pro

HealthTrack Pro is a Personal Health & Fitness Management System developed for the CS6004ES Application Development group coursework.

## Team Members

- Maleesha Dewmina
- Chathura Denuwan
- Dinethi Ruvisha

## Technologies

- ASP.NET MVC 5
- C#
- .NET Framework 4.8
- Entity Framework 6
- ASP.NET Identity
- SQL Server / LocalDB
- Bootstrap
- JavaScript
- Git / GitHub

## User Roles

- Client
- Trainer
- Administrator

## Current Project Structure

The system uses separate MVC Areas for:

- Admin
- Client
- Trainer

Shared functionality is maintained outside the Areas.

## Branch Strategy

- `main` - stable working version
- `develop` - integrated development version
- `feature/*` - individual feature development

## Team Development Rules

1. Do not develop directly on `main`.
2. Do not develop directly on `develop`.
3. Create a feature branch from `develop`.
4. Commit small completed changes.
5. Push the feature branch.
6. Create a Pull Request into `develop`.
7. Test before merging.

## Initial Team Responsibilities

### Maleesha Dewmina
- Database architecture
- Authentication and roles
- Administrator module
- Security
- Shared services
- Entity Framework migrations
- Final integration

### Chathura Denuwan
- Client dashboard
- Client profile
- Fitness goals
- Workout management
- Diet management
- Health tracking
- Progress analytics

### Dinethi Ruvisha
- Trainer dashboard
- Assigned clients
- Appointment management
- Trainer availability
- Notifications
- Trainer feedback
- Client-trainer communication

## Important Shared Rules

Do not create duplicate versions of:

- `ApplicationUser`
- `ApplicationDbContext`
- `ClientProfile`

The project uses one shared database and one shared ASP.NET Identity system.

## Running the Project

1. Clone the repository.
2. Open `HealthTrackPro.sln`.
3. Restore NuGet packages if required.
4. Build the solution.
5. Run using IIS Express.

## Coursework

Module: CS6004ES - Application Development

System: HealthTrack - Personal Health & Fitness Management System
