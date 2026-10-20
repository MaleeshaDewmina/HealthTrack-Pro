using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

using HealthTrackPro.Web.Areas.Trainer.Models;
using HealthTrackPro.Web.Models.Profiles;
using HealthTrackPro.Web.Services.Implementations;
using HealthTrackPro.Web.Services.Interfaces;

namespace HealthTrackPro.Web.Areas.Trainer.Controllers
{
    /// <summary>Trainer-only pages for the Clients assigned to the logged-in trainer.</summary>
    [Authorize(Roles = "Trainer")]
    public class ClientsController : Controller
    {
        private readonly ITrainerService _trainerService;
        private readonly IProgressService _progressService;

        public ClientsController()
        {
            _trainerService = new TrainerService();
            _progressService = new ProgressService();
        }

        /// <summary>Lists only the clients assigned to the signed-in trainer.</summary>
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            // The trainer comes from the signed-in identity, never from the URL or form.
            string trainerUserId = User.Identity.GetUserId();

            List<ClientProfile> clients =
                await _trainerService.GetAssignedClientsAsync(trainerUserId);

            var model = new TrainerClientListViewModel();

            foreach (ClientProfile client in clients)
            {
                decimal? bmi =
                    await _progressService.GetCurrentBmiAsync(client.UserId);

                model.Clients.Add(
                    TrainerClientListItemViewModel.FromProfile(client, bmi));
            }

            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                (_trainerService as IDisposable)?.Dispose();
                (_progressService as IDisposable)?.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
