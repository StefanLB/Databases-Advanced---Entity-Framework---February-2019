using System;
using System.Collections.Generic;
using System.Text;

namespace P01_HospitalDatabase.Data
{
    using P01_HospitalDatabase.Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class DiagnoseConfig : IEntityTypeConfiguration<Diagnose>
    {
        public void Configure(EntityTypeBuilder<Diagnose> builder)
        {
            builder.HasKey(d => d.DiagnoseId);

            builder.Property(d => d.Name)
                .HasMaxLength(50)
                .IsUnicode(true);

            builder.Property(d => d.Comments)
                .HasMaxLength(250)
                .IsUnicode(true);

            builder.HasOne(d => d.Patient)
                .WithMany(p => p.Diagnoses)
                .HasForeignKey(d => d.PatientId);
        }
    }
}
