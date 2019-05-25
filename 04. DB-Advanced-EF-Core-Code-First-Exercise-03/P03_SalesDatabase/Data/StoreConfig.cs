﻿using System;
using System.Collections.Generic;
using System.Text;

namespace P03_SalesDatabase.Data
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using P03_SalesDatabase.Data.Models;

    public class StoreConfig : IEntityTypeConfiguration<Store>
    {
        public void Configure(EntityTypeBuilder<Store> builder)
        {
            builder.HasKey(s => s.StoreId);

            builder.Property(p => p.Name)
                .HasMaxLength(80)
                .IsUnicode();
        }
    }
}
