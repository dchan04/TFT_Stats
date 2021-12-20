using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MingweiSamuel.Camille;
using MingweiSamuel.Camille.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using TFT_Stats.Models;

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
            var riotApi = RiotApi.NewInstance("RGAPI-72a76fa1-9258-4006-b75e-03297ee07552");
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
            foreach(var item in entry)
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
            var match = riotApi.TftMatchV1.GetMatch(Region.Americas, matchList[0]);
            Console.WriteLine($"match:{match}");


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
            return View();
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
