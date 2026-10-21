
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HealthTrackPro.Web.Areas.Client.Models
{
    // Data used by the AI Chat page
    public class ClientAiChatViewModel
    {
        [Required(ErrorMessage = "Please enter a message.")]
        [StringLength(
            1500,
            ErrorMessage = "Your message must be 1500 characters or less.")]
        [Display(Name = "Your Message")]
        public string NewMessage { get; set; }

        public List<AiChatMessageViewModel> Messages { get; set; }

        public string ErrorMessage { get; set; }

        public ClientAiChatViewModel()
        {
            Messages = new List<AiChatMessageViewModel>();
        }
    }

    // Represents one message in the conversation
    public class AiChatMessageViewModel
    {
        public string Role { get; set; }

        public string Content { get; set; }

        public DateTime TimestampUtc { get; set; }
    }
}
