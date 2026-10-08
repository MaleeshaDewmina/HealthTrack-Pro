using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

using HealthTrackPro.Web.Areas.Admin.Models;
using HealthTrackPro.Web.Models;
using HealthTrackPro.Web.Services.Implementations;
using HealthTrackPro.Web.Services.Interfaces;

using Microsoft.AspNet.Identity;

namespace HealthTrackPro.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AssignmentsController : Controller
    {
        private readonly ApplicationDbContext db =
            new ApplicationDbContext();

        private readonly ITrainerService trainerService =
            new TrainerService();

        private readonly IAuditService auditService =
            new AuditService();

        public async Task<ActionResult> Index()
        {
            var clients =
                await db.ClientProfiles
                    .AsNoTracking()
                    .OrderBy(c => c.FullName)
                    .ToListAsync();

            var trainers =
                await db.TrainerProfiles
                    .AsNoTracking()
                    .Where(t => t.IsApproved)
                    .OrderBy(t => t.FullName)
                    .ToListAsync();

            var trainerUsers =
                await db.Users
                    .AsNoTracking()
                    .ToListAsync();

            var activeTrainerIds =
                trainerUsers
                    .Where(u =>
                        !u.LockoutEndDateUtc.HasValue ||
                        u.LockoutEndDateUtc.Value <=
                            DateTime.UtcNow)
                    .Select(u => u.Id)
                    .ToList();

            trainers =
                trainers
                    .Where(t =>
                        activeTrainerIds.Contains(
                            t.UserId))
                    .ToList();

            var assignments =
                await db.TrainerClients
                    .AsNoTracking()
                    .Where(tc => tc.IsActive)
                    .ToListAsync();

            var trainerNames =
                trainers.ToDictionary(
                    t => t.UserId,
                    t => t.FullName);

            var model =
                new AdminAssignmentsIndexViewModel();

            foreach (var trainer in trainers)
            {
                model.Trainers.Add(
                    new AdminTrainerOptionViewModel
                    {
                        UserId =
                            trainer.UserId,

                        FullName =
                            trainer.FullName
                    });
            }

            foreach (var client in clients)
            {
                var assignment =
                    assignments.FirstOrDefault(
                        a =>
                            a.ClientUserId ==
                                client.UserId);

                string trainerId = null;
                string trainerName = null;

                if (assignment != null)
                {
                    trainerId =
                        assignment.TrainerUserId;

                    if (trainerNames.ContainsKey(
                        trainerId))
                    {
                        trainerName =
                            trainerNames[trainerId];
                    }
                    else
                    {
                        trainerName =
                            "Unavailable Trainer";
                    }
                }

                model.Clients.Add(
                    new AdminClientAssignmentViewModel
                    {
                        ClientUserId =
                            client.UserId,

                        ClientName =
                            client.FullName,

                        CurrentTrainerUserId =
                            trainerId,

                        CurrentTrainerName =
                            trainerName
                    });
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Assign(
            string clientUserId,
            string trainerUserId)
        {
            if (string.IsNullOrWhiteSpace(
                    clientUserId) ||
                string.IsNullOrWhiteSpace(
                    trainerUserId))
            {
                TempData["ErrorMessage"] =
                    "Please select a valid Trainer.";

                return RedirectToAction("Index");
            }

            var trainerAccount =
                await db.Users
                    .FirstOrDefaultAsync(u =>
                        u.Id == trainerUserId);

            if (trainerAccount == null)
            {
                TempData["ErrorMessage"] =
                    "Trainer account was not found.";

                return RedirectToAction("Index");
            }

            bool trainerSuspended =
                trainerAccount.LockoutEndDateUtc
                    .HasValue &&
                trainerAccount.LockoutEndDateUtc.Value >
                    DateTime.UtcNow;

            if (trainerSuspended)
            {
                TempData["ErrorMessage"] =
                    "A suspended Trainer cannot be assigned.";

                return RedirectToAction("Index");
            }

            var client =
                await db.ClientProfiles
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c =>
                        c.UserId == clientUserId);

            var trainer =
                await db.TrainerProfiles
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t =>
                        t.UserId ==
                            trainerUserId);

            if (client == null ||
                trainer == null)
            {
                TempData["ErrorMessage"] =
                    "Client or Trainer could not be found.";

                return RedirectToAction("Index");
            }

            var success =
                await trainerService
                    .AssignClientAsync(
                        trainerUserId,
                        clientUserId);

            if (!success)
            {
                TempData["ErrorMessage"] =
                    "The Client could not be assigned.";

                return RedirectToAction("Index");
            }

            await auditService.LogAsync(
                User.Identity.GetUserId(),
                "Assign Trainer",
                "TrainerClient",
                clientUserId,
                "Assigned " +
                    client.FullName +
                    " to Trainer " +
                    trainer.FullName + ".",
                Request.UserHostAddress);

            TempData["SuccessMessage"] =
                client.FullName +
                " is now assigned to " +
                trainer.FullName + ".";

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Unassign(
            string clientUserId,
            string trainerUserId)
        {
            if (string.IsNullOrWhiteSpace(
                    clientUserId) ||
                string.IsNullOrWhiteSpace(
                    trainerUserId))
            {
                TempData["ErrorMessage"] =
                    "No active Trainer assignment was found.";

                return RedirectToAction("Index");
            }

            var client =
                await db.ClientProfiles
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c =>
                        c.UserId ==
                            clientUserId);

            var trainer =
                await db.TrainerProfiles
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t =>
                        t.UserId ==
                            trainerUserId);

            var success =
                await trainerService
                    .UnassignClientAsync(
                        trainerUserId,
                        clientUserId);

            if (!success)
            {
                TempData["ErrorMessage"] =
                    "The Trainer assignment could not be removed.";

                return RedirectToAction("Index");
            }

            var clientName =
                client != null
                    ? client.FullName
                    : "Client";

            var trainerName =
                trainer != null
                    ? trainer.FullName
                    : "Trainer";

            await auditService.LogAsync(
                User.Identity.GetUserId(),
                "Unassign Trainer",
                "TrainerClient",
                clientUserId,
                "Removed Trainer " +
                    trainerName +
                    " from " +
                    clientName + ".",
                Request.UserHostAddress);

            TempData["SuccessMessage"] =
                "Trainer assignment removed successfully.";

            return RedirectToAction("Index");
        }

        protected override void Dispose(
            bool disposing)
        {
            if (disposing)
            {
                db.Dispose();

                var trainerDisposable =
                    trainerService as IDisposable;

                if (trainerDisposable != null)
                {
                    trainerDisposable.Dispose();
                }

                var auditDisposable =
                    auditService as IDisposable;

                if (auditDisposable != null)
                {
                    auditDisposable.Dispose();
                }
            }

            base.Dispose(disposing);
        }
    }
}
