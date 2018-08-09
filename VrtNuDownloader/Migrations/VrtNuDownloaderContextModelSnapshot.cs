﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VrtNuDownloader.Models.Sqlite;

namespace VrtNuDownloader.Migrations
{
    [DbContext(typeof(VrtNuDownloaderContext))]
    partial class VrtNuDownloaderContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.1-rtm-30846");

            modelBuilder.Entity("VrtNuDownloader.Models.Sqlite.Downloaded", b =>
                {
                    b.Property<string>("Name")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DownloadDate");

                    b.Property<string>("EpisodeUrl");

                    b.Property<string>("VideoUrl");

                    b.HasKey("Name");

                    b.ToTable("Downloaded");
                });
#pragma warning restore 612, 618
        }
    }
}
