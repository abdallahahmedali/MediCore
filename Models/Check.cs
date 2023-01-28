using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;

namespace HospitalAppl.Models;

public partial class Check
{
    public Check(){}
   [Key]
    public int Check_ID { get; set; }
    public DateTime Visit_Start_Date { get; set; }
    public string Patient_Username { get; set; } = null!;
    public string Type_Name { get; set; } = null!;
    public string Room_Type_Name { get; set; } = null!;
    public int Room_Number { get; set; }
    public string? Doctor_Username { get; set; } = null!;

    public DateTime Check_Date { get; set; } = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd 00:00:00"));

    public bool Check_Has_Medical_Consultation { get; set; }

    public double Check_Cost { get; set; }

    public bool Check_Is_Confirmed { get; set; }
    public bool? Check_Is_Rejected { get; set; }

    public string? Check_Details { get; set; } = null!;

    public string EmployeeUsername { get; set; } = null!;
    public string SelectedDepartmentName { get; set; } = null!;
    public IEnumerable<SelectListItem> Departments { get; set; } = null!;
    public IEnumerable<SelectListItem> Doctors { get; set; }
    public DataTable Checks { get; set; }
    public DataTable Consultations { get; set; }

    //public virtual TypeDetail TypeNameNavigation { get; set; } = null!;
  public static int Approve(IConfiguration config, int _id)
    {
        //DataTable reservations = SelectRoomReservations(config);
        // update code
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DBCon.Open();
        string query = "Update Checks SET Check_Is_Confirmed = 1 " +
            "WHERE Check_ID = @Check_ID";
        SqlCommand Command = new SqlCommand(query, DBCon);
        Command.Parameters.AddWithValue("@Check_ID", _id);
        int rowsAff = Command.ExecuteNonQuery();
        DBCon.Close();
        return rowsAff;
    }

    public static int Delete(IConfiguration config, int _id)
    {
        //DataTable reservations = SelectRoomReservations(config);
        // update code
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DBCon.Open();
        string query = "Update Checks SET Check_Is_Rejected = 1 " +
            "WHERE Check_ID = @Check_ID";
        SqlCommand Command = new SqlCommand(query, DBCon);
        Command.Parameters.AddWithValue("@Check_ID", _id);
        int rowsAff = Command.ExecuteNonQuery();
        DBCon.Close();
        return rowsAff;
    }

    public static DataTable SelectChecks(IConfiguration config)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DataTable RoomRes = new DataTable();
        DBCon.Open();
        string query = "SELECT * FROM Checks " +
            "LEFT JOIN RoomTypes ON RoomTypes.Room_Type_Name = Checks.Room_Type_Name";
        SqlCommand Command = new SqlCommand(query, DBCon);
        SqlDataReader sdr = Command.ExecuteReader();
        if (sdr.HasRows)
        {
            RoomRes.Load(sdr);
            sdr.Close();
        }
        DBCon.Close();

        return RoomRes;
    }
    public static DataTable SelectActiveChecks(IConfiguration config)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DataTable RoomRes = new DataTable();
        DBCon.Open();
        string query = "SELECT * FROM Checks " +
            "LEFT JOIN RoomTypes ON RoomTypes.Room_Type_Name = Checks.Room_Type_Name " +
            "WHERE Check_Is_Confirmed = 0 AND Check_Is_Rejected = 0";
        SqlCommand Command = new SqlCommand(query, DBCon);
        SqlDataReader sdr = Command.ExecuteReader();
        if (sdr.HasRows)
        {
            RoomRes.Load(sdr);
            sdr.Close();
        }
        DBCon.Close();

        return RoomRes;
    }

    public static DataTable SelectVisitActiveChecks(IConfiguration config, string Username, DateTime Visit_Start_Date)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DataTable RoomRes = new DataTable();
        DBCon.Open();
        string query = "SELECT * FROM Checks " +
            "LEFT JOIN RoomTypes ON RoomTypes.Room_Type_Name = Checks.Room_Type_Name " +
            "WHERE Checks.Patient_Username = @Username AND Checks.Visit_Start_Date = @Visit_Start_Date";
        SqlCommand Command = new SqlCommand(query, DBCon);
        Command.Parameters.AddWithValue("@Username", Username);
        Command.Parameters.AddWithValue("@Visit_Start_Date", Visit_Start_Date);
        SqlDataReader sdr = Command.ExecuteReader();
        if (sdr.HasRows)
        {
            RoomRes.Load(sdr);
            sdr.Close();
        }
        DBCon.Close();

        return RoomRes;
    }
    public static DataTable SelectApprovedChecks(IConfiguration config)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DataTable RoomRes = new DataTable();
        DBCon.Open();
        string query = "SELECT * FROM Checks " +
            "LEFT JOIN RoomTypes ON RoomTypes.Room_Type_Name = Checks.Room_Type_Name " +
        "WHERE Check_Is_Confirmed = 1 AND Check_Is_Rejected = 0";
        SqlCommand Command = new SqlCommand(query, DBCon);
        SqlDataReader sdr = Command.ExecuteReader();
        if (sdr.HasRows)
        {
            RoomRes.Load(sdr);
            sdr.Close();
        }
        DBCon.Close();

        return RoomRes;
    }
    public static DataTable SelectChecks(IConfiguration config, int _id)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DataTable Rooms = new DataTable();
        DBCon.Open();
        string query = @"
            SELECT DISTINCT Doctor_Username, Check_Has_Medical_Consultation, Check_ID, Check_Cost, 
            Check_Date, Patient_Username, Visit_Start_Date, Type_Name, 
            Check_Is_Confirmed, Checks.Room_Type_Name, Checks.Room_Number
            From Checks
            LEFT JOIN RoomTypes r ON r.Room_Type_Name = Checks.Room_Type_Name
            WHERE Checks.Check_ID = @Check_ID
        ";
        SqlCommand Command = new SqlCommand(query, DBCon);
        Command.Parameters.AddWithValue("@Check_ID", _id);
        SqlDataReader sdr = Command.ExecuteReader();
        if (sdr.HasRows)
        {
            Rooms.Load(sdr);
            sdr.Close();
        }
        DBCon.Close();

        return Rooms;
    }

    public int InsertCheck(IConfiguration config)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DBCon.Open();
        string query = @"INSERT INTO Checks (Room_Type_Name, Type_Name, Room_Number, Check_Date, Check_Is_Confirmed, Check_Cost, Patient_Username, Visit_Start_Date,Doctor_Username)
            values(@Room_Type_Name, @Type_Name, @Room_Number, @Check_Date, @Check_Is_Confirmed, @Check_Cost, @Patient_Username, @Visit_Start_Date,@Doctor_Username)";
        SqlCommand Command = new SqlCommand(query, DBCon);
        Command.Parameters.AddWithValue("@Room_Type_Name", this.Room_Type_Name);
        Command.Parameters.AddWithValue("@Type_Name", this.Type_Name);
        Command.Parameters.AddWithValue("@Room_Number", this.Room_Number);
        Command.Parameters.AddWithValue("@Check_Date", this.Check_Date);
        Command.Parameters.AddWithValue("@Check_Is_Confirmed", this.Check_Is_Confirmed);
        Command.Parameters.AddWithValue("@Check_Cost", this.Check_Cost);
        Command.Parameters.AddWithValue("@Patient_Username", this.Patient_Username);
        Command.Parameters.AddWithValue("@Doctor_Username", this.Doctor_Username);
        Command.Parameters.AddWithValue("@Visit_Start_Date", this.Visit_Start_Date);
        int rowsAff = Command.ExecuteNonQuery();
        DBCon.Close();

        return rowsAff;
    }

    public int UpdateCheck(IConfiguration config)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DBCon.Open();
        string query = "Update Checks SET Doctor_Username = @Doctor_Username, Room_Number = @Room_Number, Check_Date = @Check_Date, Check_Is_Confirmed = @Check_Is_Confirmed " +
            "WHERE Check_ID = @Check_ID";
        SqlCommand Command = new SqlCommand(query, DBCon);
        //Command.Parameters.AddWithValue("@Check_Cost", this.Check_Cost);
        Command.Parameters.AddWithValue("@Check_Date", this.Check_Date);
        Command.Parameters.AddWithValue("@Check_Is_Confirmed", this.Check_Is_Confirmed);
        Command.Parameters.AddWithValue("@Room_Number", this.Room_Number);
        Command.Parameters.AddWithValue("@Doctor_Username", this.Doctor_Username);
        Command.Parameters.AddWithValue("@Check_ID", this.Check_ID);
        int rowsAff = Command.ExecuteNonQuery();
        DBCon.Close();

        return rowsAff;
    }

    public static int UpdateConsultFlag(IConfiguration config, int _id)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DBCon.Open();
        string query = "Update Checks SET Check_Has_Medical_Consultation = 1 " +
            "WHERE Check_ID = @Check_ID";
        SqlCommand Command = new SqlCommand(query, DBCon);
        Command.Parameters.AddWithValue("@Check_ID", _id);
        int rowsAff = Command.ExecuteNonQuery();
        DBCon.Close();

        return rowsAff;
    }


    public Check(DataTable DepartmentsDT){

        List<SelectListItem> DepartmentList = new List<SelectListItem>();
        List<SelectListItem> DoctorList = new List<SelectListItem>();
        foreach(DataRow Department in DepartmentsDT.Rows){
            DepartmentList.Add(new SelectListItem{ Value = Department["Department_Name"].ToString(), Text = Department["Department_Name"].ToString() });
        }
        Departments = DepartmentList;
        Doctors = DoctorList;
    }


    public Check(DataTable Check,DataTable Consultation){
        Checks = new DataTable();
        Consultations = new DataTable();
        Checks = Check;
        Consultations = Consultation;
    }


    //public virtual TypeDetail TypeNameNavigation { get; set; } = null!;
}
