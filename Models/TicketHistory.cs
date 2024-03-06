using Microsoft.Identity.Client;
using System.ComponentModel;

namespace BugTracks.Models
{
    public class TicketHistory
    {
        public int Id { get; set; }

        [DisplayName("Ticket")]
        public int TicketId { get; set; }

        [DisplayName("Updated Item")]
        public string Property { get; set; }

        [DisplayName("Previous")]
        public string OldValue { get; set; }

        [DisplayName("Current")]
        public string NewValue { get; set; }

        [DisplayName("Date Modified")]
        public DateTimeOffset Created { get; set; } = new DateTimeOffset(new DateTime(2022, 8, 20), new TimeSpan(0, 0, 0));

        [DisplayName("Description of Change")]
        public string Description { get; set; }

        [DisplayName("Team Member")]
        public string? UserId { get; set; }

        // Navigation Properties
        public virtual Ticket Ticket { get; set; }

        public virtual BTUser User { get; set; }
    
    }
}
