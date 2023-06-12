using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using VisitorLog.Server.Models;
using VisitorLog.Server.Utils;

namespace VisitorLog.Server.Services
{
	public static class StaffServices
	{

		/// <summary>
		/// Genertate admin token
		/// </summary>
		/// <param name="staff">The admin details</param>
		/// <returns></returns>
		public static string GenerateAuthToken(this Staff staff)
		{
			var adminClaims = new Claim[]
			{
				new Claim("staffId", staff.Id.ToString()),
				new Claim("fullname", staff.FullName),
				new Claim("email", staff.Email),
				new Claim("phoneNumber", staff.PhoneNumber),
				new Claim("address", staff.Address),
				new Claim("gender", staff.Gender),
				new Claim("position", staff.Position),
				new Claim("dob", staff.Dob.ToString()),
			};

			return adminClaims.GenerateToken();
		}

		public async static Task<StaffCredentials> GetStaffCredentials(this DbSet<Staff> staff, int staffId, ApplicationDbContext dbContext = null)
		{
			var currentStaff = await staff.FindAsync(staffId);

			return new StaffCredentials
			{
				Id = currentStaff.Id.ToString(),
				FullName = currentStaff.FullName,
				Email = currentStaff.Email,
				Address = currentStaff.Address,
				Dob = currentStaff.Dob,
				Gender = currentStaff.Gender,
				PhoneNumber = currentStaff.PhoneNumber,
				Position = currentStaff.Position,
			};
		}

		public class StaffCredentials
		{
			public string Id { get; set; }
			public string FullName { get; set; }
			public string Email { get; set; }
			public string PhoneNumber { get; set; }
			public string Gender { get; set; }
			public string Position { get; set; }
			public string Address { get; set; }
			public DateTime? Dob { get; set; }
		}


	}
}
