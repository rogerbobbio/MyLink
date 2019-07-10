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
    public class PaymentsController : Controller
    {
        private MyLinkContext db = new MyLinkContext();

        public ActionResult Index(string account, string personCharge, string bank, string providerFlag, string sortOrder, int? projectId, int? page = null)
        {
            ViewBag.BankId = new SelectList(CombosHelper.GetBanks(), "BankId", "Name");
            ViewBag.ProjectId = new SelectList(CombosHelper.GetProjects(), "ProjectId", "Name");
            ViewBag.ProjectSelectedId = projectId;
            ViewBag.PersonChargeSelected = personCharge;
            ViewBag.AccountSelected = account;
            ViewBag.BankSelected = bank;
            ViewBag.ProviderSelectedFlag = providerFlag;

            if (Convert.ToInt32(projectId) == 0) projectId = null;            
            if (Convert.ToBoolean(providerFlag) == false) providerFlag = null;

            var providerParameter = Convert.ToBoolean(providerFlag);

            page = (page ?? 1);
            var payments = db.Payments.Include(l => l.Project);
            payments = db.Payments.Where(c => (c.Account.Contains(account) || account == null)
                                    && (c.PersonCharge.Contains(personCharge) || personCharge == null)
                                    && (c.Bank.Contains(bank) || bank == null)
                                    && (c.ProjectId == projectId || projectId == null)
                                    && (c.ProviderFlag.Equals(providerParameter) || providerFlag == null)
                                );

            ViewBag.RecordCount = payments.ToList().Count;

            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "PersonCharge" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            switch (sortOrder)
            {
                case "account":
                    payments = payments.OrderByDescending(s => s.Account);
                    break;
                case "Date":
                    payments = payments.OrderBy(s => s.CreationDate);
                    break;
                case "date_desc":
                    payments = payments.OrderByDescending(s => s.CreationDate);
                    break;
                default:
                    payments = payments.OrderBy(s => s.PersonCharge);
                    break;
            }

            return View(payments.ToPagedList((int)page, 10));
        }

        public void ExportToExcel()
        {
            List<Payment> payments;
            payments = db.Payments.ToList();

            var pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add("Report");

            ws.Cells["A1"].Value = "Lista de Pagos";
            ws.Cells["A1"].Style.Font.Size = 20;
            ws.Cells["A1"].Style.Font.Bold = true;


            ws.Cells["A6"].Value = "Banco";
            ws.Cells["A6"].Style.Font.Bold = true;

            ws.Cells["B6"].Value = "Cuenta";
            ws.Cells["B6"].Style.Font.Bold = true;

            ws.Cells["C6"].Value = "Monto";
            ws.Cells["C6"].Style.Font.Bold = true;

            var rowStart = 7;
            foreach (var item in payments)
            {
                ws.Cells[string.Format("A{0}", rowStart)].Value = item.Bank;
                ws.Cells[string.Format("B{0}", rowStart)].Value = item.Account;
                ws.Cells[string.Format("D{0}", rowStart)].Value = item.Amount;
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
            Payment payment = db.Payments.Find(id);
            if (payment == null)
            {
                return HttpNotFound();
            }
            return View(payment);
        }

        public ActionResult Create()
        {
            ViewBag.ProjectId = new SelectList(CombosHelper.GetProjects(), "ProjectId", "Name");
            var paymentModel = new Payment
            {
                Amount = 0,
                CreationDate = DateTime.Now,
            };
            return View(paymentModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Payment payment)
        {
            if (ModelState.IsValid)
            {
                db.Payments.Add(payment);
                var responseSave = DBHelper.SaveChanges(db);
                if (responseSave.Succeeded)
                {
                    if (payment.PaymentFile != null)
                    {
                        const string folder = "~/Content/Payments";
                        var file = string.Format("{0}.jpg", payment.PaymentId);

                        var response = FilesHelper.UploadPhoto(payment.PaymentFile, folder, file);
                        if (response)
                        {
                            var pic = string.Format("{0}/{1}", folder, file);
                            payment.Voucher = pic;
                            db.Entry(payment).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }

                    return RedirectToAction("Index");
                }
                ModelState.AddModelError(string.Empty, responseSave.Message);
            }

            ViewBag.ProjectId = new SelectList(CombosHelper.GetProjects(), "ProjectId", "Name", payment.ProjectId);
            return View(payment);
        }
        
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Payment payment = db.Payments.Find(id);
            if (payment == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProjectId = new SelectList(CombosHelper.GetProjects(), "ProjectId", "Name", payment.ProjectId);
            return View(payment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Payment payment)
        {
            if (ModelState.IsValid)
            {
                if (payment.PaymentFile != null)
                {
                    var pic = string.Empty;
                    const string folder = "~/Content/Payments";
                    var file = string.Format("{0}.jpg", payment.PaymentId);
                    var response = FilesHelper.UploadPhoto(payment.PaymentFile, folder, file);
                    if (response)
                    {
                        pic = string.Format("{0}/{1}.", folder, file);
                        payment.Voucher = pic;
                    }
                }

                db.Entry(payment).State = EntityState.Modified;
                var responseSave = DBHelper.SaveChanges(db);
                if (responseSave.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError(string.Empty, responseSave.Message);
            }
            ViewBag.ProjectId = new SelectList(CombosHelper.GetProjects(), "ProjectId", "Name", payment.ProjectId);
            return View(payment);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Payment payment = db.Payments.Find(id);
            if (payment == null)
            {
                return HttpNotFound();
            }
            return View(payment);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Payment payment = db.Payments.Find(id);
            db.Payments.Remove(payment);
            var responseSave = DBHelper.SaveChanges(db);
            if (responseSave.Succeeded)
            {
                return RedirectToAction("Index");
            }
            ModelState.AddModelError(string.Empty, responseSave.Message);
            return View(payment);
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
