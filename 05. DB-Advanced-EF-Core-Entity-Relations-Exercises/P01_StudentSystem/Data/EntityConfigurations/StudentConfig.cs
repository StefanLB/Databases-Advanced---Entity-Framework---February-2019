using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using P01_StudentSystem.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace P01_StudentSystem.Data.EntityConfigurations
{
    public class StudentConfig : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.HasKey(s=>s.StudentId);

            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode();

            builder.Property(s => s.PhoneNumber)
                .IsRequired(false)
                .IsUnicode(false)
                .HasMaxLength(10);

            builder.Property(s => s.Birthday)
                .IsRequired(false);
        }
    }
}
