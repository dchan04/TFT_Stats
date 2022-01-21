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
        private readonly TFTDbContext _context;
        public HomeController(TFTDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CompanionView()
        {
            return View(_context.CompanionViewModel.ToList().OrderByDescending(c => c.Count));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
