using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BugTracks.Models
{
    public class Project
    {
        public int Id { get; set; }

        [DisplayName("Company")]
        public int CompanyId { get; set; }

        [Required]
        [StringLength(50)]
        [DisplayName("Project Name")]
        public string Name { get; set; }

        [DisplayName("Description")]
        public string Description { get; set; }

        [DisplayName("Start Date")]
        public DateTimeOffset StartDate { get; set; } = new DateTimeOffset(new DateTime(2022, 8, 20), new TimeSpan(0, 0, 0));

        [DisplayName("End Date")]
        public DateTimeOffset EndDate { get; set; } = new DateTimeOffset(new DateTime(2022, 8, 20), new TimeSpan(0, 0, 0));

        [DisplayName("Priority")]
        public int? ProjectPriorityId { get; set; }


        // Image Properties
        [NotMapped]
        [DataType(DataType.Upload)]
        public IFormFile? ImageFile { get; set; }

        [DisplayName("File Name")]
        public string? ImageFileName { get; set; }

        public byte[]? ImageFileData { get; set; }

        [DisplayName("File Extension")]
        public string? ImageContentType { get; set; }

        [DisplayName("Archived")]
        public bool Archived { get; set; }


        // Navigation Properties
        public virtual Company Company { get; set; }
        public virtual ProjectPriority ProjectPriority { get; set; }
        public virtual ICollection<BTUser> Members { get; set; } = new HashSet<BTUser>();
        public virtual ICollection<Ticket> Tickets { get; set; } = new HashSet<Ticket>();

    }
}
