using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        public IActionResult CompanionView(string sortOrder, string currentFilter, string searchString, string speciesFilter)
        {
            ViewBag.NameSortParm = sortOrder == "name_ascen" ? "name_desc" : "name_ascen";
            ViewBag.Species = sortOrder == "species_ascen" ? "species_desc" : "species_ascen";
            ViewBag.Count = String.IsNullOrEmpty(sortOrder) ? "count_ascen" : "";
            var speciesQuery = _context.CompanionViewModel.OrderBy(c => c.Species).Select(c => c.Species).Distinct().ToList();
            ViewBag.speciesList = new SelectList(speciesQuery);
            var companionList = _context.CompanionViewModel.AsQueryable();
            if (searchString == null)
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;

            if (!String.IsNullOrEmpty(searchString) && !String.IsNullOrEmpty(speciesFilter))
            {
                companionList = companionList.Where(c => c.Name == searchString && c.Species == speciesFilter);
            }
            else if (String.IsNullOrEmpty(searchString) && !String.IsNullOrEmpty(speciesFilter))
            {
                companionList = companionList.Where(c => c.Species == speciesFilter);
            }
            else if (!String.IsNullOrEmpty(searchString) && String.IsNullOrEmpty(speciesFilter))
            {
                companionList = companionList.Where(c => c.Name == searchString);
            }
            else
            {
                //nothing
            }

            switch (sortOrder)
            {
                case "name_desc":
                    companionList = companionList.OrderByDescending(c => c.Count);
                    break;
                case "species_desc":
                    companionList = companionList.OrderByDescending(c => c.Species);
                    break;
                case "name_ascen":
                    companionList = companionList.OrderBy(c => c.Name);
                    break;
                case "species_ascen":
                    companionList = companionList.OrderBy(c => c.Species);
                    break;
                case "count_ascen":
                    companionList = companionList.OrderBy(c => c.Count);
                    break;
                default:
                    companionList = companionList.OrderByDescending(c => c.Count);
                    break;
            }
            return View(companionList);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
