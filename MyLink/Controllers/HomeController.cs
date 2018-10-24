using System.Linq;
using System.Web.Mvc;
using MyLink.Models;

namespace MyLink.Controllers
{
    public class HomeController : Controller
    {
        private MyLinkContext db = new MyLinkContext();

        public ActionResult Index()
        {
            var user = db.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
            return View(user);
        }

        public ActionResult About()
        {
            ViewBag.Message = "System .NET.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Informacion de Contacto.";

            return View();
        }
    }
}