using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Linq;
using TFT_Stats.Data;
using TFT_Stats.Models;

namespace TFT_Stats.Controllers
{
    public class HomeController : Controller
    {
        private readonly TFTDbContext _context;
        public HomeController(TFTDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var top3Companions = _context.CompanionViewModel.OrderByDescending(c => c.Count).Take(3).ToList();
            ViewBag.First = top3Companions[0].ImgUrl;
            ViewBag.Second = top3Companions[1].ImgUrl;
            ViewBag.Third = top3Companions[2].ImgUrl;
            return View();
        }

        public IActionResult CompanionView(string sortOrder)
        {
            ViewBag.NameSortParm = sortOrder == "name_ascen" ? "name_desc" : "name_ascen";
            ViewBag.Species = sortOrder == "species_ascen" ? "species_desc" : "species_ascen";
            ViewBag.Count = String.IsNullOrEmpty(sortOrder) ? "count_ascen" : "";
            return sortOrder switch
            {
                "name_desc" => View(_context.CompanionViewModel.ToList().OrderByDescending(c => c.Name)),
                "species_desc" => View(_context.CompanionViewModel.ToList().OrderByDescending(c => c.Species)),
                "name_ascen" => View(_context.CompanionViewModel.ToList().OrderBy(c => c.Name)),
                "species_ascen" => View(_context.CompanionViewModel.ToList().OrderBy(c => c.Species)),
                "count_ascen" => View(_context.CompanionViewModel.ToList().OrderBy(c => c.Count)),
                _ => View(_context.CompanionViewModel.ToList().OrderByDescending(c => c.Count)),
            };
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
