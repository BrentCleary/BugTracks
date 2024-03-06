using System.ComponentModel;

namespace BugTracks.Models
{
    public class Invite
    {
        // Primary Key
        public int Id { get; set; }

        [DisplayName("Date Sent")]
        public DateTimeOffset InviteDate { get; set; } = new DateTimeOffset(new DateTime(2022, 8, 20), new TimeSpan(0, 0, 0));

        [DisplayName("Join Date")]
        public DateTimeOffset JoinDate { get; set; } 


        // Used to track a specific company in BugTracks
        [DisplayName("Code")]
        public Guid CompanyToken { get; set; }


        // Nav Ids
        [DisplayName("Company")]
        public int CompanyId { get; set; }

        [DisplayName("Project")]
        public int ProjectId { get; set; }

        [DisplayName("Invitor")]
        public string InvitorId { get; set; }

        [DisplayName("Invitee")]
        public string InviteeId { get; set; }

        // Invitee Props
        [DisplayName("Invitee Email")]
        public string InviteeEmail { get; set; }

        [DisplayName("Invitee First Name")]
        public string InviteeFirstName { get; set; }

        [DisplayName("Invitee Last Name")]
        public string InviteeLastName { get; set; }


        // Determines whether the invite is valid after a certain number of days
        public bool IsValid { get; set; }
    
    
        // Navigation Properties
        public virtual Company Company { get; set; }
        public virtual BTUser Invitor { get; set; }
        public virtual BTUser Invitee { get; set; }
        public virtual Project Project { get; set; }

    }
}
