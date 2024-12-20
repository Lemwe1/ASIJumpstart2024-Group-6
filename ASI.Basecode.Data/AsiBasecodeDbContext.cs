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

        public virtual DbSet<MBudget> MBudgets { get; set; }
        public virtual DbSet<MCategory> MCategories { get; set; }
        public virtual DbSet<MTransaction> MTransactions { get; set; }
        public virtual DbSet<MUser> MUsers { get; set; }
        public virtual DbSet<MWallet> MWallets { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=DESKTOP-90VPBGF\\SQLEXPRESS;Database=AsiBasecodeDb;Trusted_Connection=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MBudget>(entity =>
            {
                entity.HasKey(e => e.BudgetId)
                    .HasName("PK__M_Budget__E38E7924D6BD7748");

                entity.ToTable("M_Budgets");

                entity.Property(e => e.BudgetName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.LastResetDate).HasColumnType("datetime");

                entity.Property(e => e.MonthlyBudget).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.RemainingBudget).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.MBudgets)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__M_Budgets__Categ__0880433F");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.MBudgets)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__M_Budgets__UserI__09746778");
            });

            modelBuilder.Entity<MCategory>(entity =>
            {
                entity.HasKey(e => e.CategoryId)
                    .HasName("PK__M_Catego__19093A0B79D345B8");

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
                    .HasConstraintName("FK_MCategory_MUser_UserId");
            });

            modelBuilder.Entity<MTransaction>(entity =>
            {
                entity.HasKey(e => e.TransactionId)
                    .HasName("PK__M_Transa__55433A6BAC20D22F");

                entity.ToTable("M_Transaction");

                entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.Note).HasMaxLength(255);

                entity.Property(e => e.TransactionDate).HasColumnType("date");

                entity.Property(e => e.TransactionSort)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.TransactionType)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.MTransactions)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_category_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.MTransactions)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_user_id");

                entity.HasOne(d => d.Wallet)
                    .WithMany(p => p.MTransactions)
                    .HasForeignKey(d => d.WalletId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_wallet_id");
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

                entity.Property(e => e.VerificationToken).HasMaxLength(255);

                entity.Property(e => e.VerificationTokenExpiration).HasColumnType("datetime");
            });

            modelBuilder.Entity<MWallet>(entity =>
            {
                entity.HasKey(e => e.WalletId)
                    .HasName("PK__M_Wallet__84D4F90E28877477");

                entity.ToTable("M_Wallet");

                entity.Property(e => e.WalletBalance).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.WalletColor)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.WalletIcon)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.WalletName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.WalletOriginalBalance).HasColumnType("decimal(10, 2)");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.MWallets)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_M_Wallet_UserId");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
