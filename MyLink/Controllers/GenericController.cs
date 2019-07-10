using System.Linq;
using System.Web.Mvc;
using MyLink.Models;
using System.Data.Entity;

namespace MyLink.Controllers
{
    public class GenericController : Controller
    {
        private MyLinkContext db = new MyLinkContext();

        public JsonResult GetCities(int departmentId)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var cities = db.Cities.Where(c => c.DepartmentId == departmentId);
            return Json(cities);
        }        

        public JsonResult GetProjects(int projectId)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var projects = db.Projects.Include(c => c.Bank).Where(c => c.ProjectId == projectId);            
            return Json(projects);
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