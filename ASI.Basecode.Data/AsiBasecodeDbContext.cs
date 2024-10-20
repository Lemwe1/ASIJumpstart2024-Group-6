﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using ASI.Basecode.Data.Models;

namespace ASI.Basecode.Data
{
    public partial class AsiBasecodeDbContext : DbContext
    {
        public AsiBasecodeDbContext()
        {
        }

        public AsiBasecodeDbContext(DbContextOptions<AsiBasecodeDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<MCategory> MCategories { get; set; }
        public virtual DbSet<MDebitLiab> MDebitLiabs { get; set; }
        public virtual DbSet<MRole> MRoles { get; set; }
        public virtual DbSet<MTransaction> MTransactions { get; set; }
        public virtual DbSet<MUser> MUsers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=localhost;Database=AsiBasecodeDb;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MCategory>(entity =>
            {
                entity.HasKey(e => e.CategoryId)
                    .HasName("PK__M_Catego__19093A0B4546A1B0");

                entity.ToTable("M_Category");

                entity.Property(e => e.Color).IsRequired();

                entity.Property(e => e.Icon).IsRequired();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Type).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.MCategories)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MCategory_MUser_UserId");
            });

            modelBuilder.Entity<MDebitLiab>(entity =>
            {
                entity.HasKey(e => e.DeLiId)
                    .HasName("PK__M_DebitL__B2BE78F2AA1F2BD2");

                entity.ToTable("M_DebitLiab");

                entity.Property(e => e.DeLiBalance).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.DeLiColor)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.DeLiDue).HasColumnType("datetime");

                entity.Property(e => e.DeLiHapp).HasColumnType("datetime");

                entity.Property(e => e.DeLiIcon)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.DeLiName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.DeLiType)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.MDebitLiabs)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_M_DebitLiab_UserId");
            });

            modelBuilder.Entity<MRole>(entity =>
            {
                entity.HasKey(e => e.RoleId);

                entity.ToTable("M_ROLE");

                entity.Property(e => e.RoleId).ValueGeneratedNever();

                entity.Property(e => e.RoleName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<MTransaction>(entity =>
            {
                entity.HasKey(e => e.TransactionId)
                    .HasName("PK__M_Transa__55433A6B99DD4BCE");

                entity.ToTable("M_Transaction");

                entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.Note).HasMaxLength(255);

                entity.Property(e => e.TransactionDate).HasColumnType("date");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.MTransactions)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__M_Transac__Categ__07C12930");

                entity.HasOne(d => d.DeLi)
                    .WithMany(p => p.MTransactions)
                    .HasForeignKey(d => d.DeLiId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__M_Transac__DeLiI__08B54D69");
            });

            modelBuilder.Entity<MUser>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.ToTable("M_User");

                entity.Property(e => e.UserId).ValueGeneratedNever();

                entity.Property(e => e.FirstName).HasMaxLength(50);

                entity.Property(e => e.FirstNameKana).HasMaxLength(50);

                entity.Property(e => e.InsBy)
                    .HasMaxLength(256)
                    .IsUnicode(false);

                entity.Property(e => e.InsDt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.LastName).HasMaxLength(50);

                entity.Property(e => e.LastNameKana).HasMaxLength(50);

                entity.Property(e => e.Mail)
                    .HasMaxLength(256)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(512)
                    .IsUnicode(false);

                entity.Property(e => e.PasswordResetExpiration).HasColumnType("datetime");

                entity.Property(e => e.PasswordResetToken).HasMaxLength(1000);

                entity.Property(e => e.Remarks).HasMaxLength(500);

                entity.Property(e => e.TemporaryPassword)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UpdBy)
                    .HasMaxLength(256)
                    .IsUnicode(false);

                entity.Property(e => e.UpdDt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UserCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
