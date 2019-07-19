using System.Linq;
using System.Web.Mvc;
using MyLink.Models;
using System.Collections.Generic;
using System;

namespace MyLink.Controllers
{
    public class HomeController : Controller
    {
        private MyLinkContext db = new MyLinkContext();

        public ActionResult Index()
        {
            var list = db.Links.ToList();

            List<int> categoriasCount = new List<int>();
            var categorias = list.Select(c => c.LinkCategory.Name).Distinct();
            foreach (var item in categorias)
            {
                categoriasCount.Add(list.Count(x => x.LinkCategory.Name == item));
            }            
            ViewBag.CATEGORIAS = categorias;
            ViewBag.CATEGORIASCOUNT = categoriasCount.ToList();

            List<int> rankingCount = new List<int>();
            List<string> rankingList = new List<string>();
            var rankings = list.Select(c => c.Ranking).Distinct();
            foreach (var item in rankings)
            {
                rankingCount.Add(list.Count(x => x.Ranking == item));
                rankingList.Add("Ranking " + item);
            }
            ViewBag.RANKINGS = rankingList;
            ViewBag.RANKINGSCOUNT = rankingCount.ToList();

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