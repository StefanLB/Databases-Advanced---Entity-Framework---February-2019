using System;
using System.Collections.Generic;
using System.Text;

namespace P01_HospitalDatabase.Data
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using P01_HospitalDatabase.Data.Models;

    public class MedicamentConfig : IEntityTypeConfiguration<Medicament>
    {
        public void Configure(EntityTypeBuilder<Medicament> builder)
        {
            builder.HasKey(m => m.MedicamentId);

            builder.Property(m => m.Name)
                .HasMaxLength(50)
                .IsUnicode(true);
        }
    }
}
