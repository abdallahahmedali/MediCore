using Microsoft.AspNetCore.Mvc;
using HospitalAppl.Models;
using System.Data;

namespace HospitalAppl.Controllers.Admin
{
    public class AdminEmployeeController : Controller
    {
        private readonly IConfiguration _config;

        public AdminEmployeeController(IConfiguration config)
        {
            _config = config;
        }
        public IActionResult Index()
        {
            if(HttpContext.Session.GetString("Username") == null){
                TempData["DangerMessage"] = "No Session Exists What Are You Doing ?!!";
                return RedirectToAction("Index","Home");
            }
            DataTable Emps = Employee.SelectEmployees(_config);
            ViewBag.Employees = Emps.Rows;

            return View("~/Views/Admin/Employee/index.cshtml");
        }

        public IActionResult Promote(string param)
        {
            if(HttpContext.Session.GetString("Username") == null){
                TempData["DangerMessage"] = "No Session Exists What Are You Doing ?!!";
                return RedirectToAction("Index","Home");
            }
            try
            {
                int rowsAff = Employee.Promote(_config, param);
                TempData["SuccessMessage"] = "Successfully Promoted";

                return Redirect(HttpContext.Request.Headers["Referer"]);
            }
            catch (Exception e)
            {
                TempData["DangerMessage"] = "Failed To Promote";

                return Redirect(HttpContext.Request.Headers["Referer"]);
            }

        }
    }
}
