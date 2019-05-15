using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Models;
using SocialNetwork.Services;

namespace SocialNetwork.Controllers
{
/*    [Route("[controller]/[action]")]
    [ApiController]*/
    public class CircleController : Controller
    {
        private readonly CircleService _circleService;
        private readonly UserService _userService;
        private readonly WallService _wallService;


        public CircleController(CircleService circleService, UserService userService, WallService wallService)
        {
            _wallService = wallService;
            _circleService = circleService;
            _userService = userService;
        }

        public IActionResult Index(string id)
        {
            return View(_circleService.Get());
        }

        public IActionResult Create()
        {
            
            if(!string.IsNullOrEmpty(_circleService.GetLoggedInUserId()))
                return View();
            else
            {
                return Unauthorized();
            }
            
        }

        // POST: Circle/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Circle circle)
        {

            circle.OwnerId = _circleService.GetLoggedInUserId();

            var wall = new Wall()
            {
                owner = _userService.Get(circle.OwnerId).Name,
                Followers = new List<follower>(),
                BlackList = new List<blacklistedUser>(),
                ownerID = _circleService.GetLoggedInUserId()
            };

            _wallService.Create(wall);

            //add circle.wallId after creating wall
            circle.WallId = wall.ID;

            // sæt returværdi på Create, så vi kan gå til den circle vi lige har created.
            var circleId = _circleService.Create(circle);
            return RedirectToAction("Index");

        }

        [Route("Circle/ShowCircle/{id}")]
        public IActionResult ShowCircle(string circleId)
        {
            return View(_circleService.Get(circleId));
        }

        public IActionResult Delete(string id)
        {
            _circleService.Remove(id);
            return RedirectToAction("Index");
            
        }

    }
}