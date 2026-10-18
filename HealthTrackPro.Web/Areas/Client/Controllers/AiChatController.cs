
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using HealthTrackPro.Web.Areas.Client.Models;

namespace HealthTrackPro.Web.Areas.Client.Controllers
{
    [Authorize(Roles = "Client")]
    public class AiChatController : Controller
    {
        private const string ChatSessionKey =
            "HealthTrackClientAiChatHistory";

        [HttpGet]
        public ActionResult Index()
        {
            var model = new ClientAiChatViewModel
            {
                Messages = GetChatHistory().ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ClientAiChatViewModel model)
        {
            var history = GetChatHistory();

            if (!ModelState.IsValid ||
                string.IsNullOrWhiteSpace(model.NewMessage))
            {
                model.Messages = history.ToList();

                if (string.IsNullOrWhiteSpace(model.NewMessage))
                {
                    ModelState.AddModelError(
                        "NewMessage",
                        "Please enter a message.");
                }

                return View(model);
            }

            history.Add(new AiChatMessageViewModel
            {
                Role = "user",
                Content = model.NewMessage.Trim(),
                TimestampUtc = DateTime.UtcNow
            });

            // Temporary system message.
            // We will replace this with a real Grok API reply.
            history.Add(new AiChatMessageViewModel
            {
                Role = "system",
                Content =
                    "Your message was received. " +
                    "Grok AI is not connected yet, so I cannot " +
                    "generate a fitness answer right now. " +
                    "We will enable real replies when the API " +
                    "connection is configured.",
                TimestampUtc = DateTime.UtcNow
            });

            if (history.Count > 20)
            {
                history.RemoveRange(
                    0,
                    history.Count - 20);
            }

            Session[ChatSessionKey] = history;

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Clear()
        {
            Session.Remove(ChatSessionKey);

            return RedirectToAction("Index");
        }

        private List<AiChatMessageViewModel> GetChatHistory()
        {
            var history = Session[ChatSessionKey]
                as List<AiChatMessageViewModel>;

            if (history == null)
            {
                history = new List<AiChatMessageViewModel>();
                Session[ChatSessionKey] = history;
            }

            return history;
        }
    }
}
