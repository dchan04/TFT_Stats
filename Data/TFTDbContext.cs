﻿using Microsoft.EntityFrameworkCore;
using TFT_Stats.Models;
using System.Configuration;
using Microsoft.Extensions.Configuration;

namespace TFT_Stats.Data
{
    public class TFTDbContext: DbContext
    {
        protected readonly IConfiguration Configuration;
        public TFTDbContext(DbContextOptions<TFTDbContext> options, IConfiguration configuration) : base(options)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(Configuration.GetConnectionString("TFT_Database"));
        }

        public DbSet<Summoners> Summoners { get; set; }

        public DbSet<Participant> Participants { get; set; }

        public DbSet<Match> Matches { get; set; }

        public DbSet<Companion> Companions { get; set; }
    }
}