using BugTracks.Data;
using BugTracks.Models;
using BugTracks.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;
using System.Linq;

namespace BugTracks.Services
{
    public class BTProjectService : IBTProjectService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BTUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public BTProjectService(ApplicationDbContext context,
                                UserManager<BTUser> userManager,
                                RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task AddNewProjectAsync(Project project)
        {
            _context.Add(project);
            await _context.SaveChangesAsync();

        }

        public async Task<bool> AddProjectManagerAsync(string userId, int projectId)
        {
            BTUser user = await _userManager.FindByIdAsync(userId);



            bool result = await _userManager.AddToRoleAsync(user, role);
        }

        public async Task<bool> AddUserToProjectAsync(string userId, int projectId)
        {
            BTUser user = await _context.Users.FirstOrDefaultAsync(u=> u.Id == userId);
            
            if(user != null)
            {
                Project project = await _context.Projects.Include(p => p.Members).FirstOrDefaultAsync(p => p.Id == projectId);
                if(!await IsUserOnProjectAsync(userId, projectId))
                {
                    try
                    {
                        project.Members.Add(user);
                        await _context.SaveChangesAsync();
                        return true;
                    }
                    catch(Exception)
                    {
                        throw;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }

        public async Task ArchiveProjectAsync(Project project)
        {
            project.Archived = true;
            _context.Update(project);
            await _context.SaveChangesAsync();
        }

        List<int> userIds = (await _userManager.GetUsersInRoleAsync(roleName)).Select(u => u.Id).ToList();
        List<BTUser> roleUsers = _context.Users.Where(u => !userIds.Contains(u.Id)).ToList();
        List<BTUser> result = roleUsers.Where(u => u.CompanyId == companyId).ToList();

            return result;

        public async Task<List<BTUser>> GetAllProjectMembersExceptPMAsync(int projectId)
        {
            List<int> userIds = (await _userManager.GetUsersInRoleAsync(Manager)).Select(u => u.Id).ToList();
            List<BTUser> roleUsers = _context.Users.Where(u => !userIds.Contains(u.Id)).ToList();
            List<BTUser> result = roleUsers.Where(u => u.ProjectId == projectId).ToList();
        }

        public async Task<List<Project>> GetAllProjectsByCompany(int companyId)
        {
            List<Project> result = new List<Project>();

            result = await _context.Projects.Where(p => p.CompanyId == companyId && p.Archived == false)
                                            .Include(p => p.Members)
                                            .Include(p => p.Tickets)
                                                .ThenInclude(t => t.Comments)
                                            .Include(p => p.Tickets)
                                                .ThenInclude(t => t.Attachments)
                                            .Include(p => p.Tickets)
                                                .ThenInclude(t => t.History)
                                            .Include(p => p.Tickets)
                                                .ThenInclude(t => t.Notifications)
                                            .Include(p => p.Tickets)
                                                .ThenInclude(t => t.DeveloperUser)
                                            .Include(p => p.Tickets)
                                                .ThenInclude(t => t.OwnerUser)
                                            .Include(p => p.Tickets)
                                                .ThenInclude(t => t.TicketStatus)
                                            .Include(p => p.Tickets)
                                                .ThenInclude(t => t.TicketPriority)
                                            .Include(p => p.Tickets)
                                                .ThenInclude(t => t.TicketType)
                                            .Include(p => p.ProjectPriority)
                                            .ToListAsync();

            return result;
        }

        public Task<List<Project>> GetAllProjectsByPriority(int companyId, string priorityName)
        {
            throw new NotImplementedException();
        }

        public Task<List<Project>> GetArchivedProjectsByCompany(int companyId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<BTUser>> GetDevelopersOnProjectAsync(int projectId)
        {
            List<BTUser> users = _context.Projects.
            List<int> userIds = await _userManager.GetUsersInRoleAsync(Developer).Select(u => u.Id).ToListAsync();
        }

        public async Task<Project> GetProjectByIdAsync(int projectId, int companyId)
        {
            Project project = await _context.Projects
                                            .Include(p => p.Tickets)
                                            .Include(p => p.Members)
                                            .Include(p => p.ProjectPriority)
                                            .FirstOrDefaultAsync(p => p.Id == projectId && p.CompanyId == companyId);

            return project;
        }

        public async Task<BTUser> GetProjectManagerAsync(int projectId)
        {
            
        }

        public Task<List<BTUser>> GetProjectMembersByRoleAsync(int projectId, string role)
        {
            throw new NotImplementedException();
        }

        public Task<List<BTUser>> GetSubmittersOnProjectAsync(int projectId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Project>> GetUserProjectsAsync(string userId)
        {
            try
            {
                List<Project> userProjects = (await _context.Users
                    .Include(u => u.Projects)
                        .ThenInclude(p=>p.Company)
                    .Include(u => u.Projects)
                        .ThenInclude(p => p.Members)
                    .Include(u => u.Projects)
                        .ThenInclude(p => p.Tickets)
                    .Include(u => u.Projects)
                        .ThenInclude(p => p.Tickets)
                            .ThenInclude(t=>t.DeveloperUser)
                    .Include(u => u.Projects)
                        .ThenInclude(p => p.Tickets)
                            .ThenInclude(t => t.OwnerUser)
                    .Include(u => u.Projects)
                        .ThenInclude(p => p.Tickets)
                            .ThenInclude(t => t.TicketPriority)
                    .Include(u => u.Projects)
                        .ThenInclude(p => p.Tickets)
                            .ThenInclude(t => t.TicketStatus)
                    .Include(u => u.Projects)
                        .ThenInclude(p => p.Tickets)
                            .ThenInclude(t => t.TicketType)
                    .FirstOrDefaultAsync(u=>u.Id == userId)).Projects.ToList();

                return userProjects;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"********** ERROR ********** - Error Getting User Projects List. ----> {ex.Message}");
                throw;
            }


        }

        public Task<List<BTUser>> GetUsersNotOnProjectAsync(int projectId, int companyId)
        {
            throw new NotImplementedException();
        }

        // Returns project by Id, including Member List, checks for userId in list, returns bool
        public async Task<bool> IsUserOnProjectAsync(string userId, int projectId)
        {
            Project project = await _context.Projects.Include(p => p.Members).FirstOrDefaultAsync(p => p.Id == projectId);
            bool result = false; ;
            
            if(project != null)
            {
                // Any Query return a bool
                result = project.Members.Any(m => m.Id == userId);
            }

            return result;
        }

        public Task<int> LookupProjectPriorityId(string priorityName)
        {
            throw new NotImplementedException();
        }

        public Task RemoveProjectManagerAsync(int projectId)
        {
            throw new NotImplementedException();
        }

        public async Task RemoveUserFromProjectAsync(string userId, int projectId)
        {

            try
            {
                BTUser user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                Project project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
                try
                {

                    if (await IsUserOnProjectAsync(userId, projectId))
                    {
                        project.Members.Remove(user);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"********** ERROR ********** - Error Removing User from Project. ----> {ex.Message}");
                throw;
            }

        }

        public Task RemoveUsersFromProjectByRoleAsync(string role, int projectId)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateProjectAsync(Project project)
        {
            _context.Update(project);
            await _context.SaveChangesAsync();
        }
    }
}
