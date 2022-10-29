﻿using front_to_back.DAL;
using front_to_back.Models;
using front_to_back.ViewModels.Home;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace front_to_back.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _appDbContext;

        public HomeController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<IActionResult> Index()
        {

            var recentWorkComponents = await _appDbContext.RecentWorkComponents.ToListAsync();

            var model = new HomeIndexViewModel
            {
                RecentWorkComponents = await _appDbContext.RecentWorkComponents.OrderByDescending(rcw => rcw.Id).Take(3).ToListAsync()
            };

            return View(model);
        }
         public async Task<IActionResult> Loadmore(int skipRow)
        {

            var recentWorkComponents = await _appDbContext.RecentWorkComponents.OrderByDescending(rcw => rcw.Id).Skip(3 * skipRow).Take(3).ToListAsync();
            return PartialView("_RecentWorkComponentPartial",recentWorkComponents);
        }


    }
}
