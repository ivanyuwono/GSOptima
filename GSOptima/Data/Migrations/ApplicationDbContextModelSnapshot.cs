﻿// <auto-generated />
using GSOptima.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace GSOptima.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.2-rtm-10011")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("GSOptima.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("Address");

                    b.Property<DateTime>("BirthDate");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<DateTime>("EndDate");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("MembershipType");

                    b.Property<string>("Name");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<DateTime>("StartDate");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("GSOptima.Models.Stock", b =>
                {
                    b.Property<string>("StockID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("StockID");

                    b.ToTable("Stock");
                });

            modelBuilder.Entity("GSOptima.Models.StockPrice", b =>
                {
                    b.Property<string>("StockID");

                    b.Property<DateTime>("Date");

                    b.Property<string>("Action");

                    b.Property<decimal?>("AverageBigWave");

                    b.Property<decimal?>("BBLower");

                    b.Property<decimal?>("BBUpper");

                    b.Property<decimal?>("BigWave");

                    b.Property<decimal>("Close");

                    b.Property<decimal?>("EMA12");

                    b.Property<decimal?>("EMA26");

                    b.Property<long?>("Frequency");

                    b.Property<decimal?>("GSLine");

                    b.Property<string>("GSLineDirection");

                    b.Property<decimal>("High");

                    b.Property<decimal?>("Highest12Months");

                    b.Property<decimal?>("Highest3Months");

                    b.Property<decimal?>("Highest6Months");

                    b.Property<decimal>("Low");

                    b.Property<decimal?>("Lowest12Months");

                    b.Property<decimal?>("Lowest3Months");

                    b.Property<decimal?>("Lowest6Months");

                    b.Property<decimal?>("MA20");

                    b.Property<decimal?>("MA60");

                    b.Property<decimal?>("MACD");

                    b.Property<decimal>("Open");

                    b.Property<decimal?>("Resistance");

                    b.Property<decimal?>("SD20");

                    b.Property<decimal?>("SignalLine");

                    b.Property<decimal?>("Support");

                    b.Property<int?>("TrendHigh");

                    b.Property<int?>("TrendLow");

                    b.Property<long>("Volume");

                    b.HasKey("StockID", "Date");

                    b.ToTable("StockPrice");
                });

            modelBuilder.Entity("GSOptima.Models.StockWatchList", b =>
                {
                    b.Property<string>("ApplicationUserId");

                    b.Property<string>("StockID");

                    b.HasKey("ApplicationUserId", "StockID");

                    b.HasIndex("StockID");

                    b.ToTable("StockWatchList");
                });

            modelBuilder.Entity("GSOptima.Models.GSProAdminWatchList", b =>
            {
                b.Property<string>("StockID");

                b.Property<decimal?>("Target1");

                b.Property<decimal?>("Target2");

                b.HasKey("StockID");

                b.HasIndex("StockID");

                b.ToTable("GSProAdminWatchList");
            });

            


            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("GSOptima.Models.StockPrice", b =>
                {
                    b.HasOne("GSOptima.Models.Stock", "Stocks")
                        .WithMany("Prices")
                        .HasForeignKey("StockID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GSOptima.Models.StockWatchList", b =>
                {
                    b.HasOne("GSOptima.Models.ApplicationUser", "User")
                        .WithMany("StockWatchList")
                        .HasForeignKey("ApplicationUserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GSOptima.Models.Stock", "Stock")
                        .WithMany("StockWatchList")
                        .HasForeignKey("StockID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GSOptima.Models.GSProAdminWatchList", b =>
            {
                b.HasOne("GSOptima.Models.Stock", "Stocks")
                    .WithMany("GSProAdminWatchList")
                    .HasForeignKey("StockID")
                    .OnDelete(DeleteBehavior.Cascade);
            });
            
            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("GSOptima.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("GSOptima.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GSOptima.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("GSOptima.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
