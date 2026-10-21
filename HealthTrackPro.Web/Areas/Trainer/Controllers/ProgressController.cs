using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

using HealthTrackPro.Web.Areas.Trainer.Models;
using HealthTrackPro.Web.Models.Fitness;
using HealthTrackPro.Web.Models.Profiles;
using HealthTrackPro.Web.Services.Implementations;
using HealthTrackPro.Web.Services.Interfaces;

namespace HealthTrackPro.Web.Areas.Trainer.Controllers
{
    /// <summary>
    /// Read-only progress analytics for the logged-in trainer's assigned clients.
    /// </summary>
    [Authorize(Roles = "Trainer")]
    public class ProgressController : Controller
    {
        private readonly ITrainerService _trainerService;
        private readonly IProgressService _progressService;

        public ProgressController()
        {
            _trainerService = new TrainerService();
            _progressService = new ProgressService();
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            string trainerUserId = User.Identity.GetUserId();

            var clients =
                await _trainerService
                    .GetAssignedClientsAsync(trainerUserId);

            var model =
                new ProgressReportViewModel
                {
                    GeneratedAt = DateTime.Now
                };


            foreach (
                ClientProfile client
                in clients.OrderBy(c => c.FullName))
            {
                var goals =
                    await _progressService
                        .GetGoalsAsync(client.UserId);

                var metrics =
                    (
                        await _progressService
                            .GetHealthMetricsAsync(client.UserId)
                    ).ToList();

                var latest =
                    await _progressService
                        .GetLatestHealthMetricAsync(client.UserId);

                var bmi =
                    await _progressService
                        .GetCurrentBmiAsync(client.UserId);

                var active =
                    goals
                        .Where(g => !g.IsCompleted)
                        .ToList();


                decimal averageProgress = 0m;

                if (goals.Count > 0)
                {
                    averageProgress =
                        Math.Round(
                            goals.Average(
                                g =>
                                    g.IsCompleted
                                        ? 100m
                                        : Math.Max(
                                            0m,
                                            Math.Min(
                                                100m,
                                                g.CalculateProgress()
                                            )
                                        )
                            ),
                            0
                        );
                }


                var row =
                    new ProgressClientRowViewModel
                    {
                        ClientUserId =
                            client.UserId,

                        ClientName =
                            string.IsNullOrWhiteSpace(
                                client.FullName
                            )
                                ? "Unnamed client"
                                : client.FullName,

                        Goal =
                            client.PrimaryGoal.HasValue
                                ? SplitPascalCase(
                                    client.PrimaryGoal
                                        .Value
                                        .ToString()
                                )
                                : "Not set",

                        CurrentWeight =
                            latest != null &&
                            latest.WeightKg.HasValue
                                ? latest.WeightKg
                                    .Value
                                    .ToString("0.0") +
                                  " kg"
                                : "No weight recorded",

                        CurrentWeightValue =
                            latest != null
                                ? latest.WeightKg
                                : null,

                        LastRecorded =
                            latest != null
                                ? latest.RecordedAt
                                    .ToString(
                                        "MMM d, yyyy"
                                    )
                                : "Never",

                        Bmi =
                            bmi.HasValue
                                ? bmi.Value
                                    .ToString("0.0")
                                : "-",

                        BmiValue =
                            bmi,

                        ActiveGoals =
                            active.Count,

                        CompletedGoals =
                            goals.Count -
                            active.Count,

                        TotalGoals =
                            goals.Count,

                        AverageGoalProgress =
                            goals.Count == 0
                                ? "-"
                                : averageProgress
                                    .ToString("0") +
                                  "%",

                        AverageGoalProgressValue =
                            averageProgress,

                        MeasurementCount =
                            metrics.Count
                    };


                var trend =
                    metrics
                        .Where(
                            m =>
                                m.WeightKg.HasValue ||
                                m.CalculateBMI()
                                    .HasValue
                        )
                        .OrderByDescending(
                            m => m.RecordedAt
                        )
                        .ThenByDescending(
                            m => m.HealthMetricId
                        )
                        .Take(10)
                        .OrderBy(
                            m => m.RecordedAt
                        )
                        .ThenBy(
                            m => m.HealthMetricId
                        )
                        .ToList();


                foreach (var metric in trend)
                {
                    row.Trend.Add(
                        new ProgressMetricPointViewModel
                        {
                            Label =
                                metric.RecordedAt
                                    .ToString("MMM d h:mm tt"),

                            WeightKg =
                                metric.WeightKg,

                            Bmi =
                                metric.CalculateBMI(),

                            Steps =
                                metric.Steps
                        }
                    );
                }


                model.Clients.Add(row);
            }


            model.TotalActiveGoals =
                model.Clients.Sum(
                    c => c.ActiveGoals
                );

            model.TotalCompletedGoals =
                model.Clients.Sum(
                    c => c.CompletedGoals
                );

            model.ClientsWithMeasurements =
                model.Clients.Count(
                    c => c.MeasurementCount > 0
                );


            var clientsWithGoals =
                model.Clients
                    .Where(c => c.TotalGoals > 0)
                    .ToList();

            model.OverallAverageGoalProgress =
                clientsWithGoals.Count == 0
                    ? 0m
                    : Math.Round(
                        clientsWithGoals.Average(
                            c =>
                                c.AverageGoalProgressValue
                        ),
                        0
                    );


            return View(model);
        }


        private static string SplitPascalCase(
            string value)
        {
            return Regex.Replace(
                value,
                "([a-z])([A-Z])",
                "$1 $2"
            );
        }


        protected override void Dispose(
            bool disposing)
        {
            if (disposing)
            {
                (_trainerService as IDisposable)
                    ?.Dispose();

                (_progressService as IDisposable)
                    ?.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}