using System.ComponentModel;

namespace BugTracks.Models
{
    public class TicketType
    {
        public int Id { get; set; }

        [DisplayName("Type Name")]
        public string Name { get; set; }
    }

    // This model is a "Lookup Table"
}
