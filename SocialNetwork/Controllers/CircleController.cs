using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Models;
using SocialNetwork.Services;
using SocialNetwork.ViewModels;
using SocialNetWork.Models;

namespace SocialNetwork.Controllers
{
/*    [Route("[controller]/[action]")]
    [ApiController]*/
    public class CircleController : Controller
    {
        private readonly CircleService _circleService;
        private readonly UserService _userService;
        private readonly WallService _wallService;
        private readonly PostService _postService;

        public CircleController(CircleService circleService, UserService userService, WallService wallService, PostService postService)
        {
            _postService = postService;
            _wallService = wallService;
            _circleService = circleService;
            _userService = userService;
        }

        public IActionResult Index()
        {

            var circles = _circleService.Get();
            

            return View(circles);
        }

        public IActionResult MyCircles(string id)
        {
            var user = _userService.Get(id);

            Debug.WriteLine(user.Id);

            var wallsWhereUserIsFollower = _wallService.Get()
                .Where(wall => wall.Followers.Any(follower => follower.followerID == id)).ToList();

            foreach (var wall in _wallService.Get())
            {
                foreach (var wallFollower in wall.Followers)
                {
                    Debug.WriteLine(wallFollower.followerID);
                }
            }

            foreach (var wall in wallsWhereUserIsFollower)
            {
                Debug.WriteLine(wall.ID);
            }

            var circles = _circleService.Get();

            foreach (var circle in circles)
            {
                Debug.WriteLine(circle.Id);
            }

            var circlesResult = new List<Circle>();

            foreach (var wall in wallsWhereUserIsFollower)
            {
                foreach (var circle in circles)
                {
                    if (wall.ID == circle.WallId)
                    {
                        circlesResult.Add(circle);
                    }
                }
            }


            return View(circlesResult);
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
            var loggedInUserId = _circleService.GetLoggedInUserId();
            circle.OwnerId = loggedInUserId;
            var circleId = _circleService.Create(circle);

            

            var wall = new Wall()
            {
                owner = circle.Name,
                Followers = new List<follower>()
                {
                    new follower()
                    {
                        followerID = loggedInUserId,
                        followerName = _userService.Get(loggedInUserId).Name
                    }
                },
                BlackList = new List<blacklistedUser>(),
                ownerID = circleId,
                type = "Circle",
                postIDs = new List<string>(),
            };

            var newWall = _wallService.Create(wall);

            //add circle.wallId after creating wall
            circle.WallId = newWall.ID;
            circle.Id = circleId;

            _circleService.Update(circle.Id, circle);
            
            return RedirectToAction("Index");

        }

        [Route("Circle/ShowCircle/{id}")]
        public IActionResult ShowCircle(string circleId)
        {
            var viewModel = new CircleViewModel {Circle = _circleService.Get(circleId)};

            viewModel.Wall = _wallService.Get(viewModel.Circle.WallId,"Circle");

            if(viewModel.Wall != null)
                viewModel.Posts = _postService.Get().Where(p => p.WallId == viewModel.Wall.ID).ToList();

            return View(viewModel);
        }

        public IActionResult Delete(string id)
        {
            _circleService.Remove(id);
            return RedirectToAction("Index");
            
        }

    }
}