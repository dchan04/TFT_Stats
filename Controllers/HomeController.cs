using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MingweiSamuel.Camille;
using MingweiSamuel.Camille.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using TFT_Stats.Data;
using TFT_Stats.Models;
using TFT_Stats.Models.ViewModel;

namespace TFT_Stats.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private TFTDbContext _context;
        public HomeController(ILogger<HomeController> logger, TFTDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View(_context.Companions.ToList());
        }

        public IActionResult CompanionView()
        {
            var companions = _context.Companions.ToList();
            List<CompanionVM> companionVMList = new();

            //Get all unique companion names
            var ListNames = _context.Companions
                .Select(c => c.Name)
                .Distinct()
                .ToList();
            //For each unique companion, get related data
            foreach (var name in ListNames)
            {
                CompanionVM objVM = new();

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
                objVM.Name = name;
                objVM.Count = count;
                objVM.Level1 = lvl1Count;
                objVM.Level2 = lvl2Count;
                objVM.Level3 = lvl3Count;
                objVM.Species = cSpecies.species;
                objVM.ImgUrl = cSpecies.imgPath;
                companionVMList.Add(objVM);
            }
            return View(companionVMList);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
