using BugTracks.Extensions;
using BugTracks.Models;
using BugTracks.Models.ViewModels;
using BugTracks.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BugTracks.Controllers
{
    [Authorize]
    public class UserRolesController : Controller
    {
        private readonly IBTRolesService _rolesService;
        private readonly IBTCompanyInfoService _companyInfoService;

        public UserRolesController(IBTRolesService rolesService, IBTCompanyInfoService companyInfoService)
        {
            _rolesService = rolesService;
            _companyInfoService = companyInfoService;
        }

        [HttpGet]
        public async Task<IActionResult> ManageUserRoles()
        {
            // Add an Instance of a ViewModel as a List
            List<ManageUserRolesViewModel> model = new();

            // Get CompanyId
            int companyId = User.Identity.GetCompanyId().Value;

            // Get all Company Users
            List<BTUser> users = await _companyInfoService.GetAllMembersAsync(companyId);

            // Loop over the users to populate the ViewModel
            // - Instantiate ViewModel
            // - Use _rolesServices
            // - Create multi-selects
            foreach(BTUser user in users)
            {
                ManageUserRolesViewModel viewModel = new();
                viewModel.BTUser = user;
                IEnumerable<string> selected = await _rolesService.GetUserRolesAsync(user);
                viewModel.Roles = new MultiSelectList(await _rolesService.GetRolesAsync(), "Name", "Name", selected);

                model.Add(viewModel);
            }

            // Return the model to the View

            return View(model);
        }
        
        
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageUserRoles(ManageUserRolesViewModel member)
        {
            // Get CompanyId from UserClaims
            int companyId = User.Identity.GetCompanyId().Value;

            // Instantiate the BTUser from the CompInfoService
            BTUser btUser = (await _companyInfoService.GetAllMembersAsync(companyId)).FirstOrDefault(u=>u.Id == member.BTUser.Id);

            // Get the Roles for the User
            IEnumerable<string> roles = await _rolesService.GetUserRolesAsync(btUser);

            // Grab the selected role
            string userRole = member.SelectedRoles.FirstOrDefault();

            if(!string.IsNullOrEmpty(userRole))
            {
                
                // Remove the User from their roles
                if(await _rolesService.RemoveUserFromRolesAsync(btUser, roles))
                {
                    // Add User to the new role
                    await _rolesService.AddUserToRoleAsync(btUser, userRole);
                }

            }
            
            // Navigate back to the View

            return RedirectToAction(nameof(ManageUserRoles));
        }
    }
}
