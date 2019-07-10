using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using MyLink.Models;
using PagedList;
using MyLink.Classes;

namespace MyLink.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProjectsController : Controller
    {
        private MyLinkContext db = new MyLinkContext();

        public ActionResult Index(int? page = null)
        {
            page = (page ?? 1);
            var projects = db.Projects.Include(p => p.Bank).Include(p => p.City).Include(p => p.Department).OrderBy(c => c.Bank.Name);
            return View(projects.ToPagedList((int)page, 5));
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        public ActionResult Create()
        {
            ViewBag.BankId = new SelectList(CombosHelper.GetBanks(), "BankId", "Name");
            ViewBag.DepartmentId = new SelectList(CombosHelper.GetDepartments(), "DepartmentId", "Name");
            ViewBag.CityId = new SelectList(CombosHelper.GetCities(0), "CityId", "Name");
            var projectModel = new Project
            {
                Salary = 0
            };
            return View(projectModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Project project)
        {
            if (ModelState.IsValid)
            {
                db.Projects.Add(project);
                var response = DBHelper.SaveChanges(db);
                if (response.Succeeded)
                {
                return RedirectToAction("Index");
            }
                ModelState.AddModelError(string.Empty, response.Message);
            }

            ViewBag.BankId = new SelectList(CombosHelper.GetBanks(), "BankId", "Name", project.BankId);
            ViewBag.CityId = new SelectList(CombosHelper.GetCities(project.DepartmentId), "CityId", "Name", project.CityId);
            ViewBag.DepartmentId = new SelectList(CombosHelper.GetDepartments(), "DepartmentId", "Name", project.DepartmentId);
            return View(project);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            ViewBag.BankId = new SelectList(CombosHelper.GetBanks(), "BankId", "Name", project.BankId);
            ViewBag.CityId = new SelectList(CombosHelper.GetCities(project.DepartmentId), "CityId", "Name", project.CityId);
            ViewBag.DepartmentId = new SelectList(CombosHelper.GetDepartments(), "DepartmentId", "Name", project.DepartmentId);
            return View(project);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Project project)
        {
            if (ModelState.IsValid)
            {
                db.Entry(project).State = EntityState.Modified;
                var response = DBHelper.SaveChanges(db);
                if (response.Succeeded)
                {
                return RedirectToAction("Index");
            }
                ModelState.AddModelError(string.Empty, response.Message);
            }
            ViewBag.BankId = new SelectList(db.Banks, "BankId", "Name", project.BankId);
            ViewBag.CityId = new SelectList(CombosHelper.GetCities(project.DepartmentId), "CityId", "Name", project.CityId);
            ViewBag.DepartmentId = new SelectList(CombosHelper.GetDepartments(), "DepartmentId", "Name", project.DepartmentId);
            return View(project);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = db.Projects.Find(id);
            db.Projects.Remove(project);
            var response = DBHelper.SaveChanges(db);
            if (response.Succeeded)
            {
            return RedirectToAction("Index");
        }
            ModelState.AddModelError(string.Empty, response.Message);
            return View(project);
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
