using BugTracks.Data;
using BugTracks.Models;
using BugTracks.Services.Interfaces;
using BugTracks.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BugTracks.Services
{
    public class BTTicketService : IBTTicketService
    {
        #region Properties
        private readonly IBTRolesService _rolesService;
        private readonly ApplicationDbContext _context;
        private readonly IBTProjectService _projectService;
        #endregion        

        #region Constructor
        public BTTicketService(ApplicationDbContext context,
                               IBTRolesService rolesService,
                               IBTProjectService projectService) 
        { 
            _context = context;
            _rolesService = rolesService;
            _projectService = projectService;
        }
		#endregion                               

		#region Add New Ticket Async
		public async Task AddNewTicketAsync(Ticket ticket)
        {

            try
            {
                _context.Add(ticket);
                await _context.SaveChangesAsync();

            }
            catch(Exception)
            {
                throw;
            }
        }
		#endregion

		#region Archive Ticket Async
		public async Task ArchiveTicketAsync(Ticket ticket)
        {
            try
            {
                ticket.Archived = true;
                _context.Update(ticket);
                await _context.SaveChangesAsync();

            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Add Ticket Comment
        public async Task AddTicketCommentAsync(TicketComment ticketComment)
        {
            try
            {
                await _context.TicketsComments.AddAsync(ticketComment);
                _context.SaveChangesAsync();

            }
            catch (Exception)
            {

                throw;
            }

        }
		#endregion

		#region Add Ticket Attachment Async
		public async Task AddTicketAttachmentAsync(TicketAttachment ticketAttachment)
        {
            try
            {
                await _context.AddAsync(ticketAttachment);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

		#endregion

		#region Assign Ticket Async
		public async Task AssignTicketAsync(int ticketId, string userId)
        {

            Ticket ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId);

            try
            {
                if (ticket != null)
                {
                    try
                    {
                        ticket.DeveloperUserId = userId;
                        // Revisit this code when assigning Tickets
                        ticket.TicketStatusId = (await LookupTicketStatusIdAsync("Development")).Value;
                        await _context.SaveChangesAsync();

                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

		#endregion

		#region Get All Tickets By Company Async
		public async Task<List<Ticket>> GetAllTicketsByCompanyAsync(int companyId)
        {
            try
            {
                List<Ticket> tickets = await _context.Projects
                                                     .Where(p => p.CompanyId == companyId)
                                                     .SelectMany(p => p.Tickets)
                                                         .Include(t => t.Attachments)
                                                         .Include(t => t.Comments)
                                                         .Include(t => t.DeveloperUser)
                                                         .Include(t => t.History)
                                                         .Include(t => t.OwnerUser)
                                                         .Include(t => t.TicketPriority)
                                                         .Include(t => t.TicketStatus)
                                                         .Include(t => t.TicketType)
                                                         .Include(t => t.Project)
                                                     .ToListAsync();

                return tickets;
            }
            catch (Exception)
            {
                throw;
            }

        }

		#endregion

		#region Get All Tickets By Priority Async
		public async Task<List<Ticket>> GetAllTicketsByPriorityAsync(int companyId, string priorityName)
        {

            int priorityId = (await LookupTicketPriorityIdAsync(priorityName)).Value;

            try
            {
                List<Ticket> tickets = await _context.Projects
                                                     .Where(p => p.CompanyId == companyId)
                                                        .SelectMany(p => p.Tickets)
                                                                .Include(t => t.Attachments)
                                                                .Include(t => t.Comments)
                                                                .Include(t => t.DeveloperUser)
                                                                .Include(t => t.History)
                                                                .Include(t => t.OwnerUser)
                                                                .Include(t => t.TicketPriority)
                                                                .Include(t => t.TicketStatus)
                                                                .Include(t => t.TicketType)
                                                                .Include(t => t.Project)
                                                        .Where(t => t.TicketPriorityId == priorityId)
                                                        .ToListAsync();
                return tickets;
            }
            catch (Exception)
            {
                throw;
            }

        }

		#endregion

		#region Get All Tickets By Status Async
		public async Task<List<Ticket>> GetAllTicketsByStatusAsync(int companyId, string statusName)
        {
            int statusId = (await LookupTicketStatusIdAsync(statusName)).Value;

            try
            {
                List<Ticket> tickets = await _context.Projects
                                                     .Where(p => p.CompanyId == companyId)
                                                     .SelectMany(p => p.Tickets)
                                                        .Include(t => t.Attachments)
                                                        .Include(t => t.Comments)
                                                        .Include(t => t.DeveloperUser)
                                                        .Include(t => t.History)
                                                        .Include(t => t.OwnerUser)
                                                        .Include(t => t.TicketPriority)
                                                        .Include(t => t.TicketStatus)
                                                        .Include(t => t.TicketType)
                                                        .Include(t => t.Project)
                                                .Where(t => t.TicketStatusId == statusId)
                                                .ToListAsync();
                return tickets;
            }
            catch (Exception)
            {
                throw;
            }
        }

		#endregion

		#region Get All Tickets By Type Async
		public async Task<List<Ticket>> GetAllTicketsByTypeAsync(int companyId, string typeName)
        {
            int typeId = (await LookupTicketTypeIdAsync(typeName)).Value;

            try
            {
                List<Ticket> tickets = await _context.Projects
                                                     .Where(p => p.CompanyId == companyId)
                                                     .SelectMany(p => p.Tickets)
                                                         .Include(t => t.Attachments)
                                                         .Include(t => t.Comments)
                                                         .Include(t => t.DeveloperUser)
                                                         .Include(t => t.History)
                                                         .Include(t => t.OwnerUser)
                                                         .Include(t => t.TicketPriority)
                                                         .Include(t => t.TicketStatus)
                                                         .Include(t => t.TicketType)
                                                         .Include(t => t.Project)
                                                    .Where(t => t.TicketTypeId == typeId)
                                                    .ToListAsync();
                return tickets;
            }
            catch (Exception)
            {
                throw;
            }
        }

		#endregion

		#region Get Archived Tickets Async
		public async Task<List<Ticket>> GetArchivedTicketsAsync(int companyId)
        {
            List<Ticket> tickets = new();

            try
            {
                tickets = (await GetAllTicketsByCompanyAsync(companyId))
                                                .Where(t => t.Archived == true).ToList();

                return tickets;
            }
            catch (Exception)
            {
                throw;
            }
        }

		#endregion

		#region Get Project Tickets By Priority Async
		public async Task<List<Ticket>> GetProjectTicketsByPriorityAsync(string priorityName, int companyId, int projectId)
        {
            List<Ticket> tickets = new();

            try
            {
                tickets = (await GetAllTicketsByPriorityAsync(companyId, priorityName)).Where(t => t.ProjectId == projectId).ToList();
                return tickets;
            }
            catch (Exception)
            {
                throw;
            }
        }

		#endregion

		#region Get Project Tickets By Role Async
		public async Task<List<Ticket>> GetProjectTicketsByRoleAsync(string role, string userId, int projectId, int companyId)
        {
            List<Ticket> tickets = new();

            try
            {
                tickets = (await GetTicketsByRoleAsync(role, userId, companyId)).Where(t => t.ProjectId == projectId).ToList();
                return tickets;
            }
            catch (Exception)
            {
                throw;
            }

        }

        #endregion

        #region Get Project Tickets By Status Async
        public async Task<List<Ticket>> GetProjectTicketsByStatusAsync(string statusName, int companyId, int projectId)
        {
            List<Ticket> tickets = new();

            try
            {
                tickets = (await GetAllTicketsByStatusAsync(companyId, statusName)).Where(t => t.ProjectId == projectId).ToList();

                return tickets;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Get Project Tickets By Type Async
        public async Task<List<Ticket>> GetProjectTicketsByTypeAsync(string typeName, int companyId, int projectId)
        {
            List<Ticket> tickets = new();

            try
            {
                tickets = (await GetAllTicketsByTypeAsync(companyId, typeName)).Where(t => t.ProjectId == projectId).ToList();
                return tickets;
            }
            catch (Exception)
            {
                throw;
            }

        }

        #endregion

        #region Get Ticket As No Tracking
        public async Task<Ticket> GetTicketAsNoTrackingAsync(int ticketId)
        {
            try
            {
                return await _context.Tickets
                                     .Include(t => t.DeveloperUser)
                                     .Include(t => t.Project)
                                     .Include(t => t.TicketPriority)
                                     .Include(t => t.TicketStatus)
                                     .Include(t => t.TicketType)
                                     .AsNoTracking()
                                     .FirstOrDefaultAsync(t => t.Id == ticketId);

            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

        public async Task<TicketAttachment> GetTicketAttachmentByIdAsync(int ticketAttachmentId)
		{
			try
			{
				TicketAttachment ticketAttachment = await _context.TicketAttachments
																  .Include(t => t.User)
																  .FirstOrDefaultAsync(t => t.Id == ticketAttachmentId);
				return ticketAttachment;
			}
			catch (Exception)
			{

				throw;
			}
		}

        public async Task<Ticket> GetTicketByIdAsync(int ticketId)
        {
            try
            {
                return await _context.Tickets
                                     .Include(t => t.DeveloperUser)
                                     .Include(t => t.OwnerUser)
                                     .Include(t => t.Project)
                                     .Include(t => t.TicketPriority)
                                     .Include(t => t.TicketStatus)
                                     .Include(t => t.TicketType)
									 .Include(t => t.Comments)
									 .Include(t => t.Attachments)
									 .Include(t => t.History)
									 .FirstOrDefaultAsync(t => t.Id == ticketId);
            }
            catch (Exception ex)
            {
                throw;
            }

        }

		public async Task<BTUser> GetTicketDeveloperAsync(int ticketId, int companyId)
        {
            BTUser developer = new();

            try
            {
                Ticket ticket = (await GetAllTicketsByCompanyAsync(companyId)).FirstOrDefault(t => t.Id == ticketId);

                if(ticket?.DeveloperUserId != null)
                {
                    // Developer Object is returned in GetAllTicketsByCompanyAsync Method
                    developer = ticket.DeveloperUser;
                }
                
                return developer;
            }
            catch(Exception)
            {
                throw;
            }

        }

        public async Task<List<Ticket>> GetTicketsByRoleAsync(string role, string userId, int companyId)
        {
            List<Ticket> tickets = new();

            try
            {
                // Admin has access to all tickets
                if(role == Roles.Admin.ToString())
                {
                    tickets = await GetAllTicketsByCompanyAsync(companyId);
                }
                // Developer has access to owned tickets
                else if(role == Roles.Developer.ToString())
                {
                    tickets = (await GetAllTicketsByCompanyAsync(companyId)).Where(t=>t.DeveloperUserId == userId).ToList();
                }
                // Submitter has access to tickets they submitted(own)
                else if (role == Roles.Submitter.ToString())
                {
                    tickets = (await GetAllTicketsByCompanyAsync(companyId)).Where(t => t.OwnerUserId == userId).ToList();
                }
                else if (role == Roles.ProjectManager.ToString())
                {
                    tickets = await GetTicketsByUserIdAsync(userId, companyId);
                }

                return tickets;
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        public async Task<List<Ticket>> GetTicketsByUserIdAsync(string userId, int companyId)
        {
            BTUser? btUser = await _context.Users.FirstOrDefaultAsync(u=>u.Id == userId);
            List<Ticket> tickets = new();

            try 
            {
                if (await _rolesService.IsUserInRoleAsync(btUser, Roles.Admin.ToString()))
                {
                    tickets = (await _projectService.GetAllProjectsByCompanyAsync(companyId)).SelectMany(p=>p.Tickets).ToList();
                }
                else if (await _rolesService.IsUserInRoleAsync(btUser, Roles.Developer.ToString()))
                {
                    tickets = (await _projectService.GetAllProjectsByCompanyAsync(companyId))
                                                    .SelectMany(p => p.Tickets).Where(t => t.DeveloperUserId == userId).ToList();
                }
                else if (await _rolesService.IsUserInRoleAsync(btUser, Roles.Submitter.ToString()))
                {
                    tickets = (await _projectService.GetAllProjectsByCompanyAsync(companyId))
                                                    .SelectMany(p => p.Tickets).Where(t => t.OwnerUserId == userId).ToList();
                }
                else if (await _rolesService.IsUserInRoleAsync(btUser, Roles.ProjectManager.ToString()))
                {
                    // GetUserProjects includes all ticket information fields
                    tickets = (await _projectService.GetUserProjectsAsync(userId)).SelectMany(p => p.Tickets).ToList();
                }

                return tickets;
            }
            catch (Exception)
            {
                throw;
            }


        }

        #region Get Unassigned Tickets
        public async Task<List<Ticket>> GetUnassignedTicketsAsync(int companyId)
        {
            List<Ticket> tickets = new();

            try
            {
                tickets = (await GetAllTicketsByCompanyAsync(companyId)).Where(t => string.IsNullOrEmpty(t.DeveloperUserId)).ToList();
                return tickets;
            }
            catch (Exception)
            {
                throw;
            }

        }

        #endregion

        #region Lookup Ticket Priority Id Async
        public async Task<int?> LookupTicketPriorityIdAsync(string priorityName)
        {
            try
            {
                TicketPriority priority = await _context.TicketPriorities.FirstOrDefaultAsync(p => p.Name == priorityName);
                return priority?.Id;
            }
            catch (Exception)
            {
                throw;
            }
        }

		#endregion

		#region Lookup Ticket Status Id Async
		public async Task<int?> LookupTicketStatusIdAsync(string statusName)

	        {
            try
            {
                TicketStatus status = await _context.TicketsStatuses.FirstOrDefaultAsync(s => s.Name == statusName);
                return status?.Id;
            }
            catch(Exception)
            {
                throw;
            }
        }
		#endregion

		#region Lookup Ticket Type Id Async
		public async Task<int?> LookupTicketTypeIdAsync(string typeName)
        {
            try
            {
                TicketType type = await _context.TicketTypes.FirstOrDefaultAsync(t => t.Name == typeName);
                return type?.Id;
            }
            catch (Exception)
            {
                throw;
            }
        }

		#endregion

		#region Restore Ticket Async
		public async Task RestoreTicketAsync(Ticket ticket) 
        {
            try
            {
                ticket.Archived = false;
                _context.Update(ticket);
                await _context.SaveChangesAsync();

            }
            catch (Exception)
            {
                throw;
            }
        }
		#endregion

		#region Update Ticket Async
		public async Task UpdateTicketAsync(Ticket ticket)
        {
            try
            {
                _context.Update(ticket);
                await _context.SaveChangesAsync();

            }
            catch (Exception)
            {
                throw;
            }

        }

        #endregion

	}
}
