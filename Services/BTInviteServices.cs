using BugTracks.Data;
using BugTracks.Models;
using BugTracks.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using NuGet.Common;

namespace BugTracks.Services

{
    public class BTInviteServices : IBTInviteService
    {
        private readonly ApplicationDbContext _context;

        public BTInviteServices(ApplicationDbContext contex)
        {
            _context = contex;
        }

        public async Task<bool> AcceptInviteAsync(Guid? token, string userId, int companyId)
        {
            Invite invite = await _context.Invites.FirstOrDefaultAsync(i=>i.CompanyToken == token);

            if(invite == null)
            {
                return false;
            }

            try
            {
                invite.IsValid = false;
                invite.InviteeId = userId;
                await _context.SaveChangesAsync();

                return true;
            }
            catch(Exception)
            {
                throw;
            }
        }


        public async Task AddNewInviteAsync(Invite invite)
        {
            try
            {
                await _context.AddAsync(invite);
                await _context.SaveChangesAsync();
            }
            catch
            {
                throw;
            }

        }

        public async Task<bool> AnyInviteAsync(Guid token, string email, int companyId)
        {
            try
            {
                bool result = await _context.Invites.Where(i => i.CompanyId == companyId)
                                                    .AnyAsync(i => i.CompanyToken == token && i.InviteeEmail == email);

                return result;
                
            }
            catch(Exception)
            {
                throw;
            }


        }

        public async Task<Invite> GetInviteAsync(int inviteId, int companyId)
        {
            try
            {
                Invite invite = await _context.Invites.Where(i => i.CompanyId == companyId)
                                                      .Include(i => i.Company)
                                                      .Include(i => i.Project)
                                                      .Include(i => i.Invitor)
                                                      .FirstOrDefaultAsync(i=>i.Id == inviteId);

                return invite;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Invite> GetInviteAsync(Guid token, string email, int companyId)
        {
            try
            {
                Invite invite = await _context.Invites.Where(i => i.CompanyId == companyId)
                                                      .Include(i => i.Company)
                                                      .Include(i => i.Project)
                                                      .Include(i => i.Invitor)
                                                      .FirstOrDefaultAsync(i => i.CompanyToken == token && i.InviteeEmail == email);

                return invite;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Task<bool> ValidateInviteCodeAsync(Guid? token)
        {
            throw new NotImplementedException();
        }
    }
}
