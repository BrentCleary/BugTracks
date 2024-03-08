using BugTracks.Models;

namespace BugTracks.Services.Interfaces
{
    public interface IBTRolesService
    {
        // User Role Status, Add, Remove
        public Task<bool> IsUserInRoleAsync(BTUser user, string roleName);

        Get

        public Task<IEnumerable<string>> GetUserRolesAsync(BTUser user);

        public Task<bool> AddUserToRoleAsync(BTUser user, string roleName);

        public Task<bool> RemoveUserFromRoleAsync(BTUser user, string roleName);

        public Task<bool> RemoveUserFromRolesAsync(BTUser user, IEnumerable<string> roles);


        // Role Users Information
        public Task<List<BTUser>> GetUsersInRoleAsync(string roleName, int companyId);

        public Task<List<BTUser>> GetUsersNotInRoleAsync(string roleName, int companyId);


        // Returns Role Id
        public Task<string> GetRoleNameByIdAsync(string roleId);

    }
}
