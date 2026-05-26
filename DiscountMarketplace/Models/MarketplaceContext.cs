using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace DiscountMarketplace.Models;

public partial class MarketplaceContext : DbContext
{
    public MarketplaceContext()
    {
    }

    public MarketplaceContext(DbContextOptions<MarketplaceContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Business> Businesses { get; set; }

    public virtual DbSet<Coupon> Coupons { get; set; }

    public virtual DbSet<Deal> Deals { get; set; }

    public virtual DbSet<Servicecategory> Servicecategories { get; set; }

    public virtual DbSet<User> Users { get; set; }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //    => optionsBuilder.UseMySql("server=localhost;database=DiscountMarketplace;user=root;password=admin", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.46-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Business>(entity =>
        {
            entity.HasKey(e => e.BusinessId).HasName("PRIMARY");

            entity.ToTable("businesses");

            entity.HasIndex(e => e.OwnerUserId, "FK_Businesses_Users");

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.LogoUrl).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(30);

            entity.HasOne(d => d.OwnerUser).WithMany(p => p.Businesses)
                .HasForeignKey(d => d.OwnerUserId)
                .HasConstraintName("FK_Businesses_Users");
        });

        modelBuilder.Entity<Coupon>(entity =>
        {
            entity.HasKey(e => e.CouponId).HasName("PRIMARY");

            entity.ToTable("coupons");

            entity.HasIndex(e => e.Code, "Code").IsUnique();

            entity.HasIndex(e => e.DealId, "FK_Coupons_Deals");

            entity.HasIndex(e => e.UserId, "FK_Coupons_Users");

            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.ExpirationDate).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'Active'")
                .HasColumnType("enum('Active','Used','Expired')");
            entity.Property(e => e.UsedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Deal).WithMany(p => p.Coupons)
                .HasForeignKey(d => d.DealId)
                .HasConstraintName("FK_Coupons_Deals");

            entity.HasOne(d => d.User).WithMany(p => p.Coupons)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Coupons_Users");
        });

        modelBuilder.Entity<Deal>(entity =>
        {
            entity.HasKey(e => e.DealId).HasName("PRIMARY");

            entity.ToTable("deals");

            entity.HasIndex(e => e.BusinessId, "FK_Deals_Businesses");

            entity.HasIndex(e => e.CategoryId, "FK_Deals_Categories");

            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.DiscountPrice).HasPrecision(10, 2);
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("'1'");
            entity.Property(e => e.OriginalPrice).HasPrecision(10, 2);
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.Business).WithMany(p => p.Deals)
                .HasForeignKey(d => d.BusinessId)
                .HasConstraintName("FK_Deals_Businesses");

            entity.HasOne(d => d.Category).WithMany(p => p.Deals)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Deals_Categories");
        });

        modelBuilder.Entity<Servicecategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PRIMARY");

            entity.ToTable("servicecategories");

            entity.HasIndex(e => e.Name, "Name").IsUnique();

            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.IconUrl).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(150);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PRIMARY");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "Email").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(30);
            entity.Property(e => e.Role)
                .HasDefaultValueSql("'Customer'")
                .HasColumnType("enum('Customer','BusinessOwner','Admin')");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
