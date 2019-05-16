using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SocialNetwork.Data;
using SocialNetwork.Models;
using SocialNetwork.Services;
using SocialNetwork.ViewModels;
using SocialNetWork.Models;

namespace SocialNetwork.Controllers
{

    public class WallController : Controller
    {
        
        private WallService _wallService;
        private PostService _postService;
        public WallController(WallService service,PostService pservice)
        {
            _wallService = service;
            _postService = pservice;
        }

        // GET: Wall
        public ActionResult Index()
        {
      
                
                return View(_wallService.Get());

        }
        

        // GET: Wall/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Wall/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Wall wall)
        {
            try
            {
                wall.postIDs= new List<PostId>();
                _wallService.Create(wall);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Wall/Edit/5
        public ActionResult Edit(string id,string type)
        {
            return View(_wallService.Get(id,type));
        }

        // POST: Wall/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, Wall wall)
        {
            try
            {
                _wallService.Update(wall);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Wall/Delete/5
        public ActionResult Delete(string id)
        {
            return View();
        }

        // POST: Wall/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id, Wall wall)
        {
            try
            {
                _wallService.Remove(wall);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        

        public ActionResult GetWall(string id,string type)
        {
            var wall = _wallService.Get(id, type);
            var Posts = _postService.GetPostForWall(wall.ID);
            GetWallViewModel viewModel = new GetWallViewModel();
            viewModel.wall = wall;
            viewModel.posts = Posts;
            return View(viewModel);
        }
    }
}