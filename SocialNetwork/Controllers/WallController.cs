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

namespace SocialNetwork.Controllers
{

    public class WallController : Controller
    {
        
        private WallService _wallService;

        public WallController(WallService service)
        {
            _wallService = service;
        }

        // GET: Wall
        public ActionResult Index()
        {
            return View(_wallService.Get());
        }

        // GET: Wall/Details/5
        public ActionResult Details(int id)
        {
            return View(_wallService.Get(id.ToString()));
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
                _wallService.Create(wall);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Wall/Edit/5
        public ActionResult Edit(string id)
        {
            return View(_wallService.Get(id));
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
    }
}