using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MyLink.Classes;
using MyLink.Models;

namespace MyLink.Controllers
{
    [Authorize(Roles = "Admin")]
    public class LinkCategoriesController : Controller
    {
        private MyLinkContext db = new MyLinkContext();
        
        public ActionResult Index()
        {
            return View(db.LinkCategories.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var linkCategory = db.LinkCategories.Find(id);
            if (linkCategory == null)
            {
                return HttpNotFound();
            }
            return View(linkCategory);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LinkCategory linkCategory)
        {
            if (ModelState.IsValid)
            {
                db.LinkCategories.Add(linkCategory);
                var responseSave = DBHelper.SaveChanges(db);
                if (responseSave.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError(string.Empty, responseSave.Message);
            }

            return View(linkCategory);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var linkCategory = db.LinkCategories.Find(id);
            if (linkCategory == null)
            {
                return HttpNotFound();
            }
            return View(linkCategory);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LinkCategory linkCategory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(linkCategory).State = EntityState.Modified;
                var responseSave = DBHelper.SaveChanges(db);
                if (responseSave.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError(string.Empty, responseSave.Message);
            }
            return View(linkCategory);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var linkCategory = db.LinkCategories.Find(id);
            if (linkCategory == null)
            {
                return HttpNotFound();
            }
            return View(linkCategory);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var linkCategory = db.LinkCategories.Find(id);
            db.LinkCategories.Remove(linkCategory);
            var responseSave = DBHelper.SaveChanges(db);
            if (responseSave.Succeeded)
            {
                return RedirectToAction("Index");
            }
            ModelState.AddModelError(string.Empty, responseSave.Message);
            return View(linkCategory);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
