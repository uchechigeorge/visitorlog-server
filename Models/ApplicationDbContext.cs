using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace VisitorLog.Server.Models
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions options) : base(options)
		{

		}

		public DbSet<Admin> Admins { get; set; }
		public DbSet<Staff> Staff { get; set; }
		public DbSet<Visitor> Visitors { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.EnableSensitiveDataLogging();
		}
	}
}
