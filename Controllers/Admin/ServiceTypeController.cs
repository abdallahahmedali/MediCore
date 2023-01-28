using Microsoft.AspNetCore.Mvc;
using HospitalAppl.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HospitalAppl.Controllers.Admin
{
    public class ServiceTypeController : Controller
    {

        private readonly IConfiguration _config;

        public ServiceTypeController(IConfiguration config)
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
                using (DataTable ServiceTypes = ServiceType.SelectServiceTypes(_config))
                {
                    ViewBag.ServiceTypes = null;
                    if (ServiceTypes.Rows.Count > 0)
                    {
                        ViewBag.ServiceTypes = ServiceTypes.Rows;
                    }

                    return View("~/Views/Admin/ServiceType/Index.cshtml");
                }
            }
            catch (Exception)
            {
                TempData["DangerMessage"] = "Database Error";
                return View("~/Views/Admin/ServiceType/Index.cshtml");
            }

        }


        public IActionResult Create()
        {
            if(HttpContext.Session.GetString("Username") == null){
                TempData["DangerMessage"] = "No Session Exists What Are You Doing ?!!";
                return RedirectToAction("Index","Home");
            }
            return View("~/Views/Admin/ServiceType/Create.cshtml");
        }

        public IActionResult Edit(string param)
        {
            if(HttpContext.Session.GetString("Username") == null){
                TempData["DangerMessage"] = "No Session Exists What Are You Doing ?!!";
                return RedirectToAction("Index","Home");
            }
            try
            {
                using (DataTable ServiceTypes = ServiceType.SelectServiceTypes(_config, param))
                {

                    if (ServiceTypes.Rows.Count > 0)
                    {
                        ViewBag.ServiceTypes = ServiceTypes.Rows[0];
                    }
                    else
                    {
                        TempData["DangerMessage"] = "No Service Type Found";
                        return RedirectToAction("Index");
                    }

                    return View("~/Views/Admin/ServiceType/Edit.cshtml");
                }
            }
            catch (Exception)
            {
                TempData["DangerMessage"] = "Database Error";
                return View("~/Views/Admin/ServiceType/Edit.cshtml");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Service_Type_Name")] ServiceType Serv)
        {

                try
                {
                    int rowsAff = Serv.InsertServieType(_config);
                    TempData["SuccessMessage"] = "Successfully Added " + Serv.Service_Type_Name + " To the system";

                    return View("~/Views/Admin/ServiceType/Create.cshtml");
                }
                catch (Exception)
                {
                    TempData["DangerMessage"] = "Couldn't Add " + Serv.Service_Type_Name + " To the system, please check the data entered and avoid entering a repeated room type name";
                    return View("~/Views/Admin/ServiceType/Create.cshtml");
                }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public object Edit(string Old_Service_Type_Name, [Bind("Service_Type_Name")] ServiceType Serv)
        {

                try
                {
                    int rowsAff = Serv.UpdateServiceType(_config, Old_Service_Type_Name);
                    TempData["SuccessMessage"] = "Successfully Updated " + Serv.Service_Type_Name + " To the system";

                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    TempData["DangerMessage"] = "Couldn't Update " + Old_Service_Type_Name + " To the system, please check the data entered and avoid entering a repeated room type name";

                    return RedirectToAction("Index");
                }

        }




    }
}
