using HospitalAppl.Libraies;
using HospitalAppl.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data;


namespace HospitalAppl.Controllers.Users
{
    public class ConsultController : Controller
    {
        private readonly ILogger<ConsultController> _logger;

        private readonly IConfiguration _config;

        public ConsultController(ILogger<ConsultController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        [HttpGet]
        public IActionResult Index(string? select)
        {
            DataTable MedicalConsaltations;
            if(HttpContext.Session.GetString("Username") == null){
                return RedirectToAction("Index","Home");
            }
            if (select == "Completed")
            {
                MedicalConsaltations = new DataTable();
                SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                var SelectSurgeriesQuery = @"
                    SELECT Type_Name,Doctor_Username,R.Room_Number,RT.Room_Prefix,Profile_Pic,Check_Date,Consultation_Date,Consultation_Is_Confirmed,Consultation_Is_Rejected  
                    FROM MedicalConsultations AS MC,Rooms AS R,RoomTypes AS RT,Users AS U 
                    WHERE R.Room_Type_Name = RT.Room_Type_Name 
                    AND R.Room_Number = MC.Room_Number 
                    AND U.Username=MC.Doctor_Username
                    AND Patient_Username=@Username
                    AND Consultation_Date <= CAST(CURRENT_TIMESTAMP AS DATE)
                    AND Consultation_Is_Rejected=0
                    AND MC.Room_Type_Name= R.Room_Type_Name;
                ";
                SqlCommand SelectSurgeriesCmd = new SqlCommand(SelectSurgeriesQuery, con);
                SelectSurgeriesCmd.Parameters.AddWithValue("@Username", HttpContext.Session.GetString("Username"));

                con.Open();
                SqlDataReader sdr = SelectSurgeriesCmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    MedicalConsaltations.Load(sdr);
                    sdr.Close();
                }
                con.Close();
            }
            else if (select == "Upcoming")
            {
                MedicalConsaltations = new DataTable();
                SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                var SelectSurgeriesQuery = @"
                    SELECT Type_Name,Doctor_Username,R.Room_Number,RT.Room_Prefix,Profile_Pic,Check_Date,Consultation_Date,Consultation_Is_Confirmed,Consultation_Is_Rejected    
                    FROM MedicalConsultations AS MC,Rooms AS R,RoomTypes AS RT,Users AS U 
                    WHERE R.Room_Type_Name = RT.Room_Type_Name 
                    AND R.Room_Number = MC.Room_Number 
                    AND U.Username=MC.Doctor_Username
                    AND Patient_Username=@Username
                    AND Consultation_Date >= CAST(CURRENT_TIMESTAMP AS DATE)
                    AND Consultation_Is_Confirmed=1
                    AND MC.Room_Type_Name= R.Room_Type_Name;
                ";
                SqlCommand SelectSurgeriesCmd = new SqlCommand(SelectSurgeriesQuery, con);
                SelectSurgeriesCmd.Parameters.AddWithValue("@Username", HttpContext.Session.GetString("Username"));

                con.Open();
                SqlDataReader sdr = SelectSurgeriesCmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    MedicalConsaltations.Load(sdr);
                    sdr.Close();
                }
                con.Close();
            }
            else if (select == "Selected"){
                MedicalConsaltations = new DataTable();
                SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                var SelectSurgeriesQuery = @"
                    SELECT Type_Name,Doctor_Username,R.Room_Number,RT.Room_Prefix,Profile_Pic,Check_Date,Consultation_Date,Consultation_Is_Confirmed,Consultation_Is_Rejected    
                    FROM MedicalConsultations AS MC,Rooms AS R,RoomTypes AS RT,Users AS U 
                    WHERE R.Room_Type_Name = RT.Room_Type_Name 
                    AND R.Room_Number = MC.Room_Number 
                    AND U.Username=MC.Doctor_Username
                    AND Patient_Username=@Username
                    AND Consultation_Is_Rejected=1
                    AND MC.Room_Type_Name= R.Room_Type_Name;
                ";
                SqlCommand SelectSurgeriesCmd = new SqlCommand(SelectSurgeriesQuery, con);
                SelectSurgeriesCmd.Parameters.AddWithValue("@Username", HttpContext.Session.GetString("Username"));

                con.Open();
                SqlDataReader sdr = SelectSurgeriesCmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    MedicalConsaltations.Load(sdr);
                    sdr.Close();
                }
                con.Close();
            }
            else
            {
                MedicalConsaltations = new DataTable();
                SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                var SelectSurgeriesQuery = @"
                    SELECT Type_Name,Doctor_Username,R.Room_Number,RT.Room_Prefix,Profile_Pic,Check_Date,Consultation_Date,Consultation_Is_Confirmed,Consultation_Is_Rejected    
                    FROM MedicalConsultations AS MC,Rooms AS R,RoomTypes AS RT,Users AS U 
                    WHERE R.Room_Type_Name = RT.Room_Type_Name 
                    AND R.Room_Number = MC.Room_Number 
                    AND U.Username=MC.Doctor_Username
                    AND Patient_Username=@Username
                    AND MC.Room_Type_Name= R.Room_Type_Name;
                ";
                SqlCommand SelectSurgeriesCmd = new SqlCommand(SelectSurgeriesQuery, con);
                SelectSurgeriesCmd.Parameters.AddWithValue("@Username", HttpContext.Session.GetString("Username"));

                con.Open();
                SqlDataReader sdr = SelectSurgeriesCmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    MedicalConsaltations.Load(sdr);
                    sdr.Close();
                }
                con.Close();
            }
            
            return View(MedicalConsaltations);
        }

        // Create Action
        [HttpGet]
        public IActionResult Create(string? CID)
        {
            DataTable Check = new DataTable();
           if (!string.IsNullOrWhiteSpace(CID))
            {
                DataTable Consultations = new DataTable();
                SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                var SelectCheckDetailsQuery = @"
                    SELECT *
                    FROM Checks C
                    LEFT JOIN RoomTypes RT ON C.Room_Type_Name = RT.Room_Type_Name
                    WHERE Check_ID = @CID
                ";

                SqlCommand SelectCheckDetailsCmd = new SqlCommand(SelectCheckDetailsQuery,con);
                SelectCheckDetailsCmd.Parameters.AddWithValue("@CID",CID);

                con.Open();
                SqlDataReader sdr = SelectCheckDetailsCmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    Check.Load(sdr);
                    sdr.Close();
                    //Insert into DataBase
                    if(Convert.ToBoolean(Check.Rows[0]["Check_Has_Medical_Consultation"]) != true){
                        string query = "INSERT into MedicalConsultations (Visit_Start_Date,Patient_Username,Type_Name,Doctor_Username,Check_Date,Consultation_Date)Values(@VisitStartDate,@Patient_Username,@type,@Doctor_Username,@date,@ConsultationDate);";
                        string query2 = "UPDATE Checks SET Check_Has_Medical_Consultation = 1 WHERE Check_ID = @CID;";
                        SqlCommand cmd = new SqlCommand(query, con);
                        SqlCommand cmd2 = new SqlCommand(query2, con);
                        cmd.Parameters.AddWithValue("@date", Check.Rows[0]["Check_Date"]);
                        cmd.Parameters.AddWithValue("@ConsultationDate", Convert.ToDateTime(Check.Rows[0]["Check_Date"]).AddDays(7));
                        cmd.Parameters.AddWithValue("@VisitStartDate", Check.Rows[0]["Visit_Start_Date"]);
                        cmd.Parameters.AddWithValue("@Patient_Username", Check.Rows[0]["Patient_Username"]);
                        cmd.Parameters.AddWithValue("@type", Check.Rows[0]["Type_Name"]);
                        cmd.Parameters.AddWithValue("@Doctor_Username", Check.Rows[0]["Doctor_Username"]);
                        cmd2.Parameters.AddWithValue("@CID", Check.Rows[0]["Check_ID"]);
                        cmd.ExecuteNonQuery();
                        cmd2.ExecuteNonQuery();
                        con.Close();
            
                        TempData["SuccessMessage"] = "Consultation Booked Successfully !";
                        return RedirectToAction("CheckDetail","Check", new { CID = Check.Rows[0]["Check_ID"] });
                    }
                }
                con.Close();
                TempData["DangerMessage"] = "A Consultation Already Exists !";
                return RedirectToAction("CheckDetail","Check", new { CID = Check.Rows[0]["Check_ID"] });

            }

            TempData["DangerMessage"] = "Failed To Add Consultation !";
            return RedirectToAction("CheckDetail","Check", new { CID = Check.Rows[0]["Check_ID"] });
        }

        //Get User Type
        public int check_user()  //return 1 if doctor ,0 if patient 
        {

            SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            con.Open();
            var query = "Select Count(*) From Employees Where Username = '" + ViewData["Username"] + "'";
            SqlCommand cmd = new SqlCommand(query, con);
            int result = Convert.ToInt32(cmd.ExecuteScalar());
            con.Close();
            return result;

        }

        // Edit Action 

        [HttpGet]
        public IActionResult Edit()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(MedicalConsultation CheckObj)
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
            if(HttpContext.Session.GetString("Username") == null){
                return RedirectToAction("Index","Home");
            }
            int user_type = check_user();
            if (user_type == 0)     //patient
            {
                return RedirectToAction("Detail_Patient", "Consult");
            }
            else
            {
                return RedirectToAction("Detail_Doctor", "Consult");
            }
        }

        [HttpGet]
        public IActionResult Detail_Patient()
        {
            SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            con.Open();
            DataTable Details = new DataTable();
            var query = "Select Doctor_Username, Type_Name, Check_Date,Consultation_Date,Consultation_Cost From MedicalConsultations;";
            SqlCommand cmd = new SqlCommand(query, con);
            SqlDataReader sdr = cmd.ExecuteReader();

            if (sdr.HasRows)
            {
                Details.Load(sdr);
                sdr.Close();
            }
            con.Close();
            ViewData["Detail_Patient_Con"] = Details;
            return View();
        }
        [HttpGet]
        public IActionResult Detail_Doctor()
        {
            SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            con.Open();
            DataTable Details = new DataTable();
            var query = "Select Patient_Username,Type_Name, Check_Date,Consultation_Date From MedicalConsultations; ";
            SqlCommand cmd = new SqlCommand(query, con);
            SqlDataReader sdr = cmd.ExecuteReader();

            if (sdr.HasRows)
            {
                Details.Load(sdr);
                sdr.Close();
            }
            con.Close();
            ViewData["Detail_Doctor_Con"] = Details;
            return View();
        }


        //Delete Action

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            con.Open();
            var query = "Delete From MedicalConsultations where ";    // will be changed
            SqlCommand cmd = new SqlCommand(query, con);

            cmd.ExecuteNonQuery();
            con.Close();

            TempData["State"] = "Success";
            return RedirectToAction("Index", "Home");
        }




    }
}
