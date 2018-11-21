using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using MyLink.Classes;
using MyLink.Models;
using OfficeOpenXml;
using PagedList;

namespace MyLink.Controllers
{
    [Authorize(Roles = "User, Admin")]
    public class LinksController : Controller
    {
        private MyLinkContext db = new MyLinkContext();
        
        public ActionResult Index(string description, string url, string ranking, string pendingFlag, string subtitleFlag, string oldFlag, string topFlag, string seriesFlag,
            int? languageId, int? linkCategoryId,
            int? page = null)
        {
            ViewBag.LanguageId = new SelectList(CombosHelper.GetLanguages(), "LanguageId", "Name");
            ViewBag.LinkCategoryId = new SelectList(CombosHelper.GetLinkCategory(), "LinkCategoryId", "Name");
            ViewBag.LinkCategoryIdSelected = linkCategoryId;

            if (ranking == string.Empty) ranking = null;
            if (Convert.ToBoolean(pendingFlag) == false) pendingFlag = null;
            if (Convert.ToBoolean(subtitleFlag) == false) subtitleFlag = null;
            if (Convert.ToBoolean(oldFlag) == false) oldFlag = null;
            if (Convert.ToBoolean(topFlag) == false) topFlag = null;
            if (Convert.ToBoolean(seriesFlag) == false) seriesFlag = null;

            if (Convert.ToInt32(languageId) == 0) languageId = null;
            if (Convert.ToInt32(linkCategoryId) == 0) linkCategoryId = null;

            var rankingParameter = Convert.ToInt32(ranking);
            var pendingParameter = Convert.ToBoolean(pendingFlag);
            var subtitleParameter = Convert.ToBoolean(subtitleFlag);
            var oldParameter = Convert.ToBoolean(oldFlag);
            var topParameter = Convert.ToBoolean(topFlag);
            var seriesParameter = Convert.ToBoolean(seriesFlag);

            page = (page ?? 1);
            var links = db.Links.Include(l => l.Language).Include(l => l.LinkCategory);
            links = db.Links.Where(c => (c.Description.Contains(description) || description == null)
                                        && (c.Url.Contains(url) || url == null)
                                        && (c.Ranking.Equals(rankingParameter) || ranking == null)
                                        && (c.Pending.Equals(pendingParameter) || pendingFlag == null)
                                        && (c.Subtitle.Equals(subtitleParameter) || subtitleFlag == null)
                                        && (c.Old.Equals(oldParameter) || oldFlag == null)
                                        && (c.Top.Equals(topParameter) || topFlag == null)
                                        && (c.Series.Equals(seriesParameter) || seriesFlag == null)
                                        && (c.LanguageId == languageId || languageId == null)
                                        && (c.LinkCategoryId == linkCategoryId || linkCategoryId == null)
                                      ).OrderBy(c => c.Url).ThenBy(c => c.Description);

            return View(links.ToPagedList((int)page, 10));
        }

        public void ExportToExcel()
        {
            List<Link> links;
            links = db.Links.ToList();

            var pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add("Report");

            ws.Cells["A1"].Value = "Lista de Links";
            ws.Cells["A1"].Style.Font.Size = 20;
            ws.Cells["A1"].Style.Font.Bold = true;


            ws.Cells["A6"].Value = "Descripcion";
            ws.Cells["A6"].Style.Font.Bold = true;

            ws.Cells["B6"].Value = "Url";
            ws.Cells["B6"].Style.Font.Bold = true;

            ws.Cells["C6"].Value = "Categoria";
            ws.Cells["C6"].Style.Font.Bold = true;            

            var rowStart = 7;
            foreach (var item in links)
            {
                ws.Cells[string.Format("A{0}", rowStart)].Value = item.Description;
                ws.Cells[string.Format("B{0}", rowStart)].Value = item.Url;
                ws.Cells[string.Format("D{0}", rowStart)].Value = item.LinkCategory.Name;                
                rowStart++;
            }

            ws.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment; filename=" + "ExcelReport.xlsx");
            Response.BinaryWrite(pck.GetAsByteArray());
            Response.End();
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var link = db.Links.Find(id);
            if (link == null)
            {
                return HttpNotFound();
            }
            return View(link);
        }

        public ActionResult Create()
        {
            ViewBag.LanguageId = new SelectList(db.Languages, "LanguageId", "Name");
            ViewBag.LinkCategoryId = new SelectList(db.LinkCategories, "LinkCategoryId", "Name");

            var linkModel = new Link
            {
                Ranking = 2,
                CreationDate = DateTime.Now,
            };
            return View(linkModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Link link)
        {
            if (ModelState.IsValid)
            {
                db.Links.Add(link);
                var responseSave = DBHelper.SaveChanges(db);
                if (responseSave.Succeeded)
                {                    
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError(string.Empty, responseSave.Message);
            }

            ViewBag.LanguageId = new SelectList(db.Languages, "LanguageId", "Name", link.LanguageId);
            ViewBag.LinkCategoryId = new SelectList(db.LinkCategories, "LinkCategoryId", "Name", link.LinkCategoryId);
            return View(link);
        }
        
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var link = db.Links.Find(id);
            if (link == null)
            {
                return HttpNotFound();
            }
            ViewBag.LanguageId = new SelectList(db.Languages, "LanguageId", "Name", link.LanguageId);
            ViewBag.LinkCategoryId = new SelectList(db.LinkCategories, "LinkCategoryId", "Name", link.LinkCategoryId);
            return View(link);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Link link)
        {
            if (ModelState.IsValid)
            {
                db.Entry(link).State = EntityState.Modified;
                var responseSave = DBHelper.SaveChanges(db);
                if (responseSave.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError(string.Empty, responseSave.Message);
            }
            ViewBag.LanguageId = new SelectList(db.Languages, "LanguageId", "Name", link.LanguageId);
            ViewBag.LinkCategoryId = new SelectList(db.LinkCategories, "LinkCategoryId", "Name", link.LinkCategoryId);
            return View(link);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var link = db.Links.Find(id);
            if (link == null)
            {
                return HttpNotFound();
            }
            return View(link);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var link = db.Links.Find(id);
            db.Links.Remove(link);
            var responseSave = DBHelper.SaveChanges(db);
            if (responseSave.Succeeded)
            {                
                return RedirectToAction("Index");
            }
            ModelState.AddModelError(string.Empty, responseSave.Message);
            return View(link);
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
