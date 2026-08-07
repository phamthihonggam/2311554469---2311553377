using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TourBookingAdvanced.Models;

public partial class TourBookingAdvancedDbContext : DbContext
{
    public TourBookingAdvancedDbContext()
    {
    }

    public TourBookingAdvancedDbContext(DbContextOptions<TourBookingAdvancedDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<BookingDetail> BookingDetails { get; set; }

    public virtual DbSet<Tour> Tours { get; set; }

    public virtual DbSet<TourCategory> TourCategories { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=LAPTOP-2C33T023\\MSSQLSERVER02;Database=TourBookingAdvancedDB;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Bookings__3214EC07E42BC298");

            entity.Property(e => e.BookingDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CustomerName).HasMaxLength(150);
            entity.Property(e => e.TotalAmount)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<BookingDetail>(entity =>
        {
            entity.HasKey(e => new { e.BookingId, e.TourId }).HasName("PK__BookingD__6591D44EBFC0F308");

            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingDetails)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK_Details_Bookings");

            entity.HasOne(d => d.Tour).WithMany(p => p.BookingDetails)
                .HasForeignKey(d => d.TourId)
                .HasConstraintName("FK_Details_Tours");
        });

        modelBuilder.Entity<Tour>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tours__3214EC07440C14E4");

            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TourName).HasMaxLength(200);

            entity.HasOne(d => d.Category).WithMany(p => p.Tours)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK_Tours_Categories");
        });

        modelBuilder.Entity<TourCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TourCate__3214EC071A05B367");

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
