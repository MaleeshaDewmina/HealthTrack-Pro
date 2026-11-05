# HealthTrack Pro

An ASP.NET MVC fitness platform for workout planning, nutrition tracking and collaboration between clients and trainers.

Clients can track goals and measurements, trainers can organise plans, and administrators manage access. The web application combines ASP.NET Identity, Entity Framework, SQL Server and a Groq-powered AI Coach.

**Technologies**  
C# · ASP.NET MVC 5 · Entity Framework · SQL Server · Groq

![Client Dashboard](https://github.com/user-attachments/assets/b3640383-7b4b-418a-8fa5-fbd9d54f1e7b)

[Progress Analytics](https://github.com/user-attachments/assets/573b6688-5494-47b5-aacc-e6279095428f) · [AI Fitness Coach](https://github.com/user-attachments/assets/b15edc81-6ec0-4e40-a162-8c4170b2daaa)

## Run locally

On Windows, open `HealthTrackPro.sln` in Visual Studio with the web development workload and .NET Framework 4.8. Restore NuGet packages, set `HealthTrackPro.Web` as the startup project and configure `DefaultConnection` in its `Web.config`. In Package Manager Console select `HealthTrackPro.Web` and run `Update-Database`. Set `GROQ_API_KEY` locally for AI Coach, then start with IIS Express.

## Contributors

Maleesha Dewmina Perera led the project and worked on backend integration, authentication and administration. Chathura Denuwan developed the client workflows and AI Coach experience. Dinethi Ruvisha developed the trainer workflows.
