using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VisitorLog.Server.Models
{
	public class Staff
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		public string FullName { get; set; }

		public string Email { get; set; }

		public string Password { get; set; }

		public string Address { get; set; }

		public string Position { get; set; }

		public string PhoneNumber { get; set; }
		
		public string Gender { get; set; }

		public string Degree { get; set; }
		
		public DateTime? Dob { get; set; }

		public DateTime DateModified { get; set; }

		public DateTime DateCreated { get; set; }

		public ICollection<Visitor> Visitors { get; set; }
	}

	public class StaffConfiguration : IEntityTypeConfiguration<Staff>
	{
		public void Configure(EntityTypeBuilder<Staff> builder)
		{
			builder.Property(p => p.DateModified)
				.HasDefaultValueSql("getutcdate()");

			builder.Property(p => p.DateCreated)
				.HasDefaultValueSql("getutcdate()");
		}
	}

}
