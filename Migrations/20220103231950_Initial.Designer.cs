// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TFT_Stats.Data;

namespace TFT_Stats.Migrations
{
    [DbContext(typeof(TFTDbContext))]
    [Migration("20220103231950_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.13")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("TFT_Stats.Models.Companion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("RiotCompanionID")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SkinID")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Companions");
                });

            modelBuilder.Entity("TFT_Stats.Models.Match", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("RiotMatchID")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("SummonersId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("SummonersId");

                    b.ToTable("Matches");
                });

            modelBuilder.Entity("TFT_Stats.Models.Participant", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int?>("CompanionId")
                        .HasColumnType("int");

                    b.Property<int>("MatchID")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CompanionId");

                    b.HasIndex("MatchID");

                    b.ToTable("Participants");
                });

            modelBuilder.Entity("TFT_Stats.Models.Summoners", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Puuid")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RiotSummonerId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Summoners");
                });

            modelBuilder.Entity("TFT_Stats.Models.Match", b =>
                {
                    b.HasOne("TFT_Stats.Models.Summoners", null)
                        .WithMany("Matches")
                        .HasForeignKey("SummonersId");
                });

            modelBuilder.Entity("TFT_Stats.Models.Participant", b =>
                {
                    b.HasOne("TFT_Stats.Models.Companion", "Companion")
                        .WithMany()
                        .HasForeignKey("CompanionId");

                    b.HasOne("TFT_Stats.Models.Match", "Match")
                        .WithMany("Participants")
                        .HasForeignKey("MatchID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Companion");

                    b.Navigation("Match");
                });

            modelBuilder.Entity("TFT_Stats.Models.Match", b =>
                {
                    b.Navigation("Participants");
                });

            modelBuilder.Entity("TFT_Stats.Models.Summoners", b =>
                {
                    b.Navigation("Matches");
                });
#pragma warning restore 612, 618
        }
    }
}
