using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using HealthTrackPro.Web.Areas.Client.Models;
using HealthTrackPro.Web.Areas.Client.Services;
using HealthTrackPro.Web.Models;
using HealthTrackPro.Web.Models.Communication;

namespace HealthTrackPro.Web.Areas.Client.Controllers
{
    [Authorize(Roles = "Client")]
    public class AiChatController : Controller
    {
        private readonly ClientAiCoachService aiService =
            new ClientAiCoachService();

        private readonly ApplicationDbContext db =
            new ApplicationDbContext();

        [HttpGet]
        public async Task<ActionResult> Index(int? conversationId)
        {
            string userId = User.Identity.GetUserId();

            var model = new ClientAiChatViewModel();

            if (conversationId.HasValue)
            {
                var conversation = await db.AiConversations
                    .FirstOrDefaultAsync(c =>
                        c.AiConversationId == conversationId.Value &&
                        c.ClientUserId == userId);

                if (conversation == null)
                    return HttpNotFound();

                model.ConversationId = conversation.AiConversationId;

                model.Messages = await db.AiMessages
                    .Where(m =>
                        m.AiConversationId == conversation.AiConversationId)
                    .OrderBy(m => m.AiMessageId)
                    .Select(m => new AiChatMessageViewModel
                    {
                        Role = m.Role,
                        Content = m.Content,
                        TimestampUtc = m.CreatedAtUtc
                    })
                    .ToListAsync();
            }

            await LoadSidebarAsync(model, userId);

            ViewBag.AiEnabled = aiService.IsConfigured;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(ClientAiChatViewModel model)
        {
            string userId = User.Identity.GetUserId();

            if (model == null)
            {
                model = new ClientAiChatViewModel();
                ModelState.AddModelError(
                    "NewMessage", "Please enter a message.");
            }

            AiConversation conversation = null;

            if (model.ConversationId.HasValue)
            {
                conversation = await db.AiConversations
                    .FirstOrDefaultAsync(c =>
                        c.AiConversationId == model.ConversationId.Value &&
                        c.ClientUserId == userId);

                if (conversation == null)
                    return HttpNotFound();
            }

            if (!ModelState.IsValid ||
                string.IsNullOrWhiteSpace(model.NewMessage))
            {
                if (string.IsNullOrWhiteSpace(model.NewMessage))
                {
                    ModelState.AddModelError(
                        "NewMessage", "Please enter a message.");
                }

                if (conversation != null)
                {
                    model.Messages = await GetMessagesAsync(
                        conversation.AiConversationId);
                }

                await LoadSidebarAsync(model, userId);
                ViewBag.AiEnabled = aiService.IsConfigured;

                return View(model);
            }

            string userMessage = model.NewMessage.Trim();
            DateTime now = DateTime.UtcNow;

            if (conversation == null)
            {
                conversation = new AiConversation
                {
                    ClientUserId = userId,
                    Title = userMessage.Length > 60
                        ? userMessage.Substring(0, 60) + "..."
                        : userMessage,
                    CreatedAtUtc = now,
                    UpdatedAtUtc = now
                };

                db.AiConversations.Add(conversation);
            }

            conversation.UpdatedAtUtc = now;

            db.AiMessages.Add(new AiMessage
            {
                Conversation = conversation,
                Role = "user",
                Content = userMessage,
                CreatedAtUtc = now
            });

            await db.SaveChangesAsync();

            int id = conversation.AiConversationId;

            if (!aiService.IsConfigured)
            {
                await SaveStatusAsync(
                    id,
                    "Live AI is not configured yet. " +
                    "Your message was saved permanently, " +
                    "but no AI response was generated.");
            }
            else
            {
                try
                {
                    var history = await GetMessagesAsync(id);

                    string reply =
                        await aiService.GetReplyAsync(history);

                    db.AiMessages.Add(new AiMessage
                    {
                        AiConversationId = id,
                        Role = "assistant",
                        Content = reply,
                        CreatedAtUtc = DateTime.UtcNow
                    });

                    conversation.UpdatedAtUtc = DateTime.UtcNow;

                    await db.SaveChangesAsync();
                }
                catch (InvalidOperationException)
                {
                    await SaveStatusAsync(
                        id,
                        "The AI service could not process your request. " +
                        "Please try again later.");
                }
                catch (HttpRequestException)
                {
                    await SaveStatusAsync(
                        id,
                        "Unable to connect to the AI service. " +
                        "Please check your connection and try again.");
                }
                catch (TaskCanceledException)
                {
                    await SaveStatusAsync(
                        id,
                        "The AI request timed out. Please try again.");
                }
                catch (Exception ex)
                {
                    Trace.TraceError(
                        "HealthTrack AI Coach error type: {0}",
                        ex.GetType().FullName);

                    await SaveStatusAsync(
                        id,
                        "The AI service is temporarily unavailable. " +
                        "Please try again later.");
                }
            }

            return RedirectToAction(
                "Index",
                new { conversationId = id });
        }

        [HttpGet]
        public ActionResult NewChat()
        {
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Open(int id)
        {
            return RedirectToAction(
                "Index",
                new { conversationId = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {
            string userId = User.Identity.GetUserId();

            var conversation = await db.AiConversations
                .FirstOrDefaultAsync(c =>
                    c.AiConversationId == id &&
                    c.ClientUserId == userId);

            if (conversation == null)
                return HttpNotFound();

            db.AiConversations.Remove(conversation);
            await db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        private async Task<List<AiChatMessageViewModel>>
            GetMessagesAsync(int conversationId)
        {
            return await db.AiMessages
                .Where(m => m.AiConversationId == conversationId)
                .OrderBy(m => m.AiMessageId)
                .Select(m => new AiChatMessageViewModel
                {
                    Role = m.Role,
                    Content = m.Content,
                    TimestampUtc = m.CreatedAtUtc
                })
                .ToListAsync();
        }

        private async Task LoadSidebarAsync(
            ClientAiChatViewModel model,
            string userId)
        {
            model.Conversations = await db.AiConversations
                .Where(c => c.ClientUserId == userId)
                .OrderByDescending(c => c.UpdatedAtUtc)
                .Select(c => new AiConversationListItemViewModel
                {
                    ConversationId = c.AiConversationId,
                    Title = c.Title,
                    UpdatedAtUtc = c.UpdatedAtUtc
                })
                .ToListAsync();
        }

        private async Task SaveStatusAsync(
            int conversationId,
            string content)
        {
            db.AiMessages.Add(new AiMessage
            {
                AiConversationId = conversationId,
                Role = "status",
                Content = content,
                CreatedAtUtc = DateTime.UtcNow
            });

            await db.SaveChangesAsync();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}
