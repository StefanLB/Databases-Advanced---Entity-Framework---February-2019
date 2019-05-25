using System;
using System.Collections.Generic;
using System.Text;

namespace P01_HospitalDatabase.Data
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using P01_HospitalDatabase.Data.Models;

    public class VisitationConfig : IEntityTypeConfiguration<Visitation>
    {
        public void Configure(EntityTypeBuilder<Visitation> builder)
        {
            builder.HasKey(v => v.VisitationId);

            builder.Property(v => v.Comments)
                .HasMaxLength(250)
                .IsUnicode(true);

            builder.HasOne(v => v.Patient)
                .WithMany(p => p.Visitations)
                .HasForeignKey(v => v.PatientId);

            builder.HasOne(d => d.Doctor)
                .WithMany(v => v.Visitations)
                .HasForeignKey(d => d.DoctorId);
        }
    }
}
