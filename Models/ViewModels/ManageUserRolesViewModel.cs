using Microsoft.AspNetCore.Mvc.Rendering;

namespace BugTracks.Models.ViewModels
{
    public class ManageUserRolesViewModel
    {
        public BTUser BTUser { get; set; }
        public MultiSelectList Roles { get; set; }
        public List<String> SelectedRoles { get; set; }

    }
}
