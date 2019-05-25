using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using P01_StudentSystem.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace P01_StudentSystem.Data.EntityConfigurations
{
    public class CourseConfig : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            builder.HasKey(s => s.CourseId);

            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(80)
                .IsUnicode();

            builder.Property(s => s.Description)
                .IsRequired(false)
                .IsUnicode();
        }

    }
}
