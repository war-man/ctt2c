﻿// <auto-generated />
using AppPortal.Core.Entities;
using AppPortal.Infrastructure.App;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace AppPortal.Infrastructure.Data.Migrations
{
    [DbContext(typeof(AppDataContext))]
    [Migration("20180701173322_InitAppPortalModel")]
    partial class InitAppPortalModel
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.3-rtm-10026")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("AppPortal.Core.Entities.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("IsShow");

                    b.Property<string>("LinkFooter");

                    b.Property<string>("LinkHeader");

                    b.Property<string>("MetaDescription")
                        .HasMaxLength(1000);

                    b.Property<string>("MetaKeywords")
                        .HasMaxLength(1000);

                    b.Property<string>("MetaTitle")
                        .HasMaxLength(1000);

                    b.Property<string>("Name")
                        .HasMaxLength(255);

                    b.Property<DateTime?>("OnCreated");

                    b.Property<DateTime?>("OnDeleted");

                    b.Property<DateTime?>("OnPublished");

                    b.Property<DateTime?>("OnUpdated");

                    b.Property<int>("OrderSort");

                    b.Property<int?>("ParentId");

                    b.Property<string>("Sename")
                        .HasMaxLength(500);

                    b.Property<bool>("ShowChild");

                    b.Property<int>("ShowType")
                        .HasColumnType("int");

                    b.Property<string>("TargetUrl")
                        .HasMaxLength(500);

                    b.HasKey("Id");

                    b.ToTable("Categories","AppPortal");
                });
#pragma warning restore 612, 618
        }
    }
}
