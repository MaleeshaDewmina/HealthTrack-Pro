using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthTrackPro.Web.Models.Communication
{
    public class AiMessage
    {
        [Key]
        public int AiMessageId { get; set; }

        [Required]
        [ForeignKey("Conversation")]
        public int AiConversationId { get; set; }

        [Required]
        [StringLength(20)]
        public string Role { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public virtual AiConversation Conversation { get; set; }
    }
}
