﻿// <auto-generated />
using System;
using InntalerSchachfreunde;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace InntalerSchachfreunde.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("InntalerSchachfreunde.Entities.Article", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Headline")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("ReleaseDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Text")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Articles");
                });

            modelBuilder.Entity("InntalerSchachfreunde.Entities.Game", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateOnly?>("Date")
                        .HasColumnType("date");

                    b.Property<int>("PlayerBlackId")
                        .HasColumnType("int");

                    b.Property<int>("PlayerWhiteId")
                        .HasColumnType("int");

                    b.Property<double?>("PointsWhite")
                        .HasColumnType("double");

                    b.Property<int>("TournamentId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PlayerBlackId");

                    b.HasIndex("PlayerWhiteId");

                    b.HasIndex("TournamentId");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("InntalerSchachfreunde.Entities.Image", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("ArticleId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("longtext");

                    b.Property<byte[]>("ImageBytes")
                        .IsRequired()
                        .HasColumnType("longblob");

                    b.Property<string>("Name")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("ArticleId");

                    b.ToTable("Images");
                });

            modelBuilder.Entity("InntalerSchachfreunde.Entities.KeyValue", b =>
                {
                    b.Property<string>("Key")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Key");

                    b.ToTable("KeyValues");
                });

            modelBuilder.Entity("InntalerSchachfreunde.Entities.Player", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("InntalerSchachfreunde.Entities.PlayerTournament", b =>
                {
                    b.Property<int>("PlayerId")
                        .HasColumnType("int");

                    b.Property<int>("TournamentId")
                        .HasColumnType("int");

                    b.HasKey("PlayerId", "TournamentId");

                    b.HasIndex("TournamentId");

                    b.ToTable("PlayerTournament");
                });

            modelBuilder.Entity("InntalerSchachfreunde.Entities.Termin", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Annotation")
                        .HasColumnType("longtext");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("datetime(6)");

                    b.Property<bool?>("IsCanceled")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Termins");
                });

            modelBuilder.Entity("InntalerSchachfreunde.Entities.Tournament", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Tournaments");
                });

            modelBuilder.Entity("InntalerSchachfreunde.Entities.Game", b =>
                {
                    b.HasOne("InntalerSchachfreunde.Entities.Player", "PlayerBlack")
                        .WithMany()
                        .HasForeignKey("PlayerBlackId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("InntalerSchachfreunde.Entities.Player", "PlayerWhite")
                        .WithMany("Games")
                        .HasForeignKey("PlayerWhiteId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("InntalerSchachfreunde.Entities.Tournament", "Tournament")
                        .WithMany("Games")
                        .HasForeignKey("TournamentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PlayerBlack");

                    b.Navigation("PlayerWhite");

                    b.Navigation("Tournament");
                });

            modelBuilder.Entity("InntalerSchachfreunde.Entities.Image", b =>
                {
                    b.HasOne("InntalerSchachfreunde.Entities.Article", "Article")
                        .WithMany("Images")
                        .HasForeignKey("ArticleId");

                    b.Navigation("Article");
                });

            modelBuilder.Entity("InntalerSchachfreunde.Entities.PlayerTournament", b =>
                {
                    b.HasOne("InntalerSchachfreunde.Entities.Player", "Player")
                        .WithMany("PlayerTournaments")
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("InntalerSchachfreunde.Entities.Tournament", "Tournament")
                        .WithMany("PlayerTournaments")
                        .HasForeignKey("TournamentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Player");

                    b.Navigation("Tournament");
                });

            modelBuilder.Entity("InntalerSchachfreunde.Entities.Article", b =>
                {
                    b.Navigation("Images");
                });

            modelBuilder.Entity("InntalerSchachfreunde.Entities.Player", b =>
                {
                    b.Navigation("Games");

                    b.Navigation("PlayerTournaments");
                });

            modelBuilder.Entity("InntalerSchachfreunde.Entities.Tournament", b =>
                {
                    b.Navigation("Games");

                    b.Navigation("PlayerTournaments");
                });
#pragma warning restore 612, 618
        }
    }
}
