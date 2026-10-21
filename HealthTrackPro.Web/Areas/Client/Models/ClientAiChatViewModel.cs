using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HealthTrackPro.Web.Areas.Client.Models
{
    public class ClientAiChatViewModel
    {
        public int? ConversationId { get; set; }

        [Required(ErrorMessage = "Please enter a message.")]
        [StringLength(1500, ErrorMessage = "Your message must be 1500 characters or less.")]
        [Display(Name = "Your Message")]
        public string NewMessage { get; set; }

        public List<AiChatMessageViewModel> Messages { get; set; }
        public List<AiConversationListItemViewModel> Conversations { get; set; }

        public string ErrorMessage { get; set; }

        public ClientAiChatViewModel()
        {
            Messages = new List<AiChatMessageViewModel>();
            Conversations = new List<AiConversationListItemViewModel>();
        }
    }

    public class AiConversationListItemViewModel
    {
        public int ConversationId { get; set; }
        public string Title { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
    }

    public class AiChatMessageViewModel
    {
        public string Role { get; set; }
        public string Content { get; set; }
        public DateTime TimestampUtc { get; set; }
    }
}
