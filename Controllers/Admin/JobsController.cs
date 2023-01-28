using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HospitalAppl.Models;
using HospitalAppl.Data;    
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using HospitalAppl.Libraies;
using System.IO;
using System.Xml.Linq;
using System.Data;
using HospitalAppl.Libraies;

namespace HospitalAppl.Controllers.Admin;

public class JobsController : Controller
{
    private readonly ILogger<JobsController> _logger;

    private readonly IConfiguration _config;

    public JobsController(ILogger<JobsController> logger,IConfiguration config)
    {
        _logger = logger;
        _config = config;
    }

    public IActionResult Index(){
        if(HttpContext.Session.GetString("Username") == null){
            TempData["DangerMessage"] = "No Session Exists What Are You Doing ?!!";
            return RedirectToAction("Index","Home");
        }
  
        DataTable Jobs = new DataTable();
        SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        var SelectJobQuery = "SELECT * FROM Jobs";
        SqlCommand SelectJobsCmd = new SqlCommand(SelectJobQuery,con);

        con.Open();
        SqlDataReader sdr = SelectJobsCmd.ExecuteReader();

        if (sdr.HasRows)
        {
            Jobs.Load(sdr);
            sdr.Close();
        }
        con.Close();

        return View("/Views/Admin/Jobs/Index.cshtml", Jobs);
    }

    public IActionResult CreateJob(){
        if(HttpContext.Session.GetString("Username") == null){
            TempData["DangerMessage"] = "No Session Exists What Are You Doing ?!!";
            return RedirectToAction("Index","Home");
        }
        return View("/Views/Admin/Jobs/CreateJob.cshtml");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CreateJob(Job JobObj){

        if (Job.CheckValidJobPk(_config, JobObj.Job_Name))
        {
            SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            con.Open();
            var insertJobQuery = "INSERT INTO Jobs VALUES (@Name)";
            SqlCommand insertJobCmd = new SqlCommand(insertJobQuery, con);
            // Binding Parameters For insertJobQuery

            insertJobCmd.Parameters.AddWithValue("@Name", JobObj.Job_Name);

            insertJobCmd.ExecuteNonQuery();
            con.Close();
            TempData["SuccessMessage"] = "Success";
            return RedirectToAction("Index");

        }
        else
        {
            TempData["DangerMessage"] = "Failure";
            return RedirectToAction("Index");
        }
    }

    [HttpGet]
    public IActionResult EditJob(string? jobName){
        if(HttpContext.Session.GetString("Username") == null){
            TempData["DangerMessage"] = "No Session Exists What Are You Doing ?!!";
            return RedirectToAction("Index","Home");
        }
        if (jobName == null)
        {
            TempData["DangerMessage"] = "Failure";
            return RedirectToAction("Index");
        }
        DataTable Jobs = new DataTable();
        SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        var SelectJobQuery = "SELECT * FROM Jobs WHERE Job_Name = @Name";
        SqlCommand SelectJobsCmd = new SqlCommand(SelectJobQuery,con);
        SelectJobsCmd.Parameters.AddWithValue("@Name", jobName);

        con.Open();
        SqlDataReader sdr = SelectJobsCmd.ExecuteReader();

        if (sdr.HasRows)
        {
            Jobs.Load(sdr);
            sdr.Close();
            con.Close();
        }
        else
        {
            con.Close();
            TempData["DangerMessage"] = "Failure";
            return RedirectToAction("Index");
        }

        DataRow JobRow = Jobs.Rows[0];
        Job JobObj = new Job(JobRow);

        return View("/Views/Admin/Jobs/EditJob.cshtml", JobObj);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EditJob(Job JobObj,string PrevName){
        SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        con.Open();
        var UpdateJobQuery = "Update Jobs SET Job_Name = @Name WHERE Job_Name = @PrevName";
        SqlCommand UpdateJobCmd = new SqlCommand(UpdateJobQuery, con);
        // Binding Parameters For insertJobQuery

        UpdateJobCmd.Parameters.AddWithValue("@Name", JobObj.Job_Name);
        UpdateJobCmd.Parameters.AddWithValue("@PrevName", PrevName);

        if(Job.CheckValidJobPk(_config,JobObj.Job_Name)){
            UpdateJobCmd.ExecuteNonQuery();
            con.Close();
            TempData["SuccessMessage"] = "Success";        
            return RedirectToAction("Index");
        }
        con.Close();
        TempData["DangerMessage"] = "Failure";        
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult DeleteJob(string? jobName){

        if(jobName != null && Job.GetNumWorkers(_config,jobName) == 0){
            SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            con.Open();
            string DeleteJobQuery = "DELETE FROM Jobs WHERE Job_Name = @JobName";
            SqlCommand UpdateJobCmd = new SqlCommand(DeleteJobQuery, con);
            // Binding Parameters For DeleteJobQuery

            UpdateJobCmd.Parameters.AddWithValue("@JobName", jobName);
            UpdateJobCmd.ExecuteNonQuery();
            con.Close();
            TempData["SuccessMessage"] = "Success";
            return RedirectToAction("Index");
        }
        
        TempData["DangerMessage"] = "Failure";
        return RedirectToAction("Index");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
