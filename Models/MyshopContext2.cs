using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ShopWebApplication1.Models;

public partial class MyshopContext2 : DbContext
{
    public MyshopContext2()
    {
    }

    public MyshopContext2(DbContextOptions<MyshopContext2> options)
        : base(options)
    {
    }

    public virtual DbSet<Baskett> Basketts { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Good> Goods { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderGood> OrderGoods { get; set; }

    public virtual DbSet<OrderStatus> OrderStatuses { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server= DESKTOP-HA39RFV\\SQLEXPRESS; Database=myshop; Trusted_Connection=True; Trust Server Certificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Baskett>(entity =>
        {
            entity.ToTable("Baskett");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.GoodsId).HasColumnName("goods_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Goods).WithMany(p => p.Basketts)
                .HasForeignKey(d => d.GoodsId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Baskett_Goods");

            entity.HasOne(d => d.User).WithMany(p => p.Basketts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Baskett_Users");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Category");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Good>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Barcode)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("barcode");
            entity.Property(e => e.CatId).HasColumnName("cat_id");
            entity.Property(e => e.Descrip).HasColumnName("descrip");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Price)
                .HasColumnType("smallmoney")
                .HasColumnName("price");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Cat).WithMany(p => p.Goods)
                .HasForeignKey(d => d.CatId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Goods_Category");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Orders2");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Data).HasColumnName("data");
            entity.Property(e => e.StateId).HasColumnName("state_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.State).WithMany(p => p.Orders)
                .HasForeignKey(d => d.StateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_Order_status");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_Users");
        });

        modelBuilder.Entity<OrderGood>(entity =>
        {
            entity.ToTable("Order_goods");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.GoodId).HasColumnName("good_id");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.Quatity).HasColumnName("quatity");

            entity.HasOne(d => d.Good).WithMany(p => p.OrderGoods)
                .HasForeignKey(d => d.GoodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_goods_Goods");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderGoods)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_goods_Orders");
        });

        modelBuilder.Entity<OrderStatus>(entity =>
        {
            entity.ToTable("Order_status");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Patronymic)
                .HasMaxLength(50)
                .HasColumnName("patronymic");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("phone_number");
            entity.Property(e => e.Surname)
                .HasMaxLength(50)
                .HasColumnName("surname");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
