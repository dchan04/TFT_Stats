using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using MingweiSamuel.Camille;
using MingweiSamuel.Camille.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TFT_Stats.Services
{
    public class TFT_DataService : IHostedService
    {
        private Timer _timer;
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(TestCompanionJson, null, 0, 10000);
            return Task.CompletedTask;
        }

        private void TestCompanionJson(object state)
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
            var riotApi = RiotApi.NewInstance("RGAPI-5aefafd8-4565-4f3c-b9e3-a5303ec45e73");
            var tier = "DIAMOND";
            var division = "I";
            var entry = riotApi.TftLeagueV1.GetLeagueEntries(Region.NA, tier, division);

            List<String> summonerIdList = new();
            List<String> puuidList = new();
            List<String> matchList = new();

            Console.WriteLine("Working on 1st function...");
            foreach (var item in entry)
            {
                //Console.WriteLine($"SummonerId: {item.SummonerId}");
                summonerIdList.Add(item.SummonerId.ToString());
            }
            //Console.WriteLine($"Count: {summonerIdList.Count}");
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
            }
            Console.WriteLine("Done...");
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
    }
}
