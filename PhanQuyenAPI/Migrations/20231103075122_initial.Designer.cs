﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PhanQuyenAPI.Models;

#nullable disable

namespace PhanQuyenAPI.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20231103075122_initial")]
    partial class initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("PhanQuyenAPI.Entity.Accounts", b =>
                {
                    b.Property<int>("AccountsID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AccountsID"));

                    b.Property<int>("DecentralizationID")
                        .HasColumnType("int");

                    b.Property<int>("DecentralizationsID")
                        .HasColumnType("int");

                    b.Property<string>("ResetPasswordToken")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ResetPasswordTokenExpiry")
                        .HasColumnType("datetime2");

                    b.Property<string>("avatar")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("createAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("updateAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("userName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("AccountsID");

                    b.HasIndex("DecentralizationsID");

                    b.ToTable("Account");
                });

            modelBuilder.Entity("PhanQuyenAPI.Entity.Decentralizations", b =>
                {
                    b.Property<int>("DecentralizationsID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("DecentralizationsID"));

                    b.Property<string>("AuthorityName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("createAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("updateAt")
                        .HasColumnType("datetime2");

                    b.HasKey("DecentralizationsID");

                    b.ToTable("Decentralization");
                });

            modelBuilder.Entity("PhanQuyenAPI.Entity.Accounts", b =>
                {
                    b.HasOne("PhanQuyenAPI.Entity.Decentralizations", "decentralizations")
                        .WithMany()
                        .HasForeignKey("DecentralizationsID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("decentralizations");
                });
#pragma warning restore 612, 618
        }
    }
}
