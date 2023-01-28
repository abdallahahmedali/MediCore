using HospitalAppl.Libraies;
using HospitalAppl.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data;

namespace HospitalAppl.Controllers.Users
{
    public class SurgeryController : Controller
    {
        private readonly ILogger<SurgeryController> _logger;

        private readonly IConfiguration _config;

        public SurgeryController(ILogger<SurgeryController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        [HttpGet]
        public IActionResult Index(string? select,string? PatientUsername){
            if(HttpContext.Session.GetString("Username") == null){
                return RedirectToAction("Index","Home");
            }
            if(!string.IsNullOrEmpty(PatientUsername)){
                DataTable Surgeries;
                if(select == "Completed"){
                    Surgeries = new DataTable();
                    SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                    var SelectSurgeriesQuery = @"
                        SELECT * FROM Surgeries AS S
                        LEFT JOIN RoomTypes RT ON S.Room_Type_Name = RT.Room_Type_Name
                        INNER JOIN Users U ON S.Patient_Username = @Username AND U.Username = @Username
                        INNER JOIN Users A ON S.Doctor_Username = A.Username
                        WHERE Surgery_Date <= CAST(CURRENT_TIMESTAMP AS DATE)
                        AND Surgery_Is_Rejected = 0
                        AND Surgery_Is_Confirmed = 1;
                    ";
                    SqlCommand SelectSurgeriesCmd = new SqlCommand(SelectSurgeriesQuery,con);
                    SelectSurgeriesCmd.Parameters.AddWithValue("@Username", PatientUsername);

                    con.Open();
                    SqlDataReader sdr = SelectSurgeriesCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        Surgeries.Load(sdr);
                        sdr.Close();
                    }
                    con.Close();
                }else if(select == "Upcoming"){
                    Surgeries = new DataTable();
                    SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                    var SelectSurgeriesQuery = @"
                        SELECT * FROM Surgeries AS S
                        LEFT JOIN RoomTypes RT ON S.Room_Type_Name = RT.Room_Type_Name
                        INNER JOIN Users U ON S.Patient_Username = @Username AND U.Username = @Username
                        INNER JOIN Users A ON S.Doctor_Username = A.Username
                        WHERE Surgery_Date >= CAST(CURRENT_TIMESTAMP AS DATE)
                        AND Surgery_Is_Rejected = 0
                        AND Surgery_Is_Confirmed = 1;
                    ";
                    SqlCommand SelectSurgeriesCmd = new SqlCommand(SelectSurgeriesQuery,con);
                    SelectSurgeriesCmd.Parameters.AddWithValue("@Username", PatientUsername);

                    con.Open();
                    SqlDataReader sdr = SelectSurgeriesCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        Surgeries.Load(sdr);
                        sdr.Close();
                    }
                    con.Close();
                }else if(select == "Cancelled"){
                    Surgeries = new DataTable();
                    SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                    var SelectSurgeriesQuery = @"
                        SELECT * FROM Surgeries AS S
                        LEFT JOIN RoomTypes RT ON S.Room_Type_Name = RT.Room_Type_Name
                        INNER JOIN Users U ON S.Patient_Username = @Username AND U.Username = @Username
                        INNER JOIN Users A ON S.Doctor_Username = A.Username
                        WHERE Surgery_Date >= CAST(CURRENT_TIMESTAMP AS DATE)
                        AND Surgery_Is_Rejected = 1;
                    ";
                    SqlCommand SelectSurgeriesCmd = new SqlCommand(SelectSurgeriesQuery,con);
                    SelectSurgeriesCmd.Parameters.AddWithValue("@Username", PatientUsername);

                    con.Open();
                    SqlDataReader sdr = SelectSurgeriesCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        Surgeries.Load(sdr);
                        sdr.Close();
                    }
                    con.Close();
                }else if(select == "Awaiting"){
                    Surgeries = new DataTable();
                    SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                    var SelectSurgeriesQuery = @"
                        SELECT * FROM Surgeries AS S
                        LEFT JOIN RoomTypes RT ON S.Room_Type_Name = RT.Room_Type_Name
                        INNER JOIN Users U ON S.Patient_Username = @Username AND U.Username = @Username
                        INNER JOIN Users A ON S.Doctor_Username = A.Username
                        WHERE Surgery_Date >= CAST(CURRENT_TIMESTAMP AS DATE)
                        AND Surgery_Is_Rejected = 0
                        AND Surgery_Is_Confirmed = 0;
                    ";
                    SqlCommand SelectSurgeriesCmd = new SqlCommand(SelectSurgeriesQuery,con);
                    SelectSurgeriesCmd.Parameters.AddWithValue("@Username", PatientUsername);

                    con.Open();
                    SqlDataReader sdr = SelectSurgeriesCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        Surgeries.Load(sdr);
                        sdr.Close();
                    }
                    con.Close();
                }else if(select == "Cancelled"){
                    Surgeries = new DataTable();
                    SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                    var SelectSurgeriesQuery = @"
                        SELECT * FROM Surgeries AS S
                        LEFT JOIN RoomTypes RT ON S.Room_Type_Name = RT.Room_Type_Name
                        INNER JOIN Users U ON S.Patient_Username = @Username AND U.Username = @Username
                        INNER JOIN Users A ON S.Doctor_Username = A.Username
                        WHERE Surgery_Date >= CAST(CURRENT_TIMESTAMP AS DATE)
                        AND Surgery_Is_Rejected = 1;
                    ";
                    SqlCommand SelectSurgeriesCmd = new SqlCommand(SelectSurgeriesQuery,con);
                    SelectSurgeriesCmd.Parameters.AddWithValue("@Username", PatientUsername);

                    con.Open();
                    SqlDataReader sdr = SelectSurgeriesCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        Surgeries.Load(sdr);
                        sdr.Close();
                    }
                    con.Close();
                }else{
                    Surgeries = new DataTable();
                    SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                    var SelectSurgeriesQuery = @"
                        SELECT * FROM Surgeries AS S
                        LEFT JOIN RoomTypes RT ON S.Room_Type_Name = RT.Room_Type_Name
                        INNER JOIN Users U ON S.Patient_Username = @Username AND U.Username = @Username
                        INNER JOIN Users A ON S.Doctor_Username = A.Username
                    ";
                    SqlCommand SelectSurgeriesCmd = new SqlCommand(SelectSurgeriesQuery,con);
                    SelectSurgeriesCmd.Parameters.AddWithValue("@Username", PatientUsername);

                    con.Open();
                    SqlDataReader sdr = SelectSurgeriesCmd.ExecuteReader();

                    if (sdr.HasRows)
                    {
                        Surgeries.Load(sdr);
                        sdr.Close();
                    }
                    con.Close();
                }

                return View(Surgeries);                
            }else{
                DataTable Surgeries;
                if(HttpContext.Session.GetString("UserType") == "Employee"){
                    if(select == "Completed"){
                        Surgeries = new DataTable();
                        SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                        var SelectSurgeriesQuery = @"
                            SELECT * FROM Surgeries AS S
                            LEFT JOIN RoomTypes RT ON S.Room_Type_Name = RT.Room_Type_Name
                            INNER JOIN Users A ON S.Patient_Username = A.Username
                            INNER JOIN Users U ON S.Doctor_Username = @Username AND U.Username = @Username
                            WHERE Surgery_Date <= CAST(CURRENT_TIMESTAMP AS DATE)
                            AND Surgery_Is_Rejected = 0
                            AND Surgery_Is_Confirmed = 1;
                        ";
                        SqlCommand SelectSurgeriesCmd = new SqlCommand(SelectSurgeriesQuery,con);
                        SelectSurgeriesCmd.Parameters.AddWithValue("@Username", HttpContext.Session.GetString("Username"));

                        con.Open();
                        SqlDataReader sdr = SelectSurgeriesCmd.ExecuteReader();

                        if (sdr.HasRows)
                        {
                            Surgeries.Load(sdr);
                            sdr.Close();
                        }
                        con.Close();
                    }else if(select == "Upcoming"){
                        Surgeries = new DataTable();
                        SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                        var SelectSurgeriesQuery = @"
                            SELECT * FROM Surgeries AS S
                            LEFT JOIN RoomTypes RT ON S.Room_Type_Name = RT.Room_Type_Name
                            INNER JOIN Users A ON S.Patient_Username = A.Username
                            INNER JOIN Users U ON S.Doctor_Username = @Username AND U.Username = @Username
                            WHERE Surgery_Date >= CAST(CURRENT_TIMESTAMP AS DATE)
                            AND Surgery_Is_Rejected = 0
                            AND Surgery_Is_Confirmed = 1;
                        ";
                        SqlCommand SelectSurgeriesCmd = new SqlCommand(SelectSurgeriesQuery,con);
                        SelectSurgeriesCmd.Parameters.AddWithValue("@Username", HttpContext.Session.GetString("Username"));

                        con.Open();
                        SqlDataReader sdr = SelectSurgeriesCmd.ExecuteReader();

                        if (sdr.HasRows)
                        {
                            Surgeries.Load(sdr);
                            sdr.Close();
                        }
                        con.Close();
                    }else if(select == "Cancelled"){
                        Surgeries = new DataTable();
                        SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                        var SelectSurgeriesQuery = @"
                            SELECT * FROM Surgeries AS S
                            LEFT JOIN RoomTypes RT ON S.Room_Type_Name = RT.Room_Type_Name
                            INNER JOIN Users A ON S.Patient_Username = A.Username
                            INNER JOIN Users U ON S.Doctor_Username = @Username AND U.Username = @Username
                            WHERE Surgery_Date >= CAST(CURRENT_TIMESTAMP AS DATE)
                            AND Surgery_Is_Rejected = 1;
                        ";
                        SqlCommand SelectSurgeriesCmd = new SqlCommand(SelectSurgeriesQuery,con);
                        SelectSurgeriesCmd.Parameters.AddWithValue("@Username", HttpContext.Session.GetString("Username"));

                        con.Open();
                        SqlDataReader sdr = SelectSurgeriesCmd.ExecuteReader();

                        if (sdr.HasRows)
                        {
                            Surgeries.Load(sdr);
                            sdr.Close();
                        }
                        con.Close();
                    }else if(select == "Awaiting"){
                        Surgeries = new DataTable();
                        SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                        var SelectSurgeriesQuery = @"
                            SELECT * FROM Surgeries AS S
                            LEFT JOIN RoomTypes RT ON S.Room_Type_Name = RT.Room_Type_Name
                            INNER JOIN Users A ON S.Patient_Username = A.Username
                            INNER JOIN Users U ON S.Doctor_Username = @Username AND U.Username = @Username
                            WHERE Surgery_Date >= CAST(CURRENT_TIMESTAMP AS DATE)
                            AND Surgery_Is_Rejected = 0
                            AND Surgery_Is_Confirmed = 0;
                        ";
                        SqlCommand SelectSurgeriesCmd = new SqlCommand(SelectSurgeriesQuery,con);
                        SelectSurgeriesCmd.Parameters.AddWithValue("@Username", HttpContext.Session.GetString("Username"));

                        con.Open();
                        SqlDataReader sdr = SelectSurgeriesCmd.ExecuteReader();

                        if (sdr.HasRows)
                        {
                            Surgeries.Load(sdr);
                            sdr.Close();
                        }
                        con.Close();
                    }else if(select == "Cancelled"){
                        Surgeries = new DataTable();
                        SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                        var SelectSurgeriesQuery = @"
                            SELECT * FROM Surgeries AS S
                            LEFT JOIN RoomTypes RT ON S.Room_Type_Name = RT.Room_Type_Name
                            INNER JOIN Users A ON S.Patient_Username = A.Username
                            INNER JOIN Users U ON S.Doctor_Username = @Username AND U.Username = @Username
                            WHERE Surgery_Date >= CAST(CURRENT_TIMESTAMP AS DATE)
                            AND Surgery_Is_Rejected = 1;
                        ";
                        SqlCommand SelectSurgeriesCmd = new SqlCommand(SelectSurgeriesQuery,con);
                        SelectSurgeriesCmd.Parameters.AddWithValue("@Username", HttpContext.Session.GetString("Username"));

                        con.Open();
                        SqlDataReader sdr = SelectSurgeriesCmd.ExecuteReader();

                        if (sdr.HasRows)
                        {
                            Surgeries.Load(sdr);
                            sdr.Close();
                        }
                        con.Close();
                    }else{
                        Surgeries = new DataTable();
                        SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                        var SelectSurgeriesQuery = @"
                            SELECT * FROM Surgeries AS S
                            LEFT JOIN RoomTypes RT ON S.Room_Type_Name = RT.Room_Type_Name
                            INNER JOIN Users A ON S.Patient_Username = A.Username
                            INNER JOIN Users U ON S.Doctor_Username = @Username AND U.Username = @Username
                        ";
                        SqlCommand SelectSurgeriesCmd = new SqlCommand(SelectSurgeriesQuery,con);
                        SelectSurgeriesCmd.Parameters.AddWithValue("@Username", HttpContext.Session.GetString("Username"));

                        con.Open();
                        SqlDataReader sdr = SelectSurgeriesCmd.ExecuteReader();

                        if (sdr.HasRows)
                        {
                            Surgeries.Load(sdr);
                            sdr.Close();
                        }
                        con.Close();
                    }
                }else{
                   if(select == "Completed"){
                        Surgeries = new DataTable();
                        SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                        var SelectSurgeriesQuery = @"
                            SELECT * FROM Surgeries AS S
                            LEFT JOIN RoomTypes RT ON S.Room_Type_Name = RT.Room_Type_Name
                            INNER JOIN Users U ON S.Patient_Username = @Username AND U.Username = @Username
                            INNER JOIN Users A ON S.Doctor_Username = A.Username
                            WHERE Surgery_Date <= CAST(CURRENT_TIMESTAMP AS DATE)
                            AND Surgery_Is_Rejected = 0
                            AND Surgery_Is_Confirmed = 1;
                        ";
                        SqlCommand SelectSurgeriesCmd = new SqlCommand(SelectSurgeriesQuery,con);
                        SelectSurgeriesCmd.Parameters.AddWithValue("@Username", HttpContext.Session.GetString("Username"));

                        con.Open();
                        SqlDataReader sdr = SelectSurgeriesCmd.ExecuteReader();

                        if (sdr.HasRows)
                        {
                            Surgeries.Load(sdr);
                            sdr.Close();
                        }
                        con.Close();
                    }else if(select == "Upcoming"){
                        Surgeries = new DataTable();
                        SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                        var SelectSurgeriesQuery = @"
                            SELECT * FROM Surgeries AS S
                            LEFT JOIN RoomTypes RT ON S.Room_Type_Name = RT.Room_Type_Name
                            INNER JOIN Users U ON S.Patient_Username = @Username AND U.Username = @Username
                            INNER JOIN Users A ON S.Doctor_Username = A.Username
                            WHERE Surgery_Date >= CAST(CURRENT_TIMESTAMP AS DATE)
                            AND Surgery_Is_Rejected = 0
                            AND Surgery_Is_Confirmed = 1;
                        ";
                        SqlCommand SelectSurgeriesCmd = new SqlCommand(SelectSurgeriesQuery,con);
                        SelectSurgeriesCmd.Parameters.AddWithValue("@Username", HttpContext.Session.GetString("Username"));

                        con.Open();
                        SqlDataReader sdr = SelectSurgeriesCmd.ExecuteReader();

                        if (sdr.HasRows)
                        {
                            Surgeries.Load(sdr);
                            sdr.Close();
                        }
                        con.Close();
                    }else if(select == "Cancelled"){
                        Surgeries = new DataTable();
                        SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                        var SelectSurgeriesQuery = @"
                            SELECT * FROM Surgeries AS S
                            LEFT JOIN RoomTypes RT ON S.Room_Type_Name = RT.Room_Type_Name
                            INNER JOIN Users U ON S.Patient_Username = @Username AND U.Username = @Username
                            INNER JOIN Users A ON S.Doctor_Username = A.Username
                            WHERE Surgery_Date >= CAST(CURRENT_TIMESTAMP AS DATE)
                            AND Surgery_Is_Rejected = 1;
                        ";
                        SqlCommand SelectSurgeriesCmd = new SqlCommand(SelectSurgeriesQuery,con);
                        SelectSurgeriesCmd.Parameters.AddWithValue("@Username", HttpContext.Session.GetString("Username"));

                        con.Open();
                        SqlDataReader sdr = SelectSurgeriesCmd.ExecuteReader();

                        if (sdr.HasRows)
                        {
                            Surgeries.Load(sdr);
                            sdr.Close();
                        }
                        con.Close();
                    }else if(select == "Awaiting"){
                        Surgeries = new DataTable();
                        SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                        var SelectSurgeriesQuery = @"
                            SELECT * FROM Surgeries AS S
                            LEFT JOIN RoomTypes RT ON S.Room_Type_Name = RT.Room_Type_Name
                            INNER JOIN Users U ON S.Patient_Username = @Username AND U.Username = @Username
                            INNER JOIN Users A ON S.Doctor_Username = A.Username
                            WHERE Surgery_Date >= CAST(CURRENT_TIMESTAMP AS DATE)
                            AND Surgery_Is_Rejected = 0
                            AND Surgery_Is_Confirmed = 0;
                        ";
                        SqlCommand SelectSurgeriesCmd = new SqlCommand(SelectSurgeriesQuery,con);
                        SelectSurgeriesCmd.Parameters.AddWithValue("@Username", HttpContext.Session.GetString("Username"));

                        con.Open();
                        SqlDataReader sdr = SelectSurgeriesCmd.ExecuteReader();

                        if (sdr.HasRows)
                        {
                            Surgeries.Load(sdr);
                            sdr.Close();
                        }
                        con.Close();
                    }else if(select == "Cancelled"){
                        Surgeries = new DataTable();
                        SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                        var SelectSurgeriesQuery = @"
                            SELECT * FROM Surgeries AS S
                            LEFT JOIN RoomTypes RT ON S.Room_Type_Name = RT.Room_Type_Name
                            INNER JOIN Users U ON S.Patient_Username = @Username AND U.Username = @Username
                            INNER JOIN Users A ON S.Doctor_Username = A.Username
                            WHERE Surgery_Date >= CAST(CURRENT_TIMESTAMP AS DATE)
                            AND Surgery_Is_Rejected = 1;
                        ";
                        SqlCommand SelectSurgeriesCmd = new SqlCommand(SelectSurgeriesQuery,con);
                        SelectSurgeriesCmd.Parameters.AddWithValue("@Username", HttpContext.Session.GetString("Username"));

                        con.Open();
                        SqlDataReader sdr = SelectSurgeriesCmd.ExecuteReader();

                        if (sdr.HasRows)
                        {
                            Surgeries.Load(sdr);
                            sdr.Close();
                        }
                        con.Close();
                    }else{
                        Surgeries = new DataTable();
                        SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                        var SelectSurgeriesQuery = @"
                            SELECT * FROM Surgeries AS S
                            LEFT JOIN RoomTypes RT ON S.Room_Type_Name = RT.Room_Type_Name
                            INNER JOIN Users U ON S.Patient_Username = @Username AND U.Username = @Username
                            INNER JOIN Users A ON S.Doctor_Username = A.Username
                        ";
                        SqlCommand SelectSurgeriesCmd = new SqlCommand(SelectSurgeriesQuery,con);
                        SelectSurgeriesCmd.Parameters.AddWithValue("@Username", HttpContext.Session.GetString("Username"));

                        con.Open();
                        SqlDataReader sdr = SelectSurgeriesCmd.ExecuteReader();

                        if (sdr.HasRows)
                        {
                            Surgeries.Load(sdr);
                            sdr.Close();
                        }
                        con.Close();
                    }
                }

                return View(Surgeries); 
            }
        }
        //Create Action
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Surgery SurgeryObj)
        {
            if(HttpContext.Session.GetString("Username") == null){
                return RedirectToAction("Index","Home");
            }
            SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            con.Open();
            var query = "Insert into Surgeries Values(Visit_Start_Date,Patient_Username,Type_Name,Room_Type_Name,Room_Number,Doctor_Username,Surgery_Date,Surgery_Cost,Surgery_Is_Confirmed)(@Visit_Start_Date,@Patient_Username,@Type_Name,@Room_Type_Name,@Room_Number,@Doctor_Username,@Surgery_Date,@Surgery_Cost,@Surgery_Is_Confirmed);";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Visit_Start_Date", SurgeryObj.Visit_Start_Date);
            cmd.Parameters.AddWithValue("@Patient_Username", SurgeryObj.Patient_Username);
            cmd.Parameters.AddWithValue("@Type_Name", SurgeryObj.Type_Name);
            cmd.Parameters.AddWithValue("@Room_Type_Name", SurgeryObj.Room_Type_Name);
            cmd.Parameters.AddWithValue("@Room_Number", SurgeryObj.Room_Number);
            cmd.Parameters.AddWithValue("@Doctor_Username", SurgeryObj.Doctor_Username);
            cmd.Parameters.AddWithValue("@Surgery_Date", SurgeryObj.Surgery_Date);
            cmd.Parameters.AddWithValue("@Surgery_Cost", SurgeryObj.Surgery_Cost);
            cmd.Parameters.AddWithValue("@Surgery_Is_Confirmed", SurgeryObj.Surgery_Is_Confirmed);


            cmd.ExecuteNonQuery();
            con.Close();

            TempData["State"] = "Success";         //what to do with that
            return RedirectToAction("Index", "Home");

        }

        //Get User Type

        public int Surgery_user()  //return 1 if doctor ,0 if patient 
        {

            SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            con.Open();
            var query = "Select Count(*) From Employees Where Username = '" + ViewData["Username"] + "'";
            SqlCommand cmd = new SqlCommand(query, con);
            DataTable dt = new DataTable();
            int result = Convert.ToInt32(cmd.ExecuteScalar());
            con.Close();
            return result;

        }

        //Edit Action
        [HttpGet]
        public IActionResult Edit()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Surgery SurgeryObj)
        {
            SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            con.Open();
            var query = "UPDATE MedicalConsultations Set ";    // will be changed
            SqlCommand cmd = new SqlCommand(query, con);


            cmd.ExecuteNonQuery();
            con.Close();

            TempData["State"] = "Success";
            return RedirectToAction("Index", "Home");

        }

        //Detail 
        [HttpGet]
        public IActionResult Detail()
        {

            int user_type = Surgery_user();
            if (user_type == 0)     //patient
            {
                return RedirectToAction("Detail_Patient", "Surgery");
            }
            else
            {
                return RedirectToAction("Detail_Doctor", "Surgery");
            }
        }

        [HttpGet]
        public IActionResult Detail_Patient()
        {
            SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            con.Open();
            DataTable Details = new DataTable();
            var query = "Select Doctor_Username, Type_Name, Surgery_Date,Surgery_Cost,Surgery_Is_Confirmed From Surgeries; ";
            SqlCommand cmd = new SqlCommand(query, con);
            SqlDataReader sdr = cmd.ExecuteReader();

            if (sdr.HasRows)
            {
                Details.Load(sdr);
                sdr.Close();
            }
            con.Close();
            ViewData["Detail_Patient_Sur"] = Details;
            return View();
        }
        [HttpGet]
        public IActionResult Detail_Doctor_sur()
        {
            SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            con.Open();
            DataTable Details = new DataTable();
            var query = "Select Patient_Username, Type_Name, Surgery_Date From Surgeries; ";
            SqlCommand cmd = new SqlCommand(query, con);
            SqlDataReader sdr = cmd.ExecuteReader();

            if (sdr.HasRows)
            {
                Details.Load(sdr);
                sdr.Close();
            }
            con.Close();
            ViewData["Detail_Doctor_Sur"] = Details;
            return View();
        }

        //Delete Action


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            if(HttpContext.Session.GetString("Username") == null){
                return RedirectToAction("Index","Home");
            }
            SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            con.Open();
            var query = "Delete From Surgeries where ";    // will be changed
            SqlCommand cmd = new SqlCommand(query, con);

            cmd.ExecuteNonQuery();
            con.Close();

            TempData["State"] = "Success";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult CreateRequest(string? PatientUsername){
            if(HttpContext.Session.GetString("Username") == null){
                return RedirectToAction("Index","Home");
            }
            DataTable TypeNames = new DataTable();
            SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var SelectSurgeryTypesQuery = @"SELECT * FROM TypeDetails AS TD,ServiceTypes AS ST WHERE TD.Service_Type_Name = ST.Service_Type_Name AND ST.Service_Type_Name='Surgery'";
            SqlCommand SelectSurgeryTypesCmd = new SqlCommand(SelectSurgeryTypesQuery,con);

            con.Open();
            SqlDataReader sdr = SelectSurgeryTypesCmd.ExecuteReader();

            if (sdr.HasRows)
            {
                TypeNames.Load(sdr);
                sdr.Close();
            }
            con.Close();
            SurgeryVM SurgeryObj= new SurgeryVM(TypeNames); 
            return View(SurgeryObj);
        }

        [HttpPost]
        public IActionResult CreateRequest(SurgeryVM SurgeryObj){
            DateTime NewVisitStartDate = Visit.InitVisit(_config,SurgeryObj.Surgery.Patient_Username,Convert.ToDateTime(SurgeryObj.Surgery.Surgery_Date));
            SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            if(SurgeryObj.Surgery.Surgery_Date >= DateTime.Now){
                string InsertRequestQuery = "INSERT INTO Surgeries(Visit_Start_Date,Patient_Username,Surgery_Date,Type_Name,Doctor_Username) VALUES(@VisitStartDate,@Username,@Date,@Type,@DoctorUsername);";
                SqlCommand InsertRequestCmd = new SqlCommand(InsertRequestQuery, con);
                InsertRequestCmd.Parameters.AddWithValue("@Username", SurgeryObj.Surgery.Patient_Username);
                InsertRequestCmd.Parameters.AddWithValue("@Date", SurgeryObj.Surgery.Surgery_Date);
                InsertRequestCmd.Parameters.AddWithValue("@Type", SurgeryObj.Surgery.Type_Name);
                InsertRequestCmd.Parameters.AddWithValue("@VisitStartDate", NewVisitStartDate);
                InsertRequestCmd.Parameters.AddWithValue("@DoctorUsername", HttpContext.Session.GetString("Username"));
                con.Open();
                try{
                    InsertRequestCmd.ExecuteNonQuery();
                    con.Close();
                }catch(Exception){
                    con.Close();
                    TempData["DangerMessage"] = "Your "+ SurgeryObj.Surgery.Type_Name + " Request Failed Please Try Again later !";
                    return RedirectToAction("PatientProfile","Home", new { PatientUsername = SurgeryObj.Surgery.Patient_Username });     
                }

                TempData["SuccessMessage"] = "Your "+ SurgeryObj.Surgery.Type_Name + " Request Was Sent To The Admin !";

                return RedirectToAction("PatientProfile","Home", new { PatientUsername = SurgeryObj.Surgery.Patient_Username });
            }else{
                    TempData["DangerMessage"] = "Your "+ SurgeryObj.Surgery.Type_Name + " Request Failed Please Try Again later !";
                    return RedirectToAction("PatientProfile","Home", new { PatientUsername = SurgeryObj.Surgery.Patient_Username });     
            }
        }

    }
}
