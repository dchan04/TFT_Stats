using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.EntityFrameworkCore;
using MingweiSamuel.Camille;
using MingweiSamuel.Camille.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TFT_Stats.Data;
using TFT_Stats.Models;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace TFT_Stats.Services
{
    public class TFT_DataService : ITFT_DataService
    {
        private TFTDbContext _context;
        protected readonly IConfiguration Configuration;

        public TFT_DataService(TFTDbContext context)
        {
            _context = context;
        }

        /****************************** Finished Functions ******************************/
        public void GetAdditionalCompanionInfo()
        {
            var companions = _context.Companions.ToList();
            Console.WriteLine($"{companions.Count} companions");
            var json = new WebClient().DownloadString("https://raw.communitydragon.org/pbe/plugins/rcp-be-lol-game-data/global/default/v1/companions.json");
            dynamic jObj = JsonConvert.DeserializeObject<dynamic>(json);
            foreach (var companion in companions)
            {

                var updateCompanion = _context.Companions.SingleOrDefault(c => c.Id == companion.Id);
                string token = "$.[?(@.contentId == '" + companion.RiotCompanionID + "')]";
                JToken iconLocation = jObj.SelectToken(token);

                //Get companion values from JSON
                string imgLocation = (string)iconLocation["loadoutsIcon"];
                string companionName = (string)iconLocation["name"];
                string speciesName = (string)iconLocation["speciesName"];
                int level = (int)iconLocation["level"];

                string[] splitPath = imgLocation.Split("/");
                string pngName = splitPath[splitPath.Length - 1].ToLower();
                string path = "https://raw.communitydragon.org/latest/plugins/rcp-be-lol-game-data/global/default/assets/loadouts/companions/" + pngName;

                //Update Companion values in database
                updateCompanion.ImgPath = path;
                updateCompanion.Name = companionName;
                updateCompanion.Species = speciesName;
                updateCompanion.Level = level;

                Console.WriteLine("Updated!");
                //Console.WriteLine($"image file name: {path} - Name:{companionName} -- species: {speciesName} -- level: {level}");
            }
            _context.SaveChanges();
        }

        /****************************** Test Functions ******************************/
        /*
        public void DbUsage()
        {
            //Get all unique companion names
            var ListNames = _context.Companions
                .Select(c => c.Name)
                .Distinct()
                .ToList();


            //For each unique companion, get related data
            foreach (var name in ListNames)
            {
                //Get a list of companion entities using each unique name
                var CompanionList = _context.Companions
                    .Where(c => c.Name == name)
                    .OrderBy(c => c.Level)
                    .ToList();

                //Count 
                var count = _context.Companions.Count(c => c.Name == name);
                //List of levels
                var lvl1Count = _context.Companions.Where(c => c.Name == name && c.Level == 1).Count();
                var lvl2Count = _context.Companions.Where(c => c.Name == name && c.Level == 2).Count();
                var lvl3Count = _context.Companions.Where(c => c.Name == name && c.Level == 3).Count();

                //Species name
                var cSpecies = _context.Companions
                    .Where(_c => _c.Name == name)
                    .Select(c => new
                    {
                        species = c.Species,
                        imgPath = c.ImgPath,
                    })
                    .FirstOrDefault();

                Console.WriteLine($"{name} - {count} - {cSpecies.species} - {cSpecies.imgPath}");
                Console.WriteLine($"Levels: {lvl1Count} - {lvl2Count} - {lvl3Count}");
                Console.WriteLine("*************************************************************");
            }
        }
        */
        public void UpdateDB()
        {
            Companion newCompanion = new() { RiotCompanionID = "Test", SkinID = 11 };
            Companion testCompanion = _context.Companions.FirstOrDefault(c => c.RiotCompanionID == newCompanion.RiotCompanionID);
            if (testCompanion == null)
            {
                Console.WriteLine("DUPLICATE NOT FOUND!");
            }
            else
            {
                Console.WriteLine("DUPLICATE *FOUND*");
                _context.Entry(testCompanion).State = EntityState.Modified;
            }
            //_context.Companions.Add(newCompanion);
            //var order = _context.Companions.Where(_context.Companions.Where(_context.Companions == newCompanion).FirstorDefault());
            _context.SaveChanges();
        }

        //public void TestCompanionJson()
        //{
        //    var json = new WebClient().DownloadString("https://raw.communitydragon.org/pbe/plugins/rcp-be-lol-game-data/global/default/v1/companions.json");
        //    dynamic jObj = JsonConvert.DeserializeObject<dynamic>(json);
        //    string token = "$.[?(@.contentId == " + "'0e251d36-d86e-4c58-9b7f-bcee2376a408'" + ")]";
        //    JToken iconLocation = jObj.SelectToken(token);
        //    //Console.WriteLine($"{iconLocation["loadoutsIcon"]}");
        //    string imgLocation = (string)iconLocation["loadoutsIcon"];
        //    Console.WriteLine(imgLocation);
        //    string[] splitPath = imgLocation.Split("/");
        //    string pngName = splitPath[splitPath.Length - 1].ToLower();
        //    string path = "https://raw.communitydragon.org/latest/plugins/rcp-be-lol-game-data/global/default/assets/loadouts/companions/" + pngName;
        //    Console.WriteLine($"image file name: {path}");
        //}

        public void TestRiotApi()
        {
            var riotApi = RiotApi.NewInstance("RGAPI-e8e4c727-7723-4496-ad7a-c1a900043266");
            var tier = "DIAMOND";
            var division = "I";
            var entry = riotApi.TftLeagueV1.GetLeagueEntries(Region.NA, tier, division);

            List<String> summonerIdList = new();
            //List<String> puuidList = new();
            //List<String> matchList = new();
            //Console.WriteLine("Working on 1st function...");
            foreach (var item in entry)
            {
                //Console.WriteLine($"SummonerId: {item.SummonerId}");
                summonerIdList.Add(item.SummonerId.ToString());
            }

            //for each summoner in summoner list
            //foreach (var id in summonerIdList)
            for (int i = 0; i < 15; i++)
            {
                var puuid = riotApi.TftSummonerV1.GetBySummonerId(Region.NA, summonerIdList[i]);

                //Get 20 recent matches 
                var matches = riotApi.TftMatchV1.GetMatchIdsByPUUID(Region.Americas, puuid.Puuid);
                foreach (var matchId in matches)
                {
                    Match matchExist = _context.Matches.FirstOrDefault(m => m.RiotMatchID == matchId);
                    if (matchExist == null)
                    {
                        Console.WriteLine("NEW MATCH FOUND!");
                        Match newMatch = new()
                        {
                            RiotMatchID = matchId
                        };
                        //Add new match to database
                        _context.Matches.Add(newMatch);
                        Console.WriteLine($"Match added - {matchId}");

                        //Retrieve match information from Riot API
                        var match = riotApi.TftMatchV1.GetMatch(Region.Americas, matchId);
                        foreach (var participant in match.Info.Participants)
                        {
                            Companion newCompanion = new Companion() { RiotCompanionID = participant.Companion.ContentID, SkinID = participant.Companion.SkinID };
                            //Participant participant1 = new Participant() {Companion = newCompanion, MatchID = int.Parse(matchId)};
                            _context.Companions.Add(newCompanion);
                            //Console.WriteLine($"{participant.Companion}");
                            Console.WriteLine("Companion Added!");
                        }
                        _context.SaveChanges();
                    }
                    else
                    {
                        //Console.WriteLine("DUPLICATE *FOUND*");
                        _context.Entry(matchExist).State = EntityState.Modified;
                    }
                }
                //Summoners summoner = new Summoners() { RiotSummonerId = puuid.Id, Puuid = puuid.Puuid, Matches = null };
                //_context.Summoners.Add(summoner);
            }
            //_context.SaveChanges();
            Console.WriteLine("Done...");
        }
    }
}
