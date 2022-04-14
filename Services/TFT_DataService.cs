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
using TFT_Stats.Models.ViewModel;


namespace TFT_Stats.Services
{
    public class TFT_DataService : ITFT_DataService
    {
        private readonly TFTDbContext _context;

        public TFT_DataService(TFTDbContext context, IConfiguration configuration)
        {
            Configuration = configuration;
            _context = context;
        }

        private IConfiguration Configuration { get; }

        public void GetAdditionalCompanionInfo()
        {
            Console.WriteLine("GetAdditionalCompanionInfo() has Started...");
            var companions = _context.Companions.Where(c => c.ImgPath == null).ToList();
            var json = new WebClient().DownloadString("https://raw.communitydragon.org/latest/plugins/rcp-be-lol-game-data/global/default/v1/companions.json");
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

                //Create URL link to the companion
                string[] splitPath = imgLocation.Split("/");
                string pngName = splitPath[^1].ToLower();
                string path = "https://raw.communitydragon.org/latest/plugins/rcp-be-lol-game-data/global/default/assets/loadouts/companions/" + pngName;

                //Update Companion values in database
                updateCompanion.ImgPath = path;
                updateCompanion.Name = companionName;
                updateCompanion.Species = speciesName;
                updateCompanion.Level = level;
            }
            Console.WriteLine("Worker Function GetAdditionalCompanionInfo() has Finished!");
            _context.SaveChanges();
        }
        public void GetApiData()
        {
            Console.WriteLine("GetApiData() function has Started...");
            var riotApi = RiotApi.NewInstance(Configuration["ConnectionStrings:ApiKey"]);
            var tier = "DIAMOND";
            var division = "I";
            var entry = riotApi.TftLeagueV1.GetLeagueEntries(Region.NA, tier, division);

            List<String> summonerIdList = new();

            foreach (var item in entry)
            {
                //Console.WriteLine($"SummonerId: {item.SummonerId}");
                summonerIdList.Add(item.SummonerId.ToString());
            }

            //for (int i = 0; i < 15; i++)
            foreach (var item in summonerIdList)
            {
                var puuid = riotApi.TftSummonerV1.GetBySummonerId(Region.NA, item);

                //Get 20 recent matches 
                var matches = riotApi.TftMatchV1.GetMatchIdsByPUUID(Region.Americas, puuid.Puuid);
                foreach (var matchId in matches)
                {
                    Match matchExist = _context.Matches.FirstOrDefault(m => m.RiotMatchID == matchId);
                    //If Duplicate is not found in the database
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
                            Companion newCompanion = new() { RiotCompanionID = participant.Companion.ContentID, SkinID = participant.Companion.SkinID };
                            _context.Companions.Add(newCompanion);
                            Console.WriteLine("Companion Added!");
                        }
                        _context.SaveChanges();
                    }
                    else
                    {
                        //DUPLICATE FOUND
                        _context.Entry(matchExist).State = EntityState.Modified;
                    }
                }
            }
            Console.WriteLine("Worker Function GetApiData() has Finished!");
        }

        public void UpdateCompanionVmDb()
        {
            Console.WriteLine("UpdateCompanionVmDb() has Started...");
            var ListNames = _context.Companions
                .Select(c => c.Name)
                .Distinct()
                .ToList();
            foreach (var name in ListNames)
            {
                CompanionVM CompanionObj = _context.CompanionViewModel.FirstOrDefault(c => c.Name == name);
                if (CompanionObj == null)
                {
                    //If new unique companion has yet to be found
                    CompanionVM newVM = new();

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

                    //Create VM Object
                    newVM.Name = name;
                    newVM.Count = count;
                    newVM.Level1 = lvl1Count;
                    newVM.Level2 = lvl2Count;
                    newVM.Level3 = lvl3Count;
                    newVM.Species = cSpecies.species;
                    newVM.ImgUrl = cSpecies.imgPath;
                    _context.CompanionViewModel.Add(newVM);
                }
                else
                {
                    //If Companion has already been added to the database
                    //Count 
                    var count = _context.Companions.Count(c => c.Name == name);

                    //List of levels
                    var lvl1Count = _context.Companions.Where(c => c.Name == name && c.Level == 1).Count();
                    var lvl2Count = _context.Companions.Where(c => c.Name == name && c.Level == 2).Count();
                    var lvl3Count = _context.Companions.Where(c => c.Name == name && c.Level == 3).Count();

                    //Update count
                    CompanionObj.Count = count;
                    CompanionObj.Level1 = lvl1Count;
                    CompanionObj.Level2 = lvl2Count;
                    CompanionObj.Level3 = lvl3Count;
                    _context.CompanionViewModel.Update(CompanionObj);
                }
                _context.SaveChanges();
            }
            Console.WriteLine("Worker Function UpdateCompanionVmDb() has Finished!");
        }
    }
}
