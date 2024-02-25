using BugTracks.Models;
using BugTracks.Services.Interfaces;

namespace BugTracks.Services
{
    public class BTTicketService : IBTTicketService
    {
        Task IBTTicketService.AddNewTicketAsync(Ticket ticket)
        {
            throw new NotImplementedException();
        }

        Task IBTTicketService.ArchiveTicketAsync(Ticket ticket)
        {
            throw new NotImplementedException();
        }

        Task IBTTicketService.AssignTicketAsync(int ticketId, string userId)
        {
            throw new NotImplementedException();
        }

        Task<List<Ticket>> IBTTicketService.GetAllTicketsByCompanyAsync(int companyId)
        {
            throw new NotImplementedException();
        }

        Task<List<Ticket>> IBTTicketService.GetAllTicketsByPriorityAsync(int companyId, string priorityName)
        {
            throw new NotImplementedException();
        }

        Task<List<Ticket>> IBTTicketService.GetAllTicketsByStatusAsync(int companyId, string statusName)
        {
            throw new NotImplementedException();
        }

        Task<List<Ticket>> IBTTicketService.GetAllTicketsByTypeAsync(int companyId, string typeName)
        {
            throw new NotImplementedException();
        }

        Task<List<Ticket>> IBTTicketService.GetArchivedTicketsAsync(int companyId)
        {
            throw new NotImplementedException();
        }

        Task<List<Ticket>> IBTTicketService.GetProjectTicketsByPriorityAsync(string priorityName, int companyId, int projectId)
        {
            throw new NotImplementedException();
        }

        Task<List<Ticket>> IBTTicketService.GetProjectTicketsByRoleAsync(string role, string userId, int projectId, int companyId)
        {
            throw new NotImplementedException();
        }

        Task<List<Ticket>> IBTTicketService.GetProjectTicketsByStatusAsync(string statusName, int companyId, int projectId)
        {
            throw new NotImplementedException();
        }

        Task<List<Ticket>> IBTTicketService.GetProjectTicketsByTypeAsync(string typeName, int companyId, int projectId)
        {
            throw new NotImplementedException();
        }

        Task<Ticket> IBTTicketService.GetTicketByIdAsync(int ticketId)
        {
            throw new NotImplementedException();
        }

        Task<BTUser> IBTTicketService.GetTicketDeveloperAsync(int ticketId)
        {
            throw new NotImplementedException();
        }

        Task<List<Ticket>> IBTTicketService.GetTicketsByRoleAsync(string role, string userId, int companyId)
        {
            throw new NotImplementedException();
        }

        Task<List<Ticket>> IBTTicketService.GetTicketsByUserIdAsync(string userId, int companyId)
        {
            throw new NotImplementedException();
        }

        Task<int?> IBTTicketService.LookupTicketPriorityIdAsync(string priorityName)
        {
            throw new NotImplementedException();
        }

        Task<int?> IBTTicketService.LookupTicketStatusIdAsync(string statusName)
        {
            throw new NotImplementedException();
        }

        Task<int?> IBTTicketService.LookupTicketTypeIdAsync(string typeName)
        {
            throw new NotImplementedException();
        }

        Task IBTTicketService.UpdateTicketAsync(Ticket ticket)
        {
            throw new NotImplementedException();
        }
    }
}
