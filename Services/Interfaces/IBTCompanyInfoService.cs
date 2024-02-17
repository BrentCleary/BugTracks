using BugTracks.Models;

namespace BugTracks.Services.Interfaces
{
    public interface IBTCompanyInfoService
    {
        public Task<Company> GetCompanyInfoByIdAsync(int? companyId);

        public Task<List<BTUser>> GetAllMembersAsync(int companyId);
    
        public Task<List<Project>> GetAllProjectsAsync(int companyId);
    
        public Task<List<Ticket>> GetAllTicketsAsync(int companyId);

    }
}
