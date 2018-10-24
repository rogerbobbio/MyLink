using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Configuration;
using System.Web.Mvc;
using MyLink.Classes;
using MyLink.Models;
using OfficeOpenXml;
using PagedList;

namespace MyLink.Controllers
{
    [Authorize(Roles = "User, Admin")]
    public class UsersController : Controller
    {
        private MyLinkContext db = new MyLinkContext();

        public ActionResult Index(int? page = null)
        {
            page = (page ?? 1);
            IQueryable<User> users;
            var adminUser = WebConfigurationManager.AppSettings["AdminUser"];
            //Note: Include es un INNER JOIN
            if (adminUser == User.Identity.Name)
                users = db.Users
                    .Include(u => u.City)                    
                    .Include(u => u.Department)                    
                    .Include(u => u.UserRol).OrderBy(c => c.FirstName).ThenBy(c => c.LastName);
            else
            {
                //verifica el usuario logeado y filtra por su compania
                var user = db.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
                if (user == null)
                    return RedirectToAction("Index", "Home");

                users = db.Users.OrderBy(c => c.FirstName).ThenBy(c => c.LastName);
                //==================================================
            }
            
            return View(users.ToPagedList((int)page, 5));
        }

        public void ExportToExcel()
        {
            List<User> users;
            var adminUser = WebConfigurationManager.AppSettings["AdminUser"];
            if (adminUser == User.Identity.Name)
                users = db.Users.ToList();
            else
            {                
                users = db.Users.ToList();
            }
            
            var pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add("Report");

            ws.Cells["A1"].Value = "Lista de Usuarios";
            ws.Cells["A1"].Style.Font.Size = 20;
            ws.Cells["A1"].Style.Font.Bold = true;

            
            ws.Cells["A6"].Value = "User Name";
            ws.Cells["A6"].Style.Font.Bold = true;

            ws.Cells["B6"].Value = "Full Name";
            ws.Cells["B6"].Style.Font.Bold = true;

            ws.Cells["C6"].Value = "Proyecto";
            ws.Cells["C6"].Style.Font.Bold = true;

            ws.Cells["D6"].Value = "Rol";
            ws.Cells["D6"].Style.Font.Bold = true;

            ws.Cells["E6"].Value = "Phone";
            ws.Cells["E6"].Style.Font.Bold = true;

            ws.Cells["F6"].Value = "DNI";
            ws.Cells["F6"].Style.Font.Bold = true;

            var rowStart = 7;
            foreach (var item in users)
            {
                ws.Cells[string.Format("A{0}", rowStart)].Value = item.UserName;
                ws.Cells[string.Format("B{0}", rowStart)].Value = item.FullName;                
                ws.Cells[string.Format("D{0}", rowStart)].Value = item.UserRol.Name;
                ws.Cells[string.Format("E{0}", rowStart)].Value = item.Phone;
                ws.Cells[string.Format("F{0}", rowStart)].Value = item.Dni;
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
            var user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        public ActionResult Create()
        {
            ViewBag.CityId = new SelectList(CombosHelper.GetCities(0), "CityId", "Name");            
            ViewBag.DepartmentId = new SelectList(CombosHelper.GetDepartments(), "DepartmentId", "Name");            

            var adminUser = WebConfigurationManager.AppSettings["AdminUser"];
            if (adminUser == User.Identity.Name)
            {                
                ViewBag.UserRolId = new SelectList(CombosHelper.GetUserRols(), "UserRolId", "Name");
                var adminUserModel = new User
                {
                    State = true,                    
                    BirthDate = DateTime.Now,
                };
                return View(adminUserModel);
            }            
            //verifica el usuario logeado y envia su compania a la vista
            var user = db.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
            if (user == null)
                return RedirectToAction("Index", "Home");
            
            ViewBag.UserRolId = new SelectList(CombosHelper.GetUserRols(), "UserRolId", "Name");
            var userModel = new User
            {                
                BirthDate = DateTime.Now,
                State = true,
            };
            return View(userModel);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(User user)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(user);
                var responseSave = DBHelper.SaveChanges(db);
                if (responseSave.Succeeded)
                {
                    UsersHelper.CreateUserASP(user.UserName, "User");

                    if (user.PhotoFile != null)
                    {
                        const string folder = "~/Content/Users";
                        var file = string.Format("{0}.jpg", user.UserId);

                        var response = FilesHelper.UploadPhoto(user.PhotoFile, folder, file);
                        if (response)
                        {
                            var pic = string.Format("{0}/{1}", folder, file);
                            user.Photo = pic;
                            db.Entry(user).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }

                    return RedirectToAction("Index");
                }
                ModelState.AddModelError(string.Empty, responseSave.Message);                
            }

            ViewBag.CityId = new SelectList(CombosHelper.GetCities(user.DepartmentId), "CityId", "Name", user.CityId);            
            ViewBag.DepartmentId = new SelectList(CombosHelper.GetDepartments(), "DepartmentId", "Name", user.DepartmentId);            

            var adminUser = WebConfigurationManager.AppSettings["AdminUser"];
            if (adminUser == User.Identity.Name)
            {
                ViewBag.UserRolId = new SelectList(CombosHelper.GetUserRols(), "UserRolId", "Name", user.UserRolId);
            }
            else
            {
                var userIdentity = db.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);                
                ViewBag.UserRolId = new SelectList(CombosHelper.GetUserRols(), "UserRolId", "Name", user.UserRolId);
            }
            return View(user);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.CityId = new SelectList(CombosHelper.GetCities(user.DepartmentId), "CityId", "Name", user.CityId);            
            ViewBag.DepartmentId = new SelectList(CombosHelper.GetDepartments(), "DepartmentId", "Name", user.DepartmentId);            

            var adminUser = WebConfigurationManager.AppSettings["AdminUser"];
            if (adminUser == User.Identity.Name)
            {
                ViewBag.UserRolId = new SelectList(CombosHelper.GetUserRols(), "UserRolId", "Name", user.UserRolId);
            }
            else
            {                
                ViewBag.UserRolId = new SelectList(CombosHelper.GetUserRols(), "UserRolId", "Name", user.UserRolId);
            }

            return View(user);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(User user)
        {
            if (ModelState.IsValid)
            {                
                if (user.PhotoFile != null)
                {
                    var pic = string.Empty;
                    const string folder = "~/Content/Users";
                    var file = string.Format("{0}.jpg", user.UserId);
                    var response = FilesHelper.UploadPhoto(user.PhotoFile, folder, file);
                    if (response)
                    {
                        pic = string.Format("{0}/{1}.", folder, file);
                        user.Photo = pic;
                    }
                }

                var db2 = new MyLinkContext();
                var currentUser = db2.Users.Find(user.UserId);
                if (currentUser.UserName != user.UserName)
                {
                    UsersHelper.UpdateUserName(currentUser.UserName, user.UserName);
                }
                db2.Dispose();

                db.Entry(user).State = EntityState.Modified;
                var responseSave = DBHelper.SaveChanges(db);
                if (responseSave.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError(string.Empty, responseSave.Message);
            }
            ViewBag.CityId = new SelectList(CombosHelper.GetCities(user.DepartmentId), "CityId", "Name", user.CityId);
            ViewBag.DepartmentId = new SelectList(CombosHelper.GetDepartments(), "DepartmentId", "Name", user.DepartmentId);            

            var adminUser = WebConfigurationManager.AppSettings["AdminUser"];
            if (adminUser == User.Identity.Name)
            {
                ViewBag.UserRolId = new SelectList(CombosHelper.GetUserRols(), "UserRolId", "Name", user.UserRolId);
            }
            else
            {
                var userIdentity = db.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);                
                ViewBag.UserRolId = new SelectList(CombosHelper.GetUserRols(), "UserRolId", "Name", user.UserRolId);
            }
            return View(user);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var user = db.Users.Find(id);
            db.Users.Remove(user);
            var responseSave = DBHelper.SaveChanges(db);
            if (responseSave.Succeeded)
            {
                UsersHelper.DeleteUser(user.UserName, "User");
                return RedirectToAction("Index");
            }
            ModelState.AddModelError(string.Empty, responseSave.Message);
            return View(user);
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
