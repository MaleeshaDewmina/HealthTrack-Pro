
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

using HealthTrackPro.Web.Areas.Client.Models;
using HealthTrackPro.Web.Services.Interfaces;
using HealthTrackPro.Web.Services.Implementations;

namespace HealthTrackPro.Web.Areas.Client.Controllers
{
    [Authorize(Roles = "Client")]
    public class DietController : Controller
    {
        private readonly IDietService _dietService;

        public DietController()
        {
            _dietService = new DietService();
        }

        // GET: /Client/Diet
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            string userId = User.Identity.GetUserId();

            var plans = await _dietService
                .GetClientPlansAsync(userId);

            var activePlan = await _dietService
                .GetActivePlanAsync(userId);

            var model = new ClientDietViewModel
            {
                Plans = plans,
                ActivePlan = activePlan
            };

            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                var disposable = _dietService as IDisposable;

                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }

            base.Dispose(disposing);
        }
    }
}
