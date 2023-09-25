﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PatrickBotman.Persistence;

#nullable disable

namespace patrick_botman.Migrations
{
    [DbContext(typeof(GifRatingsContext))]
    partial class GifRatingsContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.11");

            modelBuilder.Entity("PatrickBotman.Persistence.Entities.Gif", b =>
                {
                    b.Property<int>("GifId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("GifUrl")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("GifId");

                    b.ToTable("Gifs");
                });

            modelBuilder.Entity("PatrickBotman.Persistence.Entities.GifRating", b =>
                {
                    b.Property<int>("GifRatingId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<long>("ChatId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("GifId")
                        .HasColumnType("INTEGER");

                    b.Property<long>("UserId")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Vote")
                        .HasColumnType("INTEGER");

                    b.HasKey("GifRatingId");

                    b.HasIndex("GifId");

                    b.ToTable("GifRatings");
                });

            modelBuilder.Entity("PatrickBotman.Persistence.Entities.GifRating", b =>
                {
                    b.HasOne("PatrickBotman.Persistence.Entities.Gif", null)
                        .WithMany("GifRatings")
                        .HasForeignKey("GifId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PatrickBotman.Persistence.Entities.Gif", b =>
                {
                    b.Navigation("GifRatings");
                });
#pragma warning restore 612, 618
        }
    }
}
