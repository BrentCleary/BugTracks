using System.ComponentModel;

namespace BugTracks.Models
{
    public class Company
    {
        // Primary Key
        public int Id { get; set; }

        [DisplayName("Company Name")]
        public string Name { get; set; }

        [DisplayName("Company Description")]
        public string Description { get; set; }


        //Navivation Properties
        public virtual ICollection<BTUser> Members { get; set; } 
        public virtual ICollection<Project> Projects { get; set; }

    }
}
