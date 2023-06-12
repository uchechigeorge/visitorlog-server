using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VisitorLog.Server.Models
{
	public class Admin
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		public string Name { get; set; }

		public string Username { get; set; }

		public string Password { get; set; }

		public AdminAccess AccessId { get; set; }

		public DateTime DateModified { get; set; }

		public DateTime DateCreated { get; set; }

		[NotMapped]
		public string Access { get => AccessId.ToString(); }
	}

	public class AdminConfiguration : IEntityTypeConfiguration<Admin>
	{
		public void Configure(EntityTypeBuilder<Admin> builder)
		{
			builder.Property(p => p.DateModified)
				.HasDefaultValueSql("getutcdate()");

			builder.Property(p => p.DateCreated)
				.HasDefaultValueSql("getutcdate()");
		}
	}

	public enum AdminAccess
	{
		Full = 0,
		ViewOnly = 1,
	}

}
