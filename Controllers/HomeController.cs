using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MingweiSamuel.Camille;
using MingweiSamuel.Camille.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text.Json;
using System.Threading;
using TFT_Stats.Models;
using Newtonsoft.Json;

namespace TFT_Stats.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            //TestRiotApi();
            TestCompanionJson();
            return View();
        }

        public void TestCompanionJson()
        {
            var json = new WebClient().DownloadString("https://raw.communitydragon.org/pbe/plugins/rcp-be-lol-game-data/global/default/v1/companions.json");
            dynamic jObj = JsonConvert.DeserializeObject<dynamic>(json); 
            Console.WriteLine(jObj);
        }
        
        public void TestRiotApi()
        {
            var riotApi = RiotApi.NewInstance("RGAPI-23f5e1ab-6307-4cce-9329-570b7e05750c");
            var tier = "DIAMOND";
            var division = "I";
            //riotApi.TftLeagueV1.GetLeagueEntries(region1, tier, division)
            //riotApi.SummonerV4.GetBySummonerName(Region.NA, "CrackDuck")
            //riotApi.TftLeagueV1.GetLeagueEntriesForSummoner(Region.NA, "CrackDuck")
            //var entries = new[]
            //{
            //    riotApi.TftLeagueV1.GetLeagueEntries(region1, tier, division)
            //};
            var entry = riotApi.TftLeagueV1.GetLeagueEntries(Region.NA, tier, division);
            List<String> summonerIdList = new List<string>();
            List<String> puuidList = new List<string>();
            foreach (var item in entry)
            {
                //Console.WriteLine($"SummonerId: {item.SummonerId}");
                summonerIdList.Add(item.SummonerId.ToString());
            }
            Console.WriteLine($"Count: {summonerIdList.Count}");

            //Get Puuid from summonerID
            var puuid = riotApi.TftSummonerV1.GetBySummonerId(Region.NA, summonerIdList[1]);
            Console.WriteLine($"Puuid: {puuid.Puuid}");

            //Get list of matches
            var matchList = riotApi.TftMatchV1.GetMatchIdsByPUUID(Region.Americas, puuid.Puuid);
            Console.WriteLine($"matchID: {matchList[0]}");

            //Get match details
            var match = riotApi.TftMatchV1.GetMatch(Region.Americas, matchList[1]);
            Console.WriteLine($"match:{match.Info.Participants[0].Companion}");

            /*
            foreach (var summoner in summoners)
            {
                Console.WriteLine($"{summoner.Name}'s Top 10 Champs:");

                var masteries =
                    riotApi.ChampionMasteryV4.GetAllChampionMasteries(Region.NA, summoner.Id);


                for (var i = 0; i < 10; i++)
                {
                    var mastery = masteries[i];
                    //Console.WriteLine(masteries[i]);  
                    // Get champion for this mastery.
                    var champ = (Champion)mastery.ChampionId;
                    // print i, champ id, champ mastery points, and champ level
                    Console.WriteLine("{0,3}) {1,-15} {2,10:N0} ({3})", i + 1, champ.Name(), mastery.ChampionPoints, mastery.ChampionLevel);
                }
                Console.WriteLine();
            }
            */
            return;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
