using System;
using System.Collections.Generic;
using System.Text;

namespace P01_HospitalDatabase.Data
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using P01_HospitalDatabase.Data.Models;

    public class DoctorConfig : IEntityTypeConfiguration<Doctor>
    {
        public void Configure(EntityTypeBuilder<Doctor> builder)
        {
            builder.HasKey(d => d.DoctorId);

            builder.Property(d => d.Name)
                .HasMaxLength(100)
                .IsUnicode();

            builder.Property(d => d.Specialty)
                .HasMaxLength(100)
                .IsUnicode();
        }
    }
}
