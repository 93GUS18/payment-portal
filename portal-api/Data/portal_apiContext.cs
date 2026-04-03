using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using portal_api.Models.DbModel;

namespace portal_api.Data
{
    public class portal_apiContext : DbContext
    {
        public portal_apiContext (DbContextOptions<portal_apiContext> options)
            : base(options)
        {
        }

        public DbSet<PaymentsDBModel> PaymentsDBModel { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Payments table
            modelBuilder.Entity<PaymentsDBModel>(entity =>
            {
                entity.ToTable("Payments");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Amount)
                    .IsRequired();

                entity.Property(e => e.Currency)
                    .IsRequired()
                    .HasConversion<int>();

                entity.Property(e => e.Reference)
                    .IsRequired()
                    .HasMaxLength(25);

                entity.Property(e => e.CreatedAt)
                    .IsRequired();

                entity.Property(e => e.UpdatedAt)
                    .IsRequired();

                entity.Property(e => e.IsDeleted)
                    .IsRequired()
                    .HasDefaultValue(false);
            });
        }
    }
}
