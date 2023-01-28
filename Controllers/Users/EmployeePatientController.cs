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
    public class EmployeePatientController : Controller
    {
        private readonly ILogger<EmployeePatientController> _logger;

        private readonly IConfiguration _config;

        public EmployeePatientController(ILogger<EmployeePatientController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        [HttpGet]
        public IActionResult Index(string? select){
            if(HttpContext.Session.GetString("Username") == null){
                return RedirectToAction("Index","Home");
            }
            DataTable Patients = new DataTable();
            if(select == "Checks"){
                SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                var SelectPatientsQuery = @"					    
					SELECT V.Patient_Username,First_Name,Middle_Name,Last_Name,Check_Date AS Date,Profile_Pic,ST.Service_Type_Name
					FROM Visits AS V,Checks AS C,Users AS U,ServiceTypes AS ST,TypeDetails AS TD
					WHERE Doctor_Username=@Username 
					AND U.Username=V.Patient_Username 
					AND V.Patient_Username = C.Patient_Username
                    AND V.Visit_Start_Date = C.Visit_Start_Date
					AND C.Type_Name = TD.Type_Name
					AND TD.Service_Type_Name=ST.Service_Type_Name
					AND C.Check_Date IN
					(
						SELECT MAX(C.Check_Date) 
						FROM Visits AS V,Checks AS C
						WHERE Doctor_Username=@Username 
						AND V.Patient_Username = C.Patient_Username
						GROUP BY(V.Patient_Username)
					);
                ";
                SqlCommand SelectPatientsCmd = new SqlCommand(SelectPatientsQuery,con);
                SelectPatientsCmd.Parameters.AddWithValue("@Username", HttpContext.Session.GetString("Username"));

                con.Open();
                SqlDataReader sdr = SelectPatientsCmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    Patients.Load(sdr);
                    sdr.Close();
                }
                con.Close();
            }else if(select == "Surgeries"){
                SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                var SelectPatientsQuery = @"
					SELECT V.Patient_Username,First_Name,Middle_Name,Last_Name,Surgery_Date AS Date,Profile_Pic,ST.Service_Type_Name
					FROM Visits AS V,Surgeries AS S,Users AS U,ServiceTypes AS ST,TypeDetails AS TD
					WHERE Doctor_Username=@Username 
					AND U.Username=V.Patient_Username 
					AND V.Patient_Username = S.Patient_Username
                    AND V.Visit_Start_Date = S.Visit_Start_Date
					AND S.Type_Name = TD.Type_Name
					AND TD.Service_Type_Name=ST.Service_Type_Name
					AND S.Surgery_Date IN
					(
						SELECT MAX(S.Surgery_Date) 
						FROM Visits AS V,Surgeries AS S
						WHERE Doctor_Username=@Username 
						AND V.Patient_Username = S.Patient_Username
						GROUP BY(V.Patient_Username)
					);
                ";
                SqlCommand SelectPatientsCmd = new SqlCommand(SelectPatientsQuery,con);
                SelectPatientsCmd.Parameters.AddWithValue("@Username", HttpContext.Session.GetString("Username"));

                con.Open();
                SqlDataReader sdr = SelectPatientsCmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    Patients.Load(sdr);
                    sdr.Close();
                }
                con.Close();
            }else if(select == "Tests"){
                SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                var SelectPatientsQuery = @"
					SELECT V.Patient_Username,First_Name,Middle_Name,Last_Name,Test_Date AS Date,Profile_Pic,ST.Service_Type_Name
					FROM Visits AS V,Tests AS T,Users AS U,ServiceTypes AS ST,TypeDetails AS TD
					WHERE Doctor_Username=@Username 
					AND U.Username=V.Patient_Username 
					AND V.Patient_Username = T.Patient_Username
                    AND V.Visit_Start_Date = T.Visit_Start_Date
					AND T.Type_Name = TD.Type_Name
					AND TD.Service_Type_Name=ST.Service_Type_Name
					AND T.Test_Date IN
					(
						SELECT MAX(T.Test_Date) 
						FROM Visits AS V,Tests AS T
						WHERE Doctor_Username=@Username 
						AND V.Patient_Username = T.Patient_Username
						GROUP BY(V.Patient_Username)
					)
				";
                SqlCommand SelectPatientsCmd = new SqlCommand(SelectPatientsQuery,con);
                SelectPatientsCmd.Parameters.AddWithValue("@Username", HttpContext.Session.GetString("Username"));

                con.Open();
                SqlDataReader sdr = SelectPatientsCmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    Patients.Load(sdr);
                    sdr.Close();
                }
                con.Close();
            }else{
                SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                var SelectPatientsQuery = @"
					SELECT SUBQ2.PUsername AS Patient_Username,First_Name,Middle_Name,Last_Name,SUBQ2.Date,SUBQ2.Profile_Pic,SUBQ2.ST_Name AS Service_Type_Name FROM
					(
						SELECT V.Patient_Username AS PUsername,First_Name,Middle_Name,Last_Name,Check_Date AS Date,Profile_Pic,ST.Service_Type_Name AS ST_Name
						FROM Visits AS V,Checks AS C,Users AS U,ServiceTypes AS ST,TypeDetails AS TD
						WHERE Doctor_Username=@Username 
						AND U.Username=V.Patient_Username 
						AND V.Patient_Username = C.Patient_Username
                        AND V.Visit_Start_Date = C.Visit_Start_Date
						AND C.Type_Name = TD.Type_Name
						AND TD.Service_Type_Name=ST.Service_Type_Name

					UNION

						SELECT V.Patient_Username,First_Name,Middle_Name,Last_Name,Surgery_Date AS Date,Profile_Pic,ST.Service_Type_Name
						FROM Visits AS V,Surgeries AS S,Users AS U,ServiceTypes AS ST,TypeDetails AS TD
						WHERE Doctor_Username=@Username 
						AND U.Username=V.Patient_Username 
						AND V.Patient_Username = S.Patient_Username
                        AND V.Visit_Start_Date = S.Visit_Start_Date
						AND S.Type_Name = TD.Type_Name
						AND TD.Service_Type_Name=ST.Service_Type_Name
					UNION

						SELECT V.Patient_Username,First_Name,Middle_Name,Last_Name,Test_Date AS Date,Profile_Pic,ST.Service_Type_Name
						FROM Visits AS V,Tests AS T,Users AS U,ServiceTypes AS ST,TypeDetails AS TD
						WHERE Doctor_Username=@Username 
						AND U.Username=V.Patient_Username 
						AND V.Patient_Username = T.Patient_Username
                        AND V.Visit_Start_Date = T.Visit_Start_Date
						AND T.Type_Name = TD.Type_Name
						AND TD.Service_Type_Name=ST.Service_Type_Name
					) AS SUBQ2
					WHERE SUBQ2.Date IN (
					SELECT MAX(SUBQ.ServiceDate) FROM
					(
							SELECT MAX(C.Check_Date) AS ServiceDate,V.Patient_Username AS P_Username 
							FROM Visits AS V,Checks AS C
							WHERE Doctor_Username=@Username 
							AND V.Patient_Username = C.Patient_Username
							GROUP BY(V.Patient_Username)

						UNION

							SELECT MAX(S.Surgery_Date) AS ServiceDate,V.Patient_Username AS P_Username 
							FROM Visits AS V,Surgeries AS S
							WHERE Doctor_Username=@Username 
							AND V.Patient_Username = S.Patient_Username
							GROUP BY(V.Patient_Username)

						UNION

							SELECT MAX(T.Test_Date) AS ServiceDate,V.Patient_Username AS P_Username 
							FROM Visits AS V,Tests AS T
							WHERE Doctor_Username=@Username 
							AND V.Patient_Username = T.Patient_Username
							GROUP BY(V.Patient_Username)

					) AS SUBQ GROUP BY (P_Username)

					)
                ";
                SqlCommand SelectPatientsCmd = new SqlCommand(SelectPatientsQuery,con);
                SelectPatientsCmd.Parameters.AddWithValue("@Username", HttpContext.Session.GetString("Username"));

                con.Open();
                SqlDataReader sdr = SelectPatientsCmd.ExecuteReader();

                if (sdr.HasRows)
                {
                    Patients.Load(sdr);
                    sdr.Close();
                }
                con.Close();
            }


            return View(Patients);
        }
        //Create Action
        [HttpGet]
        public IActionResult ViewPatient(string? username)
        {       
            if(HttpContext.Session.GetString("Username") == null){
                return RedirectToAction("Index","Home");
            }
            DataTable Patients = new DataTable();
            SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var SelectPatientsQuery = @"
                SELECT V.Patient_Username,Surgery_Date AS Date,Profile_Pic,ST.Service_Type_Name
                FROM Visits AS V,Surgeries AS S,Users AS U,ServiceTypes AS ST,TypeDetails AS TD
                WHERE Doctor_Username=@Username 
                AND U.Username=V.Patient_Username 
                AND V.Patient_Username = S.Patient_Username
                AND S.Type_Name = TD.Type_Name
                AND TD.Service_Type_Name=ST.Service_Type_Name
                AND S.Surgery_Date IN
                (
                    SELECT MAX(S.Surgery_Date) 
                    FROM Visits AS V,Surgeries AS S
                    WHERE Doctor_Username=@Username 
                    AND V.Patient_Username = S.Patient_Username
                    GROUP BY(V.Patient_Username)
                );
            ";
            SqlCommand SelectPatientsCmd = new SqlCommand(SelectPatientsQuery,con);
            SelectPatientsCmd.Parameters.AddWithValue("@Username", HttpContext.Session.GetString("Username"));

            con.Open();
            SqlDataReader sdr = SelectPatientsCmd.ExecuteReader();

            if (sdr.HasRows)
            {
                Patients.Load(sdr);
                sdr.Close();
            }else{
                con.Close();
                TempData["DangerMessage"]="Patient Doesn't Exist !";
                return  RedirectToAction("Index");
            }
            con.Close();
            ViewData["EmployeeUsername"] = HttpContext.Session.GetString("Username");
            return View(Patients.Rows[0]);
        }


    } 
}

