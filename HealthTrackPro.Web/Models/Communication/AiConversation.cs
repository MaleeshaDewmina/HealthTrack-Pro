using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthTrackPro.Web.Models.Communication
{
    public class AiConversation
    {
        public AiConversation()
        {
            Messages = new HashSet<AiMessage>();
        }

        [Key]
        public int AiConversationId { get; set; }

        [Required]
        [StringLength(128)]
        [ForeignKey("Client")]
        public string ClientUserId { get; set; }

        [Required]
        [StringLength(120)]
        public string Title { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime UpdatedAtUtc { get; set; }

        public virtual ApplicationUser Client { get; set; }

        public virtual ICollection<AiMessage> Messages { get; set; }
    }
}
