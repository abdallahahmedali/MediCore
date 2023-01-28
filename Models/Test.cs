using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Microsoft.Data.SqlClient;

namespace HospitalAppl.Models;

public partial class Test
{
     public int Test_ID { get; set; }
    public DateTime Visit_Start_Date { get; set; }
    public string Patient_Username { get; set; } = null!;
    public string Type_Name { get; set; } = null!;
    public string Room_Type_Name { get; set; } = null!;
    public int Room_Number { get; set; }
    public string? Doctor_Username { get; set; } = null!;

    public DateTime Test_Date { get; set; }

    public double Test_Cost { get; set; }

    public bool Test_Is_Confirmed { get; set; }
    public bool? Test_Is_Rejected { get; set; }

    public DateTime? Test_Delivery_Date { get; set; }

    public string? Test_Report_File { get; set; }

    //public virtual TypeDetail Type_Name { get; set; } = null!;

    public static int Approve(IConfiguration config, int _id)
    {
        //DataTable Tests = SelectTests(config);
        // update code
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DBCon.Open();
        string query = "Update Tests SET Test_Is_Confirmed = 1 " +
            "WHERE Test_ID = @Test_ID";
        SqlCommand Command = new SqlCommand(query, DBCon);
        Command.Parameters.AddWithValue("@Test_ID", _id);
        int rowsAff = Command.ExecuteNonQuery();
        DBCon.Close();
        return rowsAff;
    }

    public static int Delete(IConfiguration config, int _id)
    {
        //DataTable Tests = SelectTests(config);
        // update code
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DBCon.Open();
        string query = "Update Tests SET Test_Is_Rejected = 1 " +
            "WHERE Test_ID = @Test_ID";
        SqlCommand Command = new SqlCommand(query, DBCon);
        Command.Parameters.AddWithValue("@Test_ID", _id);
        int rowsAff = Command.ExecuteNonQuery();
        DBCon.Close();
        return rowsAff;
    }

    public static DataTable SelectTests(IConfiguration config)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DataTable RoomRes = new DataTable();
        DBCon.Open();
        string query = "SELECT * FROM Tests " +
            "LEFT JOIN RoomTypes ON RoomTypes.Room_Type_Name = Tests.Room_Type_Name";
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
    public static DataTable SelectActiveTests(IConfiguration config)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DataTable RoomRes = new DataTable();
        DBCon.Open();
        string query = "SELECT * FROM Tests " +
            "LEFT JOIN RoomTypes ON RoomTypes.Room_Type_Name = Tests.Room_Type_Name " +
            "WHERE Test_Is_Confirmed = 0 AND Test_Is_Rejected = 0";
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


    public static DataTable SelectVisitActiveTests(IConfiguration config, string Username, DateTime Visit_Start_Date)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DataTable RoomRes = new DataTable();
        DBCon.Open();
        string query = "SELECT * FROM Tests " +
            "LEFT JOIN RoomTypes ON RoomTypes.Room_Type_Name = Tests.Room_Type_Name " +
            "WHERE Tests.Patient_Username = @Username " +
            "AND Tests.Visit_Start_Date = @Visit_Start_Date";
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


    public static DataTable SelectApprovedTests(IConfiguration config)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DataTable RoomRes = new DataTable();
        DBCon.Open();
        string query = "SELECT * FROM Tests " +
            "LEFT JOIN RoomTypes ON RoomTypes.Room_Type_Name = Tests.Room_Type_Name " +
        "WHERE Test_Is_Confirmed = 1 AND Test_Is_Rejected = 0";
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
    public static DataTable SelectTests(IConfiguration config, int _id)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DataTable Rooms = new DataTable();
        DBCon.Open();
        string query = "SELECT DISTINCT Doctor_Username, Test_ID, Test_Cost, Test_Date, Patient_Username, Visit_Start_Date, Type_Name, Test_Is_Confirmed, Tests.Room_Type_Name, Tests.Room_Number " +
            "From Tests " +
            "LEFT JOIN Rooms ON Rooms.Room_Type_Name = Tests.Room_Type_Name AND Rooms.Room_Number = Tests.Room_Number " +
            "WHERE Tests.Test_ID = @Test_ID";
        SqlCommand Command = new SqlCommand(query, DBCon);
        Command.Parameters.AddWithValue("@Test_ID", _id);
        SqlDataReader sdr = Command.ExecuteReader();
        if (sdr.HasRows)
        {
            Rooms.Load(sdr);
            sdr.Close();
        }
        DBCon.Close();

        return Rooms;
    }
    public int InsertTest(IConfiguration config)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DBCon.Open();
        string query = "INSERT INTO Tests (Test_Report_File, Room_Type_Name, Type_Name, Room_Number, Test_Date, Test_Is_Confirmed, Test_Cost, Patient_Username, Visit_Start_Date,Doctor_Username) " +
            "values(@Test_Report_File, @Room_Type_Name, @Type_Name, @Room_Number, @Test_Date, @Test_Is_Confirmed, @Test_Cost, @Patient_Username, @Visit_Start_Date,@Doctor_Username)";
        SqlCommand Command = new SqlCommand(query, DBCon);
        Command.Parameters.AddWithValue("@Room_Type_Name", this.Room_Type_Name);
        Command.Parameters.AddWithValue("@Test_Report_File", this.Test_Report_File);
        Command.Parameters.AddWithValue("@Type_Name", this.Type_Name);
        Command.Parameters.AddWithValue("@Room_Number", this.Room_Number);
        Command.Parameters.AddWithValue("@Test_Date", this.Test_Date);
        Command.Parameters.AddWithValue("@Test_Is_Confirmed", this.Test_Is_Confirmed);
        Command.Parameters.AddWithValue("@Test_Cost", this.Test_Cost);
        Command.Parameters.AddWithValue("@Patient_Username", this.Patient_Username);
        Command.Parameters.AddWithValue("@Doctor_Username", this.Doctor_Username);
        Command.Parameters.AddWithValue("@Visit_Start_Date", this.Visit_Start_Date);
        int rowsAff = Command.ExecuteNonQuery();
        DBCon.Close();

        return rowsAff;
    }

    public int UpdateTest(IConfiguration config)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DBCon.Open();
        string query = "Update Tests SET Test_Report_File = @Test_Report_File, Room_Number = @Room_Number, Test_Date = @Test_Date, Test_Is_Confirmed = @Test_Is_Confirmed " +
            "WHERE Test_ID = @Test_ID";
        SqlCommand Command = new SqlCommand(query, DBCon);
        //Command.Parameters.AddWithValue("@Check_Cost", this.Check_Cost);
        Command.Parameters.AddWithValue("@Test_Report_File", this.Test_Report_File);
        Command.Parameters.AddWithValue("@Test_Date", this.Test_Date);
        Command.Parameters.AddWithValue("@Test_Is_Confirmed", this.Test_Is_Confirmed);
        Command.Parameters.AddWithValue("@Room_Number", this.Room_Number);
        Command.Parameters.AddWithValue("@Test_ID", this.Test_ID);
        int rowsAff = Command.ExecuteNonQuery();
        DBCon.Close();

        return rowsAff;
    }

    public Test(IConfiguration config, String username)
    {
        Patient_Username = username;
    }
    public Test()
    {
   
    }
    public static DataTable SelectTypeNamesAndCost(IConfiguration config)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DataTable Typenameandcost = new DataTable();
        DBCon.Open();
        string query = "SELECT * FROM TypeDetails where Service_Type_Name='Test';";
        SqlCommand Command = new SqlCommand(query, DBCon);
        SqlDataReader sdr = Command.ExecuteReader();
        if (sdr.HasRows)
        {
            Typenameandcost.Load(sdr);
            sdr.Close();
        }
        DBCon.Close();

        return Typenameandcost;
    }
    public static DataTable SelectDoctors(IConfiguration config)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DataTable DoctorUsername = new DataTable();
        DBCon.Open();
        string query = "SELECT Username FROM Employees where Job_Name='Doctor';";
        SqlCommand Command = new SqlCommand(query, DBCon);
        SqlDataReader sdr = Command.ExecuteReader();
        if (sdr.HasRows)
        {
            DoctorUsername.Load(sdr);
            sdr.Close();
        }
        DBCon.Close();

        return DoctorUsername;
    }
    public static DataTable SelectRoomNumber(IConfiguration config)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DataTable RoomNumber = new DataTable();
        DBCon.Open();
        string query = "SELECT Room_Number FROM Rooms where Room_Type_Name='Test';";
        SqlCommand Command = new SqlCommand(query, DBCon);
        SqlDataReader sdr = Command.ExecuteReader();
        if (sdr.HasRows)
        {
            RoomNumber.Load(sdr);
            sdr.Close();
        }
        DBCon.Close();

        return RoomNumber;
    }
    public static bool ChechValidHour(IConfiguration config,int roomNumber,DateTime date)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DataTable Hours = new DataTable();
        DBCon.Open();
        string query = "SELECT * FROM Tests where Test_Date=@Test_Date and Room_Number=@roomNumber;";
        SqlCommand Command = new SqlCommand(query, DBCon);
        Command.Parameters.AddWithValue("@Test_Date", date);
        Command.Parameters.AddWithValue("@roomNumber", roomNumber);
        SqlDataReader sdr = Command.ExecuteReader();
        if (sdr.HasRows)
        {
            Hours.Load(sdr);
            sdr.Close();
            return false;
        }
        DBCon.Close();

        return true;
    }
    //public int InsertTest(IConfiguration config)
    //{
    //    SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
    //    DBCon.Open();
    //    string query = "INSERT INTO Test values(@)";
    //    SqlCommand Command = new SqlCommand(query, DBCon);
    //    Command.Parameters.AddWithValue("@Department_Name", this.Department_Name);
    //    int rowsAff = Command.ExecuteNonQuery();
    //    DBCon.Close();

    //    return rowsAff;
    //}
    //public int UpdateDepartment(IConfiguration config, string Old_Department_Name)
    //{
    //    SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
    //    DBCon.Open();
    //    string query = "Update Departments SET Department_Name = @Department_Name WHERE Department_Name = @Old_Department_Name";
    //    SqlCommand Command = new SqlCommand(query, DBCon);
    //    Command.Parameters.AddWithValue("@Department_Name", this.Department_Name);
    //    Command.Parameters.AddWithValue("@Old_Department_Name", Old_Department_Name);
    //    int rowsAff = Command.ExecuteNonQuery();
    //    DBCon.Close();

    //    return rowsAff;
    //}
    public static DataTable SelectTests(IConfiguration config, string? Patient_Username)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DataTable Tests = new DataTable();
        DBCon.Open();
        string query = "SELECT * FROM Tests WHERE Patient_Username = @Patient_Username;";
        SqlCommand Command = new SqlCommand(query, DBCon);
        Command.Parameters.AddWithValue("@Patient_Username", Patient_Username);
        SqlDataReader sdr = Command.ExecuteReader();
        if (sdr.HasRows)
        {
            Tests.Load(sdr);
            sdr.Close();
        }
        DBCon.Close();

        return Tests;
    }

}
