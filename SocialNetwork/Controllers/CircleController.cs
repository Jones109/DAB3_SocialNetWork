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

            var test = _wallService.Get();

            var wallsWhereUserIsFollower = new List<Wall>();
            var test2 = test.FindAll(wall => wall.Followers != null);

            foreach (var wall in test2)
            {
                if (wall.Followers.Any(follower => follower.followerID == id))
                {
                    wallsWhereUserIsFollower.Add(wall);
                }
            }

            /*foreach (var wall in wallsWhereUserIsFollower)
            {
                Debug.WriteLine(wall.ID);
            }*/

            var circles = _circleService.Get();

            /*foreach (var circle in circles)
            {
                Debug.WriteLine(circle.Id);
            }*/

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
            
            return RedirectToAction("GetWall","Wall", new {id = circleId, type = "Circle"});

        }

        [Route("Circle/ShowCircle/{circleId}")]
        public IActionResult ShowCircle(string circleId)
        {
            var viewModel = new CircleViewModel {Circle = _circleService.Get(circleId)};

            var walls = _wallService.Get() ?? throw new ArgumentNullException("_wallService.Get()");

            viewModel.Wall = walls.Find(wall => wall.ownerID == circleId);

            if (viewModel.Wall != null)
                viewModel.Posts = _postService.Get().Where(p => p.WallId == viewModel.Wall.ID).ToList();

            return View(viewModel);
        }

        public IActionResult Delete(string id)
        {
 
            _circleService.Remove(id);

            return RedirectToAction("Index");
            
        }


        public IActionResult FollowCircle(string idToFollow)
        {
            try
            {
                var currentUserId = HttpContext.Session.GetString("UserId");
                var circleToFollow = _circleService.Get(idToFollow);
                var currentUser = _userService.Get(currentUserId);

                var wallToFollow = _wallService.GetByWallId(circleToFollow.WallId);

                if (currentUser.Circles == null)
                {
                    currentUser.Circles = new List<string>();
                }

                currentUser.Circles.Add(circleToFollow.Id);
                _userService.UpdateNotPassword(currentUser);

                if (wallToFollow.Followers==null)
                {
                    wallToFollow.Followers = new List<follower>();
                }

                wallToFollow.Followers.Add(new follower()
                    {followerID = currentUserId, followerName = currentUser.Name});
                _wallService.Update(wallToFollow);

                return Redirect($"/Circle/ShowCircle/{circleToFollow.Id}");

            }
            catch (Exception e)
            {
                return RedirectToAction("Index");
            }
        }

        public IActionResult UnFollowCircle(string idToUnFollow)
        {
            try
            {
                var currentUserId = HttpContext.Session.GetString("UserId");
                var circleToUnFollow = _circleService.Get(idToUnFollow);
                var currentUser = _userService.Get(currentUserId);
                var wallToUnFollow = _wallService.GetByWallId(circleToUnFollow.WallId);

                currentUser.Circles.Remove(circleToUnFollow.Id);
                _userService.UpdateNotPassword(currentUser);

                int index = wallToUnFollow.Followers.FindIndex(f=>f.followerID==currentUserId);
                wallToUnFollow.Followers.RemoveAt(index);
                _wallService.Update(wallToUnFollow);

                return RedirectToAction("Index");

            }
            catch (Exception e)
            {
                return RedirectToAction("Index");
            }
        }

    }
}