using Microsoft.AspNetCore.Mvc;
using HospitalAppl.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HospitalAppl.Controllers.Admin
{
    public class RoomTypeController : Controller
    {


        private readonly IConfiguration _config;

        public RoomTypeController(IConfiguration config)
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
                using (DataTable RoomTypes = RoomType.SelectRoomTypes(_config))
                {
                    ViewBag.RoomTypea = null;
                    if (RoomTypes.Rows.Count > 0)
                    {
                        ViewBag.RoomTypes = RoomTypes.Rows;
                    }

                    return View("~/Views/Admin/RoomType/Index.cshtml");
                }
            }
            catch (Exception)
            {
                TempData["DangerMessage"] = "Database Error";
                return View("~/Views/Admin/RoomType/Index.cshtml");
            }

        }

        public IActionResult Create()
        {
            if(HttpContext.Session.GetString("Username") == null){
                TempData["DangerMessage"] = "No Session Exists What Are You Doing ?!!";
                return RedirectToAction("Index","Home");
            }
            return View("~/Views/Admin/RoomType/Create.cshtml");
        }

        public IActionResult Edit(string param)
        {
            if(HttpContext.Session.GetString("Username") == null){
                TempData["DangerMessage"] = "No Session Exists What Are You Doing ?!!";
                return RedirectToAction("Index","Home");
            }
            try
            {
                using (DataTable RoomTypes = RoomType.SelectRoomTypes(_config, param))
                {
                    
                    if(RoomTypes.Rows.Count > 0)
                    {
                        ViewBag.RoomType = RoomTypes.Rows[0];
                    }
                    else
                    {
                        TempData["DangerMessage"] = "No Room Type Found";
                        return RedirectToAction("Index");
                    }

                    return View("~/Views/Admin/RoomType/Edit.cshtml");
                }
            }
            catch (Exception)
            {
                TempData["DangerMessage"] = "Database Error";
                return View("~/Views/Admin/RoomType/Edit.cshtml");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Room_Type_Name,Room_Prefix")] RoomType RT)
        {

                try
                {
                    int rowsAff = RT.InsertRoomType(_config);
                    TempData["SuccessMessage"] = "Successfully Added " + RT.Room_Type_Name + " To the system";

                    return View("~/Views/Admin/RoomType/Create.cshtml");
                }
                catch (Exception)
                {
                    TempData["DangerMessage"] = "Couldn't Add " + RT.Room_Type_Name + "(" + RT.Room_Prefix + ") To the system, please check the data entered and avoid entering a repeated room type name";
                    return View("~/Views/Admin/RoomType/Create.cshtml");
                }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public object Edit(string Old_Room_Type_Name, [Bind("Room_Type_Name,Room_Prefix")] RoomType RT)
        {
                try
                {
                    int rowsAff = RT.UpdateRoomType(_config, Old_Room_Type_Name);
                    TempData["SuccessMessage"] = "Successfully Updated " + RT.Room_Type_Name + " To the system";

                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    TempData["DangerMessage"] = "Couldn't Update " + Old_Room_Type_Name + "(" + RT.Room_Prefix + ") To the system, please check the data entered and avoid entering a repeated room type name";

                    return RedirectToAction("Index");
                }

        }
    }
}
