
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using HealthTrackPro.Web.Areas.Client.Models;
using HealthTrackPro.Web.Services.Interfaces;
using HealthTrackPro.Web.Services.Implementations;

namespace HealthTrackPro.Web.Areas.Client.Controllers
{
    [Authorize(Roles = "Client")]
    public class ProfileController : Controller
    {
        private readonly IClientService _clientService;
        private readonly IProgressService _progressService;

        private const int MaxPhotoBytes = 2 * 1024 * 1024;

        private static readonly string[] PhotoExtensions =
        {
            ".jpg", ".png", ".webp"
        };

        public ProfileController()
        {
            _clientService = new ClientService();
            _progressService = new ProgressService();
        }

        // GET: /Client/Profile
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            string userId = User.Identity.GetUserId();

            var profile = await _clientService
                .GetProfileAsync(userId);

            if (profile == null)
            {
                return HttpNotFound();
            }

            ViewBag.HasProfilePhoto =
                FindPhotoPath(userId) != null;

            var model = new ClientProfileViewModel
            {
                FullName = profile.FullName,
                DateOfBirth = profile.DateOfBirth,
                HeightCm = profile.HeightCm,
                CurrentWeightKg = profile.CurrentWeightKg,
                ActivityLevel = profile.ActivityLevel,
                PrimaryGoal = profile.PrimaryGoal
            };

            return View(model);
        }

        // POST: /Client/Profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(
            ClientProfileViewModel model)
        {
            string userId = User.Identity.GetUserId();

            ViewBag.HasProfilePhoto =
                FindPhotoPath(userId) != null;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.DateOfBirth.HasValue &&
                model.DateOfBirth.Value.Date > DateTime.Today)
            {
                ModelState.AddModelError(
                    "DateOfBirth",
                    "Date of birth cannot be in the future.");

                return View(model);
            }

            // Save existing profile data
            bool updated = await _clientService
                .UpdateProfileAsync(
                    userId,
                    model.FullName,
                    model.DateOfBirth,
                    model.HeightCm,
                    model.CurrentWeightKg,
                    model.ActivityLevel,
                    model.PrimaryGoal
                );

            if (!updated)
            {
                ModelState.AddModelError(
                    "",
                    "Unable to update your profile.");

                return View(model);
            }

            // Record measurements only if values changed
            if (model.HeightCm.HasValue &&
                model.CurrentWeightKg.HasValue)
            {
                var latest = await _progressService
                    .GetLatestHealthMetricAsync(userId);

                if (latest == null ||
                    latest.WeightKg != model.CurrentWeightKg ||
                    latest.HeightCm != model.HeightCm)
                {
                    var metric = await _progressService
                        .AddHealthMetricAsync(
                            userId,
                            model.CurrentWeightKg,
                            model.HeightCm,
                            null,
                            null,
                            null,
                            null
                        );

                    if (metric == null)
                    {
                        TempData["WarningMessage"] =
                            "Profile saved, but health measurement was not recorded.";
                    }
                }
            }

            TempData["SuccessMessage"] =
                "Profile updated successfully!";

            return RedirectToAction("Index");
        }

        // GET: /Client/Profile/Photo
        // Only logged-in clients can access this action.
        [HttpGet]
        public ActionResult Photo()
        {
            string userId = User.Identity.GetUserId();
            string path = FindPhotoPath(userId);

            if (path == null)
            {
                return HttpNotFound();
            }

            string extension = Path
                .GetExtension(path)
                .ToLowerInvariant();

            string mimeType =
                extension == ".png" ? "image/png" :
                extension == ".webp" ? "image/webp" :
                "image/jpeg";

            Response.Cache.SetCacheability(
                HttpCacheability.NoCache);

            Response.Cache.SetNoStore();

            return File(path, mimeType);
        }

        // POST: /Client/Profile/UploadPhoto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadPhoto(
            HttpPostedFileBase photo)
        {
            if (photo == null ||
                photo.ContentLength <= 0)
            {
                TempData["PhotoError"] =
                    "Please select an image to upload.";

                return RedirectToAction("Index");
            }

            if (photo.ContentLength > MaxPhotoBytes)
            {
                TempData["PhotoError"] =
                    "The image must be 2 MB or smaller.";

                return RedirectToAction("Index");
            }

            // Read the file with a size limit.
            byte[] imageBytes;

            using (var output = new MemoryStream())
            {
                var buffer = new byte[8192];
                int bytesRead;

                while ((bytesRead = photo.InputStream.Read(
                    buffer, 0, buffer.Length)) > 0)
                {
                    if (output.Length + bytesRead >
                        MaxPhotoBytes)
                    {
                        TempData["PhotoError"] =
                            "The image must be 2 MB or smaller.";

                        return RedirectToAction("Index");
                    }

                    output.Write(
                        buffer, 0, bytesRead);
                }

                imageBytes = output.ToArray();
            }

            // Check the file signature instead of
            // trusting its name or declared content type.
            string extension;

            if (!GetValidImageExtension(
                imageBytes, out extension))
            {
                TempData["PhotoError"] =
                    "Upload a valid JPG, PNG, or WebP image.";

                return RedirectToAction("Index");
            }

            string directory = PhotoDirectory;
            string temporaryPath = null;

            try
            {
                Directory.CreateDirectory(directory);

                string userId =
                    User.Identity.GetUserId();

                string prefix = PhotoPrefix(userId);

                string finalPath = Path.Combine(
                    directory,
                    prefix + extension
                );

                temporaryPath = Path.Combine(
                    directory,
                    Guid.NewGuid().ToString("N") + ".tmp"
                );

                System.IO.File.WriteAllBytes(
                    temporaryPath,
                    imageBytes
                );

                if (System.IO.File.Exists(finalPath))
                {
                    System.IO.File.Replace(
                        temporaryPath,
                        finalPath,
                        null
                    );
                }
                else
                {
                    System.IO.File.Move(
                        temporaryPath,
                        finalPath
                    );
                }

                temporaryPath = null;

                // Delete any old formats belonging
                // to this user.
                foreach (string otherExtension
                    in PhotoExtensions)
                {
                    string oldPath = Path.Combine(
                        directory,
                        prefix + otherExtension
                    );

                    if (!string.Equals(
                            oldPath,
                            finalPath,
                            StringComparison.OrdinalIgnoreCase)
                        && System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }

                TempData["PhotoSuccess"] =
                    "Profile photo updated successfully!";
            }
            catch (IOException)
            {
                TempData["PhotoError"] =
                    "Could not save the photo. Check folder permissions.";
            }
            catch (UnauthorizedAccessException)
            {
                TempData["PhotoError"] =
                    "The application cannot write to its photo folder.";
            }
            finally
            {
                if (temporaryPath != null &&
                    System.IO.File.Exists(temporaryPath))
                {
                    System.IO.File.Delete(temporaryPath);
                }
            }

            return RedirectToAction("Index");
        }

        // POST: /Client/Profile/RemovePhoto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemovePhoto()
        {
            try
            {
                string userId =
                    User.Identity.GetUserId();

                string prefix = PhotoPrefix(userId);

                foreach (string extension
                    in PhotoExtensions)
                {
                    string path = Path.Combine(
                        PhotoDirectory,
                        prefix + extension
                    );

                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }

                TempData["PhotoSuccess"] =
                    "Profile photo removed.";
            }
            catch (IOException)
            {
                TempData["PhotoError"] =
                    "Could not remove the profile photo.";
            }
            catch (UnauthorizedAccessException)
            {
                TempData["PhotoError"] =
                    "The application cannot remove the photo.";
            }

            return RedirectToAction("Index");
        }

        // Storage inside App_Data
        private string PhotoDirectory
        {
            get
            {
                return Path.Combine(
                    Server.MapPath("~/App_Data"),
                    "ClientProfilePhotos"
                );
            }
        }

        // Convert the user ID into a safe
        // deterministic file prefix.
        private static string PhotoPrefix(
            string userId)
        {
            using (var sha = SHA256.Create())
            {
                byte[] hash = sha.ComputeHash(
                    Encoding.UTF8.GetBytes(userId)
                );

                return BitConverter.ToString(hash)
                    .Replace("-", "")
                    .ToLowerInvariant();
            }
        }

        private string FindPhotoPath(
            string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return null;
            }

            string prefix = PhotoPrefix(userId);

            foreach (string extension
                in PhotoExtensions)
            {
                string path = Path.Combine(
                    PhotoDirectory,
                    prefix + extension
                );

                if (System.IO.File.Exists(path))
                {
                    return path;
                }
            }

            return null;
        }

        // Basic image signature verification.
        private static bool GetValidImageExtension(
            byte[] bytes,
            out string extension)
        {
            extension = null;

            // JPEG
            if (bytes.Length >= 4 &&
                bytes[0] == 0xFF &&
                bytes[1] == 0xD8 &&
                bytes[2] == 0xFF)
            {
                extension = ".jpg";
            }
            // PNG
            else if (bytes.Length >= 8 &&
                     bytes[0] == 137 &&
                     bytes[1] == 80 &&
                     bytes[2] == 78 &&
                     bytes[3] == 71 &&
                     bytes[4] == 13 &&
                     bytes[5] == 10 &&
                     bytes[6] == 26 &&
                     bytes[7] == 10)
            {
                extension = ".png";
            }
            // WebP
            else if (bytes.Length >= 16 &&
                     bytes[0] == 'R' &&
                     bytes[1] == 'I' &&
                     bytes[2] == 'F' &&
                     bytes[3] == 'F' &&
                     bytes[8] == 'W' &&
                     bytes[9] == 'E' &&
                     bytes[10] == 'B' &&
                     bytes[11] == 'P' &&
                     bytes[12] == 'V' &&
                     bytes[13] == 'P' &&
                     bytes[14] == '8' &&
                     (bytes[15] == ' ' ||
                      bytes[15] == 'L' ||
                      bytes[15] == 'X'))
            {
                extension = ".webp";
            }

            return extension != null;
        }

        protected override void Dispose(
            bool disposing)
        {
            if (disposing)
            {
                var clientDisposable =
                    _clientService as IDisposable;

                if (clientDisposable != null)
                {
                    clientDisposable.Dispose();
                }

                var progressDisposable =
                    _progressService as IDisposable;

                if (progressDisposable != null)
                {
                    progressDisposable.Dispose();
                }
            }

            base.Dispose(disposing);
        }
    }
}
