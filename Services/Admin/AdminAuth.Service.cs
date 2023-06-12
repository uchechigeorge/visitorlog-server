using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using VisitorLog.Server.Models;
using VisitorLog.Server.Utils;

namespace VisitorLog.Server.Services
{
	public static class AdminServices
	{

		/// <summary>
		/// Genertate admin token
		/// </summary>
		/// <param name="admin">The admin details</param>
		/// <returns></returns>
		public async static Task<string> GenerateAuthToken(this DbSet<Admin> source, int adminId)
		{
			var admin = await source.FindAsync(adminId);

			var adminClaims = new Claim[]
			{
				new Claim("adminId", admin.Id.ToString()),
				new Claim("username", admin.Username),
				new Claim("name", admin.Name),
			};

			return adminClaims.GenerateToken();
		}
		
		/// <summary>
		/// Genertate admin token
		/// </summary>
		/// <param name="admin">The admin details</param>
		/// <returns></returns>
		public static string GenerateAuthToken(this Admin admin)
		{
			var adminClaims = new Claim[]
			{
				new Claim("adminId", admin.Id.ToString()),
				new Claim("username", admin.Username),
				new Claim("name", admin.Name),
			};

			return adminClaims.GenerateToken();
		}

		public async static Task<AdminCredentials> GetAdminCredentials(this DbSet<Admin> admin, int adminId, ApplicationDbContext dbContext = null)
		{
			var currentAdmin = await admin.FindAsync(adminId);
			var noOfAdmins = await dbContext.Admins.CountAsync();
			var noOfStaff = await dbContext.Staff.CountAsync();
			var noOfVisitors = await dbContext.Visitors.CountAsync();

			return new AdminCredentials
			{
				Id = currentAdmin.Id.ToString(),
				Name = currentAdmin.Name,
				Username = currentAdmin.Username,
				NoOfAdmins = noOfAdmins,
				NoOfStaff = noOfStaff,
				NoOfVisitors = noOfVisitors,
			};
		}

		public class AdminCredentials
		{
			public string Id { get; set; }
			public string Name { get; set; }
			public string Username { get; set; }
			public int NoOfAdmins { get; set; }
			public int NoOfStaff { get; set; }
			public int NoOfVisitors { get; set; }
		}


	}
}
