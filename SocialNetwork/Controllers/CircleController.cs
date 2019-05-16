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
            var circleId = _circleService.Create(circle);

            

            var wall = new Wall()
            {
                owner = circle.Name,
                Followers = new List<follower>(),
                BlackList = new List<blacklistedUser>(),
                ownerID = circleId,
                type = "Circle",
                postIDs = new List<string>(),
            };

            Wall newWall = _wallService.Create(wall);

            //add circle.wallId after creating wall
            circle.WallId = newWall.ID;
            circle.Id = circleId;

            _circleService.Update(circle.Id, circle);

            // sæt returværdi på Create, så vi kan gå til den circle vi lige har created.
            
            return RedirectToAction("Index");

        }

        [Route("Circle/ShowCircle/{id}")]
        public IActionResult ShowCircle(string circleId)
        {
            var viewModel = new CircleViewModel {Circle = _circleService.Get(circleId)};

            viewModel.Wall = _wallService.Get(viewModel.Circle.WallId,"Circle");

            if(viewModel.Wall != null)
                viewModel.Posts = _postService.GetPostForWall(viewModel.Wall.ID);

            return View(viewModel);
        }

        public IActionResult Delete(string id)
        {
            _circleService.Remove(id);
            return RedirectToAction("Index");
            
        }

    }
}