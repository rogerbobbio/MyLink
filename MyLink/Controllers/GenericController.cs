using System.Linq;
using System.Web.Mvc;
using MyLink.Models;

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