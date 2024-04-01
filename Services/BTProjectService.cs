using BugTracks.Data;
using BugTracks.Models;
using BugTracks.Models.Enums;
using BugTracks.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BugTracks.Extensions;
using System.ComponentModel.Design;
using System.Linq;

namespace BugTracks.Services
{
    public class BTProjectService : IBTProjectService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BTUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IBTRolesService _rolesService;

        public BTProjectService(ApplicationDbContext context,
                                UserManager<BTUser> userManager,
                                RoleManager<IdentityRole> roleManager,
                                IBTRolesService roleService)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _rolesService = roleService;
        }

        public async Task AddNewProjectAsync(Project project)
        {
            _context.Add(project);
            await _context.SaveChangesAsync();

        }

        public async Task<bool> AddProjectManagerAsync(string userId, int projectId)
        {
            // Remove Current PM
            BTUser currentPM = await GetProjectManagerAsync(projectId);
            if (currentPM != null)
            {
                try
                {
                    await RemoveProjectManagerAsync(projectId);
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Error removing current PM. - Error: {ex.Message}");
                    return false;
                }
            }

            // Add new PM
            try
            {
                await AddUserToProjectAsync(userId, projectId);
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error adding new PM. - Error: {ex.Message}");
                return false;
            }

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
            try
            {
                project.Archived = true;
                await UpdateProjectAsync(project);

                foreach(Ticket ticket in project.Tickets)
                {
                    ticket.ArchivedByProject = true;
                    _context.Update(ticket);
                    await _context.SaveChangesAsync();
                }

            }
            catch (Exception)
            {

                throw;
            }
        }


        public async Task<List<BTUser>> GetAllProjectMembersExceptPMAsync(int projectId)
        {
            List<BTUser> developers = await GetProjectMembersByRoleAsync(projectId, Roles.Developer.ToString());
            List<BTUser> submitters = await GetProjectMembersByRoleAsync(projectId, Roles.Submitter.ToString());
            List<BTUser> admin = await GetProjectMembersByRoleAsync(projectId, Roles.Admin.ToString());

            List<BTUser> teamMembers = developers.Concat(submitters).Concat(admin).ToList();

            return teamMembers;


        }

        public async Task<List<Project>> GetAllProjectsByCompanyAsync(int companyId)
        {
            List<Project> result = new List<Project>();

            result = await _context.Projects.Where(p => p.CompanyId == companyId && p.Archived == false)
                                            .Include(p => p.Members)
                                            .Include(p => p.Company)
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

        public async Task<List<Project>> GetAllProjectsByPriorityAsync(int companyId, string priorityName)
        {
            List<Project> projects = await GetAllProjectsByCompanyAsync(companyId);
            int priorityId = await LookupProjectPriorityIdAsync(priorityName);

			return projects.Where(p => p.ProjectPriorityId == priorityId).ToList();

        }

        public async Task<List<Project>> GetArchivedProjectsByCompanyAsync(int companyId)
        {
            try
            {
                List<Project> projects = await _context.Projects.Where(p => p.CompanyId == companyId && p.Archived == true)
                                            .Include(p => p.Members)
                                            .Include(p => p.Company)
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

                return projects;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<BTUser>> GetDevelopersOnProjectAsync(int projectId)
        {
			List<BTUser> members = new();

			Project project = await _context.Projects
								.Include(p => p.Members)
								.FirstOrDefaultAsync(p => p.Id == projectId);

			foreach (BTUser member in project?.Members)
			{
				if (await _rolesService.IsUserInRoleAsync(member, Roles.Developer.ToString()))
				{
					members.Add(member);
				}
			}

			return members;
		}

        public async Task<Project> GetProjectByIdAsync(int projectId, int companyId)
        {

            Project project = await _context.Projects
                                            .Include(p => p.Tickets)
                                                .ThenInclude(t => t.TicketPriority)
                                            .Include(p => p.Tickets)
                                                .ThenInclude(t => t.TicketStatus)
                                            .Include(p => p.Tickets)
                                                .ThenInclude(t => t.TicketType)
                                            .Include(p => p.Tickets)
                                                .ThenInclude(t => t.DeveloperUser)
                                            .Include(p => p.Tickets)
                                                .ThenInclude(t => t.OwnerUser)
                                            .Include(p => p.Members)
                                            .Include(p => p.ProjectPriority)
                                            .FirstOrDefaultAsync(p => p.Id == projectId && p.CompanyId == companyId);


            return project;
        }

        public async Task<BTUser> GetProjectManagerAsync(int projectId)
        {
            Project project = await _context.Projects
                                            .Include(p => p.Members)
                                            .FirstOrDefaultAsync(p=>p.Id == projectId);

            foreach(BTUser member in project?.Members)
            {
                if(await _rolesService.IsUserInRoleAsync(member, Roles.ProjectManager.ToString()))
                {
                    return member;
                }
            }
            
            return null;

        }

        public async Task<List<BTUser>> GetProjectMembersByRoleAsync(int projectId, string role)
        {
            // Find Project
            Project project = await _context.Projects
                                            .Include(p=>p.Members)
                                            .FirstOrDefaultAsync(p => p.Id == projectId);

            // Filter by Role using RoleManager
            List<BTUser> members = new(); 
            
            foreach(var user in project.Members)
            {
                if(await _rolesService.IsUserInRoleAsync(user, role))
                {
                    members.Add(user);
                }
                            
            }
            return members;

        }

        public async Task<List<BTUser>> GetSubmittersOnProjectAsync(int projectId)
        {
            List<BTUser> members = new();

			Project project = await _context.Projects
								.Include(p => p.Members)
								.FirstOrDefaultAsync(p => p.Id == projectId);

			foreach (BTUser member in project?.Members)
			{
				if (await _rolesService.IsUserInRoleAsync(member, Roles.Submitter.ToString()))
				{
					members.Add(member);
				}
			}

			return members;
		}

        // Unassigned is in reference to Project Manager Role
        #region Get Unassigned Projects
        public async Task<List<Project>> GetUnassignedProjectsAsync(int companyId)
        {
            List<Project> result = new();
            List<Project> projects = new();

            try
            {
                projects = await _context.Projects
                                         .Include(p => p.ProjectPriority)
                                         .Include(p => p.Company)
                                         .Where(p => p.CompanyId == companyId).ToListAsync();

                foreach (Project project in projects)
                {
                    // If Count == 0, statement is true, and project has no ProjectManager
                    if ((await GetProjectMembersByRoleAsync(project.Id, nameof(Roles.ProjectManager))).Count == 0)
                    {
                        result.Add(project);
                    }
                }

            }
            catch (Exception)
            {

                throw;
            }

            return result;
        }

        #endregion

        #region Get User Projects
        public async Task<List<Project>> GetUserProjectsAsync(string userId)
        {
            try
            {
                List<Project> userProjects = (await _context.Users
                    .Include(u => u.Projects)
                        .ThenInclude(p => p.Company)
                    .Include(u => u.Projects)
                        .ThenInclude(p => p.Members)
                    .Include(u => u.Projects)
                        .ThenInclude(p => p.Tickets)
                    .Include(u => u.Projects)
                        .ThenInclude(p => p.Tickets)
                            .ThenInclude(t => t.DeveloperUser)
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
                    .FirstOrDefaultAsync(u => u.Id == userId)).Projects.ToList();

                return userProjects;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"********** ERROR ********** - Error Getting User Projects List. ----> {ex.Message}");
                throw;
            }


        }

        #endregion

        #region Get Users Not On Project
        public async Task<List<BTUser>> GetUsersNotOnProjectAsync(int projectId, int companyId)
        {

            List<BTUser> users = await _context.Users.Where(u => u.Projects.All(p => p.Id != projectId)).ToListAsync();

            return users.Where(u => u.CompanyId == companyId).ToList();

        }

        #endregion

        #region Is Assigned Project Manager
        public async Task<bool> IsAssignedProjectManagerAsync(string userId, int projectId)
        {
            try
            {
                string projectManagerId = (await GetProjectManagerAsync(projectId))?.Id;

                if (projectManagerId == userId)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception)
            {

                throw;
            }

        }

        #endregion

        // Returns project by Id, including Member List, checks for userId in list, returns bool
        #region Is User On Project
        public async Task<bool> IsUserOnProjectAsync(string userId, int projectId)
        {
            Project project = await _context.Projects.Include(p => p.Members).FirstOrDefaultAsync(p => p.Id == projectId);
            bool result = false; ;

            if (project != null)
            {
                // Any Query returns a bool
                result = project.Members.Any(m => m.Id == userId);
            }

            return result;
        }

		#endregion

		#region Lookup Projec tPriority Id Async
		public async Task<int> LookupProjectPriorityIdAsync(string priorityName)
        {
            int priorityId = (await _context.ProjectPriorities.FirstOrDefaultAsync(p => p.Name == priorityName)).Id;

            return priorityId;
        }

        #endregion

        #region Remove Project Manager
        public async Task RemoveProjectManagerAsync(int projectId)
        {
            Project project = await _context.Projects.Include(p => p.Members).FirstOrDefaultAsync(p => p.Id == projectId);

            try
            {
                foreach (BTUser member in project?.Members)
                {
                    if (await _rolesService.IsUserInRoleAsync(member, Roles.ProjectManager.ToString()))
                    {
                        await RemoveUserFromProjectAsync(member.Id, projectId);
                    }
                }

            }
            catch
            {
                throw;
            }

        }

        #endregion

        #region Remove User From Project
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
        #endregion


        public async Task RemoveUsersFromProjectByRoleAsync(string role, int projectId)
        {
            try
            {
                List<BTUser> members = await GetProjectMembersByRoleAsync(projectId, role);
            
                Project project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
                
                foreach(BTUser btuser in members)
                {
                    try
                    {
                        project.Members.Remove(btuser);
                        _context.SaveChanges();

                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"********** ERROR ********** - Error Removing User from Project by Role. ----> {ex.Message}");
                throw;
            }
        }

        public async Task RestoreProjectAsync(Project project)
        {
            try
            {
                project.Archived = false;
                await UpdateProjectAsync(project);

                foreach (Ticket ticket in project.Tickets)
                {
                    ticket.ArchivedByProject = false;
                    _context.Update(ticket);
                    await _context.SaveChangesAsync();
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task UpdateProjectAsync(Project project)
        {
            _context.Update(project);
            await _context.SaveChangesAsync();
        }

    }
}
