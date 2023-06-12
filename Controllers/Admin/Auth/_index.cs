using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VisitorLog.Server.Models;

namespace VisitorLog.Server.Controllers.Admin
{
	[ApiController]
	[Route("api/admin/auth")]
	public partial class AuthController : ControllerBase
	{
		private readonly ApplicationDbContext mDbContext;
		private readonly ILogger<AuthController> mLogger;

		public AuthController(ApplicationDbContext dbContext, ILogger<AuthController> logger)
		{
			mDbContext = dbContext;
			mLogger = logger;
		}
	}
}
