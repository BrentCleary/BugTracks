using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BugTracks.Models
{
    public class ProjectPriority
    {
        public int Id { get; set; }

        [DisplayName("Priority Name")]
        public string Name { get; set; }
    
    }
}
