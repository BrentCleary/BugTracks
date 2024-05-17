using BugTracks.Extensions;
using BugTracks.Models;
using BugTracks.Models.ChartModels;
using BugTracks.Models.Enums;
using BugTracks.Models.ViewModels;
using BugTracks.Services;
using BugTracks.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BugTracks.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBTCompanyInfoService _companyInfoService;
        private readonly IBTProjectService _projectService;

		public HomeController(ILogger<HomeController> logger,
							  IBTCompanyInfoService companyInfoService,
							  IBTProjectService projectService)
		{
			_logger = logger;
			_companyInfoService = companyInfoService;
			_projectService = projectService;
		}

		// GET: Index View
		public IActionResult Index()
        {
            return View();
        }



		// GET: Dashboard View
        public async Task<IActionResult> Dashboard()
        {
            DashboardViewModel model = new DashboardViewModel();
            int companyId = User.Identity.GetCompanyId().Value;

            model.Company = await _companyInfoService.GetCompanyInfoByIdAsync(companyId);
            model.Projects = (await _companyInfoService.GetAllProjectsAsync(companyId)).Where(p => p.Archived == false).ToList();
            model.Tickets = model.Projects.SelectMany(p => p.Tickets).Where(t=>t.Archived == false).ToList();
            model.Members = model.Company.Members.ToList();

            return View(model);
        }


		// ------------- Charts --------------
        // POST: Ggl Project Tickets
		[HttpPost]
		public async Task<JsonResult> GglProjectTickets()
		{
			int companyId = User.Identity.GetCompanyId().Value;

			List<Project> projects = await _projectService.GetAllProjectsByCompanyAsync(companyId);

			List<object> chartData = new();
			chartData.Add(new object[] { "ProjectName", "TicketCount" });

			foreach (Project prj in projects)
			{
				chartData.Add(new object[] { prj.Name, prj.Tickets.Count() });
			}

			return Json(chartData);
		}

		// POST: Ggl Project Priority
		[HttpPost]
		public async Task<JsonResult> GglProjectPriority()
		{
			int companyId = User.Identity.GetCompanyId().Value;

			List<Project> projects = await _projectService.GetAllProjectsByCompanyAsync(companyId);

			List<object> chartData = new();
			chartData.Add(new object[] { "Priority", "Count" });


			foreach (string priority in Enum.GetNames(typeof(BTProjectPriority)))
			{
				int priorityCount = (await _projectService.GetAllProjectsByPriorityAsync(companyId, priority)).Count();
				chartData.Add(new object[] { priority, priorityCount });
			}

			return Json(chartData);
		}

		// POST: AmCharts
		[HttpPost]
		public async Task<JsonResult> AmCharts()
		{
			AmChartData amChartData = new();
			List<AmItem> amItems = new();

			int companyId = User.Identity.GetCompanyId().Value;

			List<Project> projects = (await _companyInfoService.GetAllProjectsAsync(companyId)).Where(p => p.Archived == false).ToList();

			foreach (Project project in projects)
			{
				AmItem item = new();

				item.Project = project.Name;
				item.Tickets = project.Tickets.Count;
				item.Developers = (await _projectService.GetProjectMembersByRoleAsync(project.Id, nameof(Roles.Developer))).Count();

				amItems.Add(item);
			}

			amChartData.Data = amItems.ToArray();


			return Json(amChartData.Data);
		}



		public IActionResult Privacy()
        {
            return View();
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
