using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Services;

namespace SocialNetwork.Controllers
{
    [Route("api/[controller]/[action]")]
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
    }
}