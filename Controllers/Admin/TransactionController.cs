using Microsoft.AspNetCore.Mvc;
using HospitalAppl.Models;
using System.Data;

namespace HospitalAppl.Controllers.Admin
{
    public class TransactionController : Controller
    {
        private readonly IConfiguration _config;

        public TransactionController(IConfiguration config)
        {
            _config = config;
        }
        public IActionResult Index()
        {
            if(HttpContext.Session.GetString("Username") == null){
                TempData["DangerMessage"] = "No Session Exists What Are You Doing ?!!";
                return RedirectToAction("Index","Home");
            }
            DataTable Transactions = FinancialTransaction.SelectOpenTransactions(_config);
            ViewBag.Transactions = Transactions.Rows;

            return View("~/Views/Admin/Transactions/index.cshtml");
        }
        public IActionResult Pay(string param, DateTime param2)
        {
            if(HttpContext.Session.GetString("Username") == null){
                TempData["DangerMessage"] = "No Session Exists What Are You Doing ?!!";
                return RedirectToAction("Index","Home");
            }
            if(HttpContext.Session.GetString("Username") == null){
                TempData["DangerMessage"] = "No Session Exists What Are You Doing ?!!";
                return RedirectToAction("Index","Home");
            }
            DataTable Visits = Visit.SelectVisits(_config, param, param2);
            ViewBag.Visit = null;
            if(Visits.Rows.Count > 0)
            {
                ViewBag.Visit = Visits.Rows[0];
            }
            DataTable TUser = Patient.SelectPatient(_config, param);
            ViewBag.TUser = null;
            if (TUser.Rows.Count > 0)
            {
                ViewBag.TUser = TUser.Rows[0];
            }
            DataTable TransRec = FinancialTransaction.SelectOpenTransactions(_config, param, param2);
            ViewBag.TransRec = null;
            if (TransRec.Rows.Count > 0)
            {
                ViewBag.TransRec = TransRec.Rows[0];
            }
            return View("~/Views/Admin/Transactions/pay.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Pay(FinancialTransaction FN)
        {

            try
            {
                FN.InsertTransaction(_config);
                TempData["SuccessMessage"] = "Transaction Paid Successfully!";
            }
            catch (Exception e)
            {

                TempData["DangerMessage"] = "Failed To Complete Your Transaction Try Again Later!";
                throw;
            }

            return RedirectToAction("Index");
        }
    }
}
