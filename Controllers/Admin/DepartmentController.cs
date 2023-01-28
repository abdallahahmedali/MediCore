using Microsoft.AspNetCore.Mvc;
using HospitalAppl.Models;
using System.Data;

namespace HospitalAppl.Controllers.Admin
{
    public class DepartmentController : Controller
    {
        private readonly IConfiguration _config;

        public DepartmentController(IConfiguration config)
        {
            _config = config;
        }

        public IActionResult Index()
        {
            if(HttpContext.Session.GetString("Username") == null){
                TempData["DangerMessage"] = "No Session Exists What Are You Doing ?!!";
                return RedirectToAction("Index","Home");
            }
            try
            {
                using (DataTable Departments = Department.SelectDepartments(_config))
                {
                    ViewBag.Departments = null;
                    if (Departments.Rows.Count > 0)
                    {
                        ViewBag.Departments = Departments.Rows;
                    }

                    return View("~/Views/Admin/Department/Index.cshtml");
                }
            }
            catch (Exception)
            {
                TempData["DangerMessage"] = "Database Error";
                return View("~/Views/Admin/Department/Index.cshtml");
            }

        }


        public IActionResult Create()
        {
            if(HttpContext.Session.GetString("Username") == null){
                TempData["DangerMessage"] = "No Session Exists What Are You Doing ?!!";
                return RedirectToAction("Index","Home");
            }
            return View("~/Views/Admin/Department/Create.cshtml");
        }


        public IActionResult Edit(string param)
        {
            if(HttpContext.Session.GetString("Username") == null){
                TempData["DangerMessage"] = "No Session Exists What Are You Doing ?!!";
                return RedirectToAction("Index","Home");
            }
            try
            {
                using (DataTable Departments = Department.SelectDepartments(_config, param))
                {

                    if (Departments.Rows.Count > 0)
                    {
                        ViewBag.Departments = Departments.Rows[0];
                    }
                    else
                    {
                        TempData["DangerMessage"] = "No Department Found";
                        return RedirectToAction("Index");
                    }

                    return View("~/Views/Admin/Department/Edit.cshtml");
                }
            }
            catch (Exception)
            {
                TempData["DangerMessage"] = "Database Error";
                return View("~/Views/Admin/Departmrnt/Edit.cshtml");
            }
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Department_Name")] Department Dept)
        {

           
                try
                {
                    int rowsAff = Dept.InsertDepartment(_config);
                    TempData["SuccessMessage"] = "Successfully Added " + Dept.Department_Name + " To the system";

                    return View("~/Views/Admin/Department/Create.cshtml");
                }
                catch (Exception)
                {
                    TempData["DangerMessage"] = "Couldn't Add " + Dept.Department_Name + " To the system, please check the data entered and avoid entering a repeated room type name";
                    return View("~/Views/Admin/Department/Create.cshtml");
                }

           
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public object Edit(string Old_Department_Name, [Bind("Department_Name")] Department Dept)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int rowsAff = Dept.UpdateDepartment(_config, Old_Department_Name);
                    TempData["SuccessMessage"] = "Successfully Updated " + Dept.Department_Name + " To the system";

                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    TempData["DangerMessage"] = "Couldn't Update " + Old_Department_Name + " To the system, please check the data entered and avoid entering a repeated room type name";

                    return RedirectToAction("Index");
                }

            }
            else
            {
                TempData["DangerMessage"] = "Invalid Data Entered or Maybe Some Data are Messing";
                return RedirectToAction("Index");
            }
        }

    }
}
