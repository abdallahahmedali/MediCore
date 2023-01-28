using Microsoft.AspNetCore.Mvc;
using HospitalAppl.Models;
using System.Data;

namespace HospitalAppl.Controllers.Admin
{
    public class TypeDetailController : Controller
    {
        private readonly IConfiguration _config;

        public TypeDetailController(IConfiguration config)
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
                using (DataTable TypeDetails = TypeDetail.SelectTypeDetails(_config))
                {
                    ViewBag.TypeDetails = null;
                    if (TypeDetails.Rows.Count > 0)
                    {
                        ViewBag.TypeDetails = TypeDetails.Rows;
                    }

                    return View("~/Views/Admin/TypeDetail/Index.cshtml");
                }
            }
            catch (Exception)
            {
                TempData["DangerMessage"] = "Database Error";
                return View("~/Views/Admin/TypeDetail/Index.cshtml");
            }

        }

        public IActionResult Create()
        {
            if(HttpContext.Session.GetString("Username") == null){
                TempData["DangerMessage"] = "No Session Exists What Are You Doing ?!!";
                return RedirectToAction("Index","Home");
            }
            DataTable ServiceTypes = ServiceType.SelectServiceTypes(_config);
            ViewBag.ServiceTypes = ServiceTypes.Rows;
            if (ServiceTypes.Rows.Count > 0)
            {
                return View("~/Views/Admin/TypeDetail/Create.cshtml");
            }
            TempData["DangerMessage"] = "Please add first a Service Type";
            return RedirectToAction("Create", "ServiceType");
        }

        public IActionResult Edit(string param, string sec_param)
        {
            if(HttpContext.Session.GetString("Username") == null){
                TempData["DangerMessage"] = "No Session Exists What Are You Doing ?!!";
                return RedirectToAction("Index","Home");
            }
            DataTable ServiceTypes = ServiceType.SelectServiceTypes(_config);
            if (ServiceTypes.Rows.Count > 0)
            {
                ViewBag.ServiceTypes = ServiceTypes.Rows;
            }
            else
            {
                TempData["DangerMessage"] = "No Servic Type Found, Please insert first a Service Type";
                return RedirectToAction("Index");
            }

            try
            {
                using (DataTable TypeDetails = TypeDetail.SelectTypeDetails(_config, param, sec_param))
                {

                    if (TypeDetails.Rows.Count > 0)
                    {
                        ViewBag.TypeDetails = TypeDetails.Rows[0];
                    }
                    else
                    {
                        TempData["DangerMessage"] = "No Type Details Found";
                        return RedirectToAction("Index");
                    }


                    return View("~/Views/Admin/TypeDetail/Edit.cshtml");
                }
            }
            catch (Exception)
            {
                TempData["DangerMessage"] = "Database Error";
                return RedirectToAction("Index");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public object Create([Bind("Service_Type_Name, Type_Name, Type_Cost")] TypeDetail TypeObj)
        {
            
                try
                {
                    int rowsAff = TypeObj.InsertTypeDetails(_config);
                    TempData["SuccessMessage"] = "Successfully Added " + TypeObj.Type_Name + " To the system";

                }
                catch (Exception)
                {
                    TempData["DangerMessage"] = "Couldn't Add " + TypeObj.Type_Name + "(" + TypeObj.Service_Type_Name + ") To the system, please check the data entered and avoid entering a repeated room type name";
                }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public object Edit(string Old_Service_Type_Name, string Old_Type_Name, [Bind("Service_Type_Name, Type_Name, Type_Cost")] TypeDetail TypeObj)
        {
                try
                {
                    int rowsAff = TypeObj.UpdateTypeDetail(_config, Old_Service_Type_Name, Old_Type_Name);
                    TempData["SuccessMessage"] = "Successfully Updated " + TypeObj.Type_Name + "(" + TypeObj.Service_Type_Name + ") To the system";

                }
                catch (Exception)
                {
                    TempData["DangerMessage"] = "Couldn't Update " + Old_Type_Name + "(" + Old_Service_Type_Name + ") To the system, please check the data entered and avoid entering a repeated room type name";

                }
            return RedirectToAction("Index");
        }
    }
}
