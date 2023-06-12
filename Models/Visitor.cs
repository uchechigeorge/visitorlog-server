using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VisitorLog.Server.Models
{
	public class Visitor
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		public string Name { get; set; }

		public DateTime? VDate { get; set; }

		public string Purpose { get; set; }

		public int? StaffId { get; set; }

		public string PhoneNumber { get; set; }
		
		public string Email { get; set; }

		public string Gender { get; set; }

		public DateTime DateModified { get; set; }

		public DateTime DateCreated { get; set; }

		public Staff Staff { get; set; }
	}

	public class VisitorConfiguration : IEntityTypeConfiguration<Visitor>
	{
		public void Configure(EntityTypeBuilder<Visitor> builder)
		{
			builder.Property(p => p.DateModified)
				.HasDefaultValueSql("getutcdate()");

			builder.Property(p => p.DateCreated)
				.HasDefaultValueSql("getutcdate()");

			builder.HasOne(p => p.Staff)
				.WithMany(s => s.Visitors)
				.OnDelete(DeleteBehavior.NoAction);
		}
	}


}
