using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Models;
using SocialNetwork.Services;

namespace SocialNetwork.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class CircleController : Controller
    {
        private readonly CircleService _circleService;
        public CircleController(CircleService circleService)
        {
            _circleService = circleService;
        }

        public IActionResult Index()
        {
            var circles = _circleService.Get();
            return View(circles);
        }

        public IActionResult Create()
        {
            return View();
        }

        // POST: Circle/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Circle circle)
        {
            try
            {   // sæt returværdi på Create, så vi kan gå til den circle vi lige har created.
                _circleService.Create(circle);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}