using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BugTracks.Data;
using BugTracks.Models;
using BugTracks.Extensions;
using BugTracks.Models.ViewModels;
using BugTracks.Services.Interfaces;
using BugTracks.Models.Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Identity;
using BugTracks.Services;
using Microsoft.AspNetCore.Authorization;

namespace BugTracks.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private readonly IBTRolesService _rolesService;
        private readonly IBTLookupService _lookupService;
        private readonly IBTFileService _fileService;
        private readonly IBTProjectService _projectService;
        private readonly UserManager<BTUser> _userManager;
        private readonly IBTCompanyInfoService _companyInfoService;

        public ProjectsController(IBTRolesService rolesService,
                                  IBTLookupService lookupService,
                                  IBTFileService fileService,
                                  IBTProjectService projectService,
                                  UserManager<BTUser> userManager,
                                  IBTCompanyInfoService companyInfoService)
        {
            _rolesService = rolesService;
            _lookupService = lookupService;
            _fileService = fileService;
            _projectService = projectService;
            _userManager = userManager;
            _companyInfoService = companyInfoService;
        }

        public async Task<IActionResult> MyProjects()
        {
            string userId = _userManager.GetUserId(User);

            List<Project> projects = await _projectService.GetUserProjectsAsync(userId);

            return View(projects);
        }



        // All Projects
        public async Task<IActionResult> AllProjects()
        {

            List<Project> projects = new();

            int companyId = User.Identity.GetCompanyId().Value;
            
            if(User.IsInRole(nameof(Roles.Admin)) || User.IsInRole(nameof(Roles.ProjectManager)))
            {
                projects = await _companyInfoService.GetAllProjectsAsync(companyId);
            }
            else
            {
                projects = await _projectService.GetAllProjectsByCompanyAsync(companyId);
            }

            return View(projects);
        }



        // Archived Projects
        public async Task<IActionResult> ArchivedProjects()
        {
            int companyId = User.Identity.GetCompanyId().Value;

            List<Project> projects = await _projectService.GetArchivedProjectsByCompanyAsync(companyId);

            return View(projects);
        }



        // Unassigned Projects
        [Authorize(Roles="Admin")]
        public async Task<IActionResult> UnassignedProjects()
        {
            int companyId = User.Identity.GetCompanyId().Value;

            List<Project> projects = new();

            projects = await _projectService.GetUnassignedProjectsAsync(companyId);

            return View(projects);
        }



        // GET: Assign PM
        [Authorize(Roles="Admin")]
        [HttpGet]
        public async Task<IActionResult> AssignPM(int projectId)
        {
            int companyId = User.Identity.GetCompanyId().Value;

            AssignPMViewModel model = new();

            model.Project = await _projectService.GetProjectByIdAsync(projectId, companyId);
            model.PMList = new SelectList(await _rolesService.GetUsersInRoleAsync(nameof(Roles.ProjectManager), companyId), "Id", "FullName");

            return View(model);

        }

        //POST: Assign PM
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignPM(AssignPMViewModel model)
        {
            if (!string.IsNullOrEmpty(model.PMID))
            {
                await _projectService.AddProjectManagerAsync(model.PMID, model.Project.Id);

                return RedirectToAction(nameof(Details), new { id = model.Project.Id });
            }

            return RedirectToAction(nameof(AssignPM), new { projectId = model.Project.Id });
        }



        // GET: Assign Members
        [Authorize(Roles = "Admin, ProjectManager")]
        [HttpGet]
        public async Task<IActionResult> AssignMembers(int id)
        {
            int companyId = User.Identity.GetCompanyId().Value; 

            ProjectMembersViewModel model = new ();

            model.Project = await _projectService.GetProjectByIdAsync(id, companyId);

            List<BTUser> developers = await _rolesService.GetUsersNotInRoleAsync(nameof(Roles.Developer), companyId);
            List<BTUser> submitters = await _rolesService.GetUsersNotInRoleAsync(nameof(Roles.Submitter), companyId);

            List<BTUser> companyMembers = developers.Concat(submitters).ToList();

            List<string> projectMembers = model.Project.Members.Select(m => m.Id).ToList();

            model.Users = new MultiSelectList(companyMembers,"Id", "FullName", projectMembers);

            return View(model);
        }

        [Authorize(Roles = "Admin, ProjectManager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignMembers(ProjectMembersViewModel model)
        {
            if (model.SelectedUsers != null)
            {
                List<string> memberIds = (await _projectService.GetAllProjectMembersExceptPMAsync(model.Project.Id))
                                                               .Select(m=>m.Id).ToList();

                // Remove current members
                foreach(string member in memberIds)
                {
                    await _projectService.RemoveUserFromProjectAsync(member, model.Project.Id);
                }

                // Add current members
                foreach (string member in model.SelectedUsers)
                {
                    await _projectService.AddUserToProjectAsync(member, model.Project.Id);
                }

                return RedirectToAction("Details", "Projects", new { id = model.Project.Id });
            }

            return RedirectToAction(nameof(AssignMembers), new { id = model.Project.Id });
        }



        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int companyId = User.Identity.GetCompanyId().Value;

            Project project = await _projectService.GetProjectByIdAsync(id.Value, companyId);

            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }



        // GET: Projects/Create
        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<IActionResult> Create()
        {
            // Get CompanyId
            int companyId = User.Identity.GetCompanyId().Value;

            //Add ViewModel instance "AddProjectWithPMViewModel
            AddProjectWithPMViewModel model = new();

            // Load SelectLists with data ie. PMList & PriorityList
            model.PMList = new SelectList( await _rolesService.GetUsersInRoleAsync(Roles.ProjectManager.ToString(), companyId), "Id", "FullName");
            model.PriorityList = new SelectList( await _lookupService.GetProjectPrioritiesAsync(), "Id", "Name");
            
            return View(model);
        }

        // POST: Projects/Create
        [Authorize(Roles = "Admin, ProjectManager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddProjectWithPMViewModel model)
        {
            if(model != null)
            {
                int companyId = User.Identity.GetCompanyId().Value;

                try
                {
                    if(model.Project.ImageFile != null)
                    {
                        model.Project.ImageFileData = await _fileService.ConvertFileToByteArrayAsync(model.Project.ImageFile);
                        model.Project.ImageFileName = model.Project.ImageFile.Name;
                        model.Project.ImageContentType = model.Project.ImageFile.ContentType;
                    }

                    model.Project.CompanyId = companyId;

                    await _projectService.AddNewProjectAsync(model.Project);

                    // Add PM if one was chosen
                    if(!string.IsNullOrEmpty(model.PmId))
                    {
                        await _projectService.AddProjectManagerAsync(model.PmId, model.Project.Id);
                    }

                }
                catch (Exception)
                {

                    throw;
                }

                // TODO: Redirect to All Projects
                return RedirectToAction("AllProjects");
            }

            return RedirectToAction("Create");
        }



        // GET: Projects/Edit/5
        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<IActionResult> Edit(int? id)
        {
            // Get CompanyId
            int companyId = User.Identity.GetCompanyId().Value;

            //Add ViewModel instance "AddProjectWithPMViewModel
            AddProjectWithPMViewModel model = new();

            model.Project = await _projectService.GetProjectByIdAsync(id.Value, companyId);
                                                   

            // Load SelectLists with data ie. PMList & PriorityList
            model.PMList = new SelectList(await _rolesService.GetUsersInRoleAsync(Roles.ProjectManager.ToString(), companyId), "Id", "FullName");
            model.PriorityList = new SelectList(await _lookupService.GetProjectPrioritiesAsync(), "Id", "Name");

            return View(model);
        }

        // POST: Projects/Edit/5
        [Authorize(Roles = "Admin, ProjectManager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AddProjectWithPMViewModel model)
        {
            if (model != null)
            {
                try
                {
                    if (model.Project.ImageFile != null)
                    {
                        model.Project.ImageFileData = await _fileService.ConvertFileToByteArrayAsync(model.Project.ImageFile);
                        model.Project.ImageFileName = model.Project.ImageFile.Name;
                        model.Project.ImageContentType = model.Project.ImageFile.ContentType;
                    }

                    await _projectService.UpdateProjectAsync(model.Project);

                    // Add PM if one was chosen
                    if (!string.IsNullOrEmpty(model.PmId))
                    {
                        await _projectService.AddProjectManagerAsync(model.PmId, model.Project.Id);
                    }

                    return RedirectToAction(actionName: "AllProjects");

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await ProjectExists(model.Project.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

            }
            return RedirectToAction("Edit");

        }



        // GET: Projects/Archive/5
        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<IActionResult> Archive(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int companyId = User.Identity.GetCompanyId().Value;
            var project = await _projectService.GetProjectByIdAsync(id.Value, companyId);


            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Archive/5
        [Authorize(Roles = "Admin, ProjectManager")]
        [HttpPost, ActionName("Archive")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ArchiveConfirmed(int id)
        {
            int companyId = User.Identity.GetCompanyId().Value;

            var project = await _projectService.GetProjectByIdAsync(id, companyId);
            await _projectService.ArchiveProjectAsync(project);

            return RedirectToAction(nameof(AllProjects));
        }



        // GET: Projects/Restore/5
        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<IActionResult> Restore(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int companyId = User.Identity.GetCompanyId().Value;
            var project = await _projectService.GetProjectByIdAsync(id.Value, companyId);


            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Restore/5
        [HttpPost, ActionName("Restore")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreConfirmed(int id)
        {
            int companyId = User.Identity.GetCompanyId().Value;

            var project = await _projectService.GetProjectByIdAsync(id, companyId);
            await _projectService.RestoreProjectAsync(project);

            return RedirectToAction(nameof(AllProjects));
        }



        private async Task<bool> ProjectExists(int id)
        {
            int companyId = User.Identity.GetCompanyId().Value;

            return (await _projectService.GetAllProjectsByCompanyAsync(companyId)).Any(p => p.Id == id);
        }
    }
}
