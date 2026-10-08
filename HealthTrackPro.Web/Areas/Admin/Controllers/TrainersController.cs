using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using HealthTrackPro.Web.Areas.Admin.Models;
using HealthTrackPro.Web.Models;
using HealthTrackPro.Web.Models.Profiles;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Host.SystemWeb;

namespace HealthTrackPro.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class TrainersController : Controller
    {
        private readonly ApplicationDbContext db =
            new ApplicationDbContext();
        private ApplicationUserManager UserManager
        {
            get
            {
                return HttpContext
                    .GetOwinContext()
                    .GetUserManager<ApplicationUserManager>();
            }
        }

        public async Task<ActionResult> Index()
        {
            var trainers =
                await db.TrainerProfiles
                    .Include(t => t.ApplicationUser)
                    .OrderBy(t => t.FullName)
                    .Select(t =>
                        new TrainerListItemViewModel
                        {
                            UserId = t.UserId,
                            FullName = t.FullName,
                            Email = t.ApplicationUser.Email,
                            Specialization = t.Specialization,
                            YearsOfExperience =
                                t.YearsOfExperience,
                            IsApproved = t.IsApproved
                        })
                    .ToListAsync();

            return View(trainers);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View(
                new CreateTrainerViewModel()
            );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(
            CreateTrainerViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existingUser =
                await UserManager.FindByEmailAsync(
                    model.Email
                );

            if (existingUser != null)
            {
                ModelState.AddModelError(
                    "Email",
                    "An account already exists with this email."
                );

                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email
            };

            var createResult =
                await UserManager.CreateAsync(
                    user,
                    model.Password
                );

            if (!createResult.Succeeded)
            {
                foreach (var error in createResult.Errors)
                {
                    ModelState.AddModelError(
                        "",
                        error
                    );
                }

                return View(model);
            }

            var roleResult =
                await UserManager.AddToRoleAsync(
                    user.Id,
                    "Trainer"
                );

            if (!roleResult.Succeeded)
            {
                await UserManager.DeleteAsync(user);

                foreach (var error in roleResult.Errors)
                {
                    ModelState.AddModelError(
                        "",
                        error
                    );
                }

                return View(model);
            }

            var trainerProfile =
                new TrainerProfile
                {
                    UserId = user.Id,
                    FullName = model.FullName,
                    Specialization =
                        model.Specialization,
                    Bio = model.Bio,
                    YearsOfExperience =
                        model.YearsOfExperience,
                    Qualifications =
                        model.Qualifications,

                    // Account is created directly
                    // by an Administrator.
                    IsApproved = true,

                    CreatedAt = DateTime.UtcNow
                };

            db.TrainerProfiles.Add(
                trainerProfile
            );

            try
            {
                await db.SaveChangesAsync();
            }
            catch
            {
                await UserManager.RemoveFromRoleAsync(
                    user.Id,
                    "Trainer"
                );

                await UserManager.DeleteAsync(user);

                ModelState.AddModelError(
                    "",
                    "Trainer profile could not be created."
                );

                return View(model);
            }

            TempData["SuccessMessage"] =
                "Trainer account created successfully.";

            return RedirectToAction("Index");
        }

        protected override void Dispose(
            bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}




