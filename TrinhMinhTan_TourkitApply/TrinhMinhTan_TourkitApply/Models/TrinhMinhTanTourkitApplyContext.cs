using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TrinhMinhTan_TourkitApply.Models;

public partial class TrinhMinhTanTourkitApplyContext : DbContext
{
    public TrinhMinhTanTourkitApplyContext()
    {
    }

    public TrinhMinhTanTourkitApplyContext(DbContextOptions<TrinhMinhTanTourkitApplyContext> options)
        : base(options)
    {
    }

    public virtual DbSet<LoaiSanPham> LoaiSanPhams { get; set; }

    public virtual DbSet<SanPham> SanPhams { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(config.GetConnectionString("MyConStr"));
        }
    }


//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Server=(local);uid=sa;pwd=admin;database=TrinhMinhTan_TourkitApply; TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LoaiSanPham>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__LoaiSanP__3214EC073B4D78C5");

            entity.ToTable("LoaiSanPham");

            entity.HasIndex(e => e.Ten, "UQ__LoaiSanP__C451FA83939C970B").IsUnique();

            entity.Property(e => e.NgayNhap).HasColumnType("datetime");
            entity.Property(e => e.Ten).HasMaxLength(255);
        });

        modelBuilder.Entity<SanPham>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SanPham__3214EC07E75C0DD0");

            entity.ToTable("SanPham");

            entity.Property(e => e.Gia).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.NgayNhap).HasColumnType("datetime");
            entity.Property(e => e.Ten).HasMaxLength(255);

            entity.HasMany(d => d.LoaiSanPhams).WithMany(p => p.SanPhams)
                .UsingEntity<Dictionary<string, object>>(
                    "SanPhamLoaiSanPham",
                    r => r.HasOne<LoaiSanPham>().WithMany()
                        .HasForeignKey("LoaiSanPhamId")
                        .HasConstraintName("FK_LoaiSanPham"),
                    l => l.HasOne<SanPham>().WithMany()
                        .HasForeignKey("SanPhamId")
                        .HasConstraintName("FK_SanPham"),
                    j =>
                    {
                        j.HasKey("SanPhamId", "LoaiSanPhamId").HasName("PK__SanPham___4FE4721A4D0BB65B");
                        j.ToTable("SanPham_LoaiSanPham");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
