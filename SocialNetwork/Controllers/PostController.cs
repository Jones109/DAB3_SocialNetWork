using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SocialNetWork.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Models;
using SocialNetwork.Services;

namespace SocialNetWork.Controllers
{

   
    public class PostController : Controller
    {
        private PostService _postService;
        private UserService _userService;
        private WallService _wallService;
        

        public PostController(PostService postService, UserService userService, WallService wallService)
        {
            _postService = postService;
            _userService = userService;
            _wallService = wallService;

        }

        // GET: Post
        public ActionResult Index()
        {
            return View(_postService.Get());
        }

        // GET: Post/Details/5
        public ActionResult Details(string id)
        {
            return View();
        }

        // GET: Post/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Post/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Post post)
        {
            try
            {
                User currentUser = _userService.Get(HttpContext.Session.GetString("UserId"));

                post.WallId = currentUser.Wall;
                post.Comments = new List<Comment>();
                post.CreationTime= DateTime.Now;
                post.OwnerName = currentUser.UserName;

                Post createdPost = _postService.Create(post);

                Wall newWall = _wallService.GetByWallId(createdPost.WallId);
                if (newWall.postIDs==null)
                {
                    newWall.postIDs = new List<string>();
                }
               

                newWall.postIDs.Add(new string(createdPost.Id));

                _wallService.Update(newWall);

                return RedirectToAction(nameof(Index));
            }
            catch(Exception e)
            {
                return View();
            }
        }

        // GET: Post/Edit/5
        public ActionResult Edit(string id)
        {
            return View();
        }

        // POST: Post/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, Post post)
        {
            try
            {

                Post editedPost = _postService.Get(id);
                if (post.ImgUri != null)
                {
                    editedPost.ImgUri = post.ImgUri;
                }

                if (post.Text != null)
                {
                    editedPost.Text = post.Text;
                }

                _postService.Update(id, editedPost);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Post/Delete/5
        public ActionResult Delete(string id)
        {
            return View();
        }

        // POST: Post/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id, Post post)
        {
            try
            {
                _postService.Remove(post);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}