using BugTracks.Data;
using BugTracks.Models;
using BugTracks.Services.Interfaces;

namespace BugTracks.Services
{
    public class BTTicketHistoryService : IBTTicketHistoryService
    {
        private readonly ApplicationDbContext _context;

        public BTTicketHistoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddHistoryAsync(Ticket oldTicket, Ticket newTicket, string userId)
        {
            // NEW TICKET
            if(oldTicket == null && newTicket != null)
            {
                TicketHistory history = new()
                {
                    TicketId = newTicket.Id,
                    Property = "",
                    OldValue = "",
                    NewValue = "",
                    Created = DateTimeOffset.Now,
                    UserId = userId,
                    Description = "New Ticket Created"
                };

                try
                {
                    await _context.TicketHistories.AddAsync(history);
                    await _context.SaveChangesAsync();
                }
                catch(Exception)
                {
                    throw;
                }
            }
            else
            {
                if (oldTicket.Title != newTicket.Title)
                {
                    TicketHistory history = new()
                    {
                        TicketId = newTicket.Id,
                        Property = "Title",
                        OldValue = oldTicket.Title,
                        NewValue = newTicket.Title,
                        Created = DateTimeOffset.Now,
                        UserId = userId,
                        Description = $"New ticket title: {newTicket.Title}"
                    };
                    await _context.TicketHistories.AddAsync(history);
                }
        }

        public Task<List<TicketHistory>> GetCompanyTicketsHistoriesAsync(int companyId)
        {
            throw new NotImplementedException();
        }

        public Task<List<TicketHistory>> GetProjectTicketsHistoriesAsync(int projectId, int companyId)
        {
            throw new NotImplementedException();
        }
    }
}
