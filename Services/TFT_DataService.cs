using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net;
using Microsoft.EntityFrameworkCore;
using MingweiSamuel.Camille;
using MingweiSamuel.Camille.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Configuration;
using Microsoft.IdentityModel.Protocols;
using TFT_Stats.Data;
using TFT_Stats.Models;
using System.Linq;

namespace TFT_Stats.Services
{
    public class TFT_DataService : ITFT_DataService
    {
        private TFTDbContext _context;

        public TFT_DataService(TFTDbContext context)
        {
            _context = context;
        }

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

        public void TestCompanionJson()
        {
            var json = new WebClient().DownloadString("https://raw.communitydragon.org/pbe/plugins/rcp-be-lol-game-data/global/default/v1/companions.json");
            dynamic jObj = JsonConvert.DeserializeObject<dynamic>(json);
            string token = "$.[?(@.contentId == " + "'0e251d36-d86e-4c58-9b7f-bcee2376a408'" + ")]";
            JToken iconLocation = jObj.SelectToken(token);
            //Console.WriteLine($"{iconLocation["loadoutsIcon"]}");
            string imgLocation = (string)iconLocation["loadoutsIcon"];
            Console.WriteLine(imgLocation);
            string[] splitPath = imgLocation.Split("/");
            string pngName = splitPath[splitPath.Length - 1].ToLower();
            string path = "https://raw.communitydragon.org/latest/plugins/rcp-be-lol-game-data/global/default/assets/loadouts/companions/" + pngName;
            Console.WriteLine($"image file name: {path}");
        }

        public void TestRiotApi()
        {
            var riotApi = RiotApi.NewInstance("RGAPI-6ac15d8a-3dcb-442a-91d5-62cfdb403c9e");
            var tier = "DIAMOND";
            var division = "I";
            var entry = riotApi.TftLeagueV1.GetLeagueEntries(Region.NA, tier, division);

            List<String> summonerIdList = new();
            List<String> puuidList = new();
            //List<String> matchList = new();
            //Console.WriteLine("Working on 1st function...");
            foreach (var item in entry)
            {
                //Console.WriteLine($"SummonerId: {item.SummonerId}");
                summonerIdList.Add(item.SummonerId.ToString());
            }
            //Console.WriteLine($"Count: {summonerIdList.Count}");

            for (int i = 0; i < 1; i++)
            {
                var puuid = riotApi.TftSummonerV1.GetBySummonerId(Region.NA, summonerIdList[i]);

                //Get 20 recent matches 
                var matches = riotApi.TftMatchV1.GetMatchIdsByPUUID(Region.Americas, puuid.Puuid);
                var matchList = new List<Match>();
                foreach (var matchId in matches)
                {
                    var match = riotApi.TftMatchV1.GetMatch(Region.Americas, matchId);
                    foreach(var participant in match.Info.Participants)
                    {
                        Companion newCompanion = new Companion() { RiotCompanionID = participant.Companion.ContentID, SkinID = participant.Companion.SkinID};
                        //Participant participant1 = new Participant() {Companion = newCompanion, MatchID = int.Parse(matchId)};
                        _context.Companions.Add(newCompanion);
                        //Console.WriteLine($"{participant.Companion}");
                        Console.WriteLine("Companion Added!");
                    }
                    
                }
                //Summoners summoner = new Summoners() { RiotSummonerId = puuid.Id, Puuid = puuid.Puuid, Matches = null };
                //_context.Summoners.Add(summoner);
            }
            _context.SaveChanges();
            //Summoners summoner = new Summoners() {RiotSummonerId }
            /*
            foreach (var puuid in puuidList)
            {
                var matches = riotApi.TftMatchV1.GetMatchIdsByPUUID(Region.Americas, puuid);
                foreach (var match in matches)
                {
                    matchList.Add(match);
                    Console.WriteLine($"{match}");
                }
            }*/
            /*
            for (int i = 0; i < 3; i++)
            {
                var puuid = riotApi.TftSummonerV1.GetBySummonerId(Region.NA, summonerIdList[i]);
                puuidList.Add(puuid.Puuid);
                //Console.WriteLine($"{i}.Puuid: {puuid.Puuid}");
            }
            
            foreach (var puuid in puuidList)
            {
                var matches = riotApi.TftMatchV1.GetMatchIdsByPUUID(Region.Americas, puuid);
                foreach (var match in matches)
                {
                    matchList.Add(match);
                }
            }

            foreach (var game in matchList)
            {
                var match = riotApi.TftMatchV1.GetMatch(Region.Americas, game);
                foreach (var participants in match.Info.Participants)
                {
                    Console.WriteLine($"{participants.Companion.ContentID} -- {participants.Companion.SkinID}"); //Skin ID is the last 2 numbers for itemId.
                }
            }*/
            Console.WriteLine("Done...");
        }
    }
}
