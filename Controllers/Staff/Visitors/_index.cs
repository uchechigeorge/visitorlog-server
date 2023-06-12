using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VisitorLog.Server.Models;

namespace VisitorLog.Server.Controllers.Staff
{
	[ApiController]
	[Route("api/staff/visitors")]
	public partial class VisitorController : ControllerBase
	{
		private readonly ApplicationDbContext mDbContext;
		private readonly ILogger<VisitorController> mLogger;

		public VisitorController(ApplicationDbContext dbContext, ILogger<VisitorController> logger)
		{
			mDbContext = dbContext;
			mLogger = logger;
		}
	}
}
