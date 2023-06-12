using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VisitorLog.Server.Models;

namespace VisitorLog.Server.Controllers.Admin
{
	[ApiController]
	[Route("api/admin/staff")]
	public partial class StaffController : ControllerBase
	{
		private ApplicationDbContext mDbContext;
		private ILogger<StaffController> mLogger;

		public StaffController(ApplicationDbContext dbContext, ILogger<StaffController> logger)
		{
			mDbContext = dbContext;
			mLogger = logger;
		}
	}
}
