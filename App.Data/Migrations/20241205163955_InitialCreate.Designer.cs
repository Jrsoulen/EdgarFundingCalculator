﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace EdgarRepo.Migrations
{
    [DbContext(typeof(CompanyContext))]
    [Migration("20241205163955_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.0");

            modelBuilder.Entity("EdgarCompanyInfo", b =>
                {
                    b.Property<int>("Cik")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("EntityName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Frame")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Cik");

                    b.ToTable("EdgarCompanies");
                });
#pragma warning restore 612, 618
        }
    }
}
