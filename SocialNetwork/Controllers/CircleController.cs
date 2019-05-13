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
        private LoginTestService _loginTestService;

        public CircleController(CircleService circleService, LoginTestService loginTestService)
        {
            _circleService = circleService;
            _loginTestService = loginTestService;
        }

        public IActionResult Index(string id)
        {
            return View(_circleService.Get());
        }

        public IActionResult Create()
        {
            try
            {
                var user = _loginTestService.Get(HttpContext.Session.GetString("UserId")).userID;

                return View();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Unauthorized();
            }
            
        }

        // POST: Circle/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Circle circle)
        {
            try
            {
                Debug.WriteLine(circle.Name);
                var loggedInUser = _loginTestService.Get(HttpContext.Session.GetString("UserId"));
                circle.OwnerId = loggedInUser.userID;


                var wall = new Wall()
                {
                    owner = loggedInUser.userName,
                    Followers = new List<follower>(),
                    BlackList = new List<blacklistedUser>(),
                    ownerID = loggedInUser.userID
                };


                //add circle.wallId after creating wall
                
                // sæt returværdi på Create, så vi kan gå til den circle vi lige har created.
                var circleId = _circleService.Create(circle);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}