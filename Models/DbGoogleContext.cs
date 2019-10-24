using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CalendarFive.Models
{
    public partial class DbGoogleContext : DbContext
    {
        public DbGoogleContext()
        {
        }

        public DbGoogleContext(DbContextOptions<DbGoogleContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Content> Content { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=UDAYA;Database=DbGoogle;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Content>(entity =>
            {
                entity.ToTable("content");

                entity.Property(e => e.Description)
                  .IsRequired()
                  .IsUnicode(false);

                entity.Property(e => e.EventId)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.End).HasColumnType("datetime");

                entity.Property(e => e.Start).HasColumnType("datetime");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .IsUnicode(false);
            });
        }
    }
}
