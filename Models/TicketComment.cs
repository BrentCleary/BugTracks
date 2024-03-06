using System.ComponentModel;

namespace BugTracks.Models
{
    public class TicketComment
    {
        public int Id { get; set; }

        [DisplayName("Member Comment")]
        public string Comment { get; set; }

        [DisplayName("Date")]
        public DateTimeOffset Created { get; set; } = new DateTimeOffset(new DateTime(2022, 8, 20), new TimeSpan(0, 0, 0));

        [DisplayName("Ticket")]
        public int TicketId { get; set; }

        [DisplayName("Team Member")]
        public string? UserId { get; set; }
    
        // Navigation Properties
        public virtual Ticket Ticket { get; set; }

        public virtual BTUser User { get; set; }
    }
}
