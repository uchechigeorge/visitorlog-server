using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VisitorLog.Server.Models;

namespace VisitorLog.Server.Controllers.Admin
{
	[ApiController]
	[Route("api/admin/admins")]
	public partial class AdminController : ControllerBase
	{
		private ApplicationDbContext mDbContext;
		private ILogger<AdminController> mLogger;

		public AdminController(ApplicationDbContext dbContext, ILogger<AdminController> logger)
		{
			mDbContext = dbContext;
			mLogger = logger;
		}
	}
}
