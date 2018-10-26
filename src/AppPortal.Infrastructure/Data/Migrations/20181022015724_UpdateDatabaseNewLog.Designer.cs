﻿// <auto-generated />
using System;
using AppPortal.Infrastructure.App;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AppPortal.Infrastructure.Data.Migrations
{
    [DbContext(typeof(AppDataContext))]
    [Migration("20181022015724_UpdateDatabaseNewLog")]
    partial class UpdateDatabaseNewLog
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.3-rtm-32065")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("AppPortal.Core.Entities.Address", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("LatLong")
                        .HasMaxLength(100);

                    b.Property<string>("Name")
                        .HasMaxLength(255);

                    b.Property<int?>("ProvinceId");

                    b.Property<string>("ProvinceType")
                        .HasMaxLength(11);

                    b.HasKey("Id");

                    b.ToTable("Addresses","AppPortal");
                });

            modelBuilder.Entity("AppPortal.Core.Entities.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("IsShow");

                    b.Property<string>("LinkFooter")
                        .HasMaxLength(255);

                    b.Property<string>("LinkHeader")
                        .HasMaxLength(255);

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

                    b.Property<int?>("OrderSort");

                    b.Property<int?>("ParentId");

                    b.Property<int>("Position")
                        .HasColumnType("int");

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

            modelBuilder.Entity("AppPortal.Core.Entities.FriendShip", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("FriendId")
                        .HasMaxLength(500);

                    b.Property<int>("FriendShipStatus")
                        .HasColumnType("int");

                    b.Property<string>("UserId")
                        .HasMaxLength(500);

                    b.HasKey("Id");

                    b.ToTable("FriendShips","AppPortal");
                });

            modelBuilder.Entity("AppPortal.Core.Entities.News", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Abstract")
                        .HasMaxLength(2000);

                    b.Property<int?>("AddressId");

                    b.Property<string>("AddressString");

                    b.Property<int?>("CategoryId");

                    b.Property<string>("Content")
                        .HasColumnType("ntext");

                    b.Property<int?>("CountLike");

                    b.Property<int?>("CountView");

                    b.Property<string>("Image");

                    b.Property<int>("IsNew")
                        .HasColumnType("int");

                    b.Property<int>("IsPosition")
                        .HasColumnType("int");

                    b.Property<bool>("IsShow")
                        .HasColumnType("bit");

                    b.Property<int>("IsStatus")
                        .HasColumnType("int");

                    b.Property<int>("IsType")
                        .HasColumnType("int");

                    b.Property<int>("IsView")
                        .HasColumnType("int");

                    b.Property<string>("Lat");

                    b.Property<string>("Link")
                        .HasMaxLength(500);

                    b.Property<string>("Lng");

                    b.Property<string>("MetaDescription")
                        .HasMaxLength(1000);

                    b.Property<string>("MetaKeywords")
                        .HasMaxLength(1000);

                    b.Property<string>("MetaTitle")
                        .HasMaxLength(1000);

                    b.Property<string>("Name")
                        .HasMaxLength(2000);

                    b.Property<string>("Note")
                        .HasMaxLength(1000);

                    b.Property<DateTime?>("OnCreated");

                    b.Property<DateTime?>("OnDeleted");

                    b.Property<DateTime?>("OnPublished");

                    b.Property<DateTime?>("OnUpdated");

                    b.Property<string>("Sename")
                        .HasMaxLength(500);

                    b.Property<string>("SourceNews")
                        .HasMaxLength(255);

                    b.Property<string>("UserAddress");

                    b.Property<string>("UserEmail")
                        .HasMaxLength(255);

                    b.Property<string>("UserFullName")
                        .HasMaxLength(255);

                    b.Property<string>("UserId")
                        .HasMaxLength(500);

                    b.Property<string>("UserName")
                        .HasMaxLength(255);

                    b.Property<string>("UserPhone");

                    b.HasKey("Id");

                    b.HasIndex("AddressId");

                    b.HasIndex("CategoryId");

                    b.ToTable("News","AppPortal");
                });

            modelBuilder.Entity("AppPortal.Core.Entities.NewsCategory", b =>
                {
                    b.Property<int?>("NewsId");

                    b.Property<int?>("CategoryId");

                    b.HasKey("NewsId", "CategoryId");

                    b.HasIndex("CategoryId");

                    b.ToTable("NewsCategories","AppPortal");
                });

            modelBuilder.Entity("AppPortal.Core.Entities.NewsLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AttachFile");

                    b.Property<string>("GroupNameFrom");

                    b.Property<string>("GroupNameTo");

                    b.Property<int?>("NewsId");

                    b.Property<string>("Note")
                        .HasColumnType("ntext");

                    b.Property<DateTime?>("OnCreated");

                    b.Property<string>("UserName");

                    b.Property<string>("UserNameTo");

                    b.Property<int>("type_status");

                    b.HasKey("Id");

                    b.ToTable("NewsLog","AppPortal");
                });

            modelBuilder.Entity("AppPortal.Core.Entities.NewsRelated", b =>
                {
                    b.Property<int>("NewsId1");

                    b.Property<int>("NewsId2");

                    b.HasKey("NewsId1", "NewsId2");

                    b.HasIndex("NewsId1", "NewsId2");

                    b.ToTable("NewsRelateds","AppPortal");
                });

            modelBuilder.Entity("AppPortal.Core.Entities.Notifications", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("NewsId");

                    b.Property<string>("Notification");

                    b.Property<DateTime?>("OnCreated");

                    b.Property<string>("UserName");

                    b.Property<bool>("isRead");

                    b.HasKey("Id");

                    b.ToTable("Notifications","AppPortal");
                });

            modelBuilder.Entity("AppPortal.Core.Entities.ReportNews", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("NewsId");

                    b.Property<DateTime?>("OnCreated");

                    b.Property<string>("UserName");

                    b.Property<string>("data");

                    b.HasKey("Id");

                    b.ToTable("ReportNews","AppPortal");
                });

            modelBuilder.Entity("AppPortal.Core.Entities.News", b =>
                {
                    b.HasOne("AppPortal.Core.Entities.Address", "Address")
                        .WithMany("News")
                        .HasForeignKey("AddressId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AppPortal.Core.Entities.Category", "Category")
                        .WithMany("News")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AppPortal.Core.Entities.NewsCategory", b =>
                {
                    b.HasOne("AppPortal.Core.Entities.Category", "Categories")
                        .WithMany("NewsCategories")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("AppPortal.Core.Entities.News", "News")
                        .WithMany("NewsCategories")
                        .HasForeignKey("NewsId")
                        .OnDelete(DeleteBehavior.Restrict);
                });
#pragma warning restore 612, 618
        }
    }
}
