using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HospitalAppl.Models;

public partial class Visit
{
    [Key]
    public string Patient_Username { get; set; } = null!;
    public DateTime Visit_Start_Date { get; set; }

    public double? Visit_Total_Cost { get; set; }

    public DateTime? Visit_End_Date { get; set; }

    //public virtual Patient Patient_Username_Navigation { get; set; } = null!;

    public static DataTable SelectVisits(IConfiguration config)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DataTable Visits = new DataTable();
        DBCon.Open();
        string query = "SELECT v.Patient_Username, Email, Phone, First_Name, Last_Name, Profile_Pic, Visit_Start_Date, Visit_End_Date, Visit_Total_Cost FROM Visits AS v " +
            "INNER JOIN Users AS u ON v.Patient_Username = u.Username";
        SqlCommand Command = new SqlCommand(query, DBCon);
        SqlDataReader sdr = Command.ExecuteReader();
        if (sdr.HasRows)
        {
            Visits.Load(sdr);
            sdr.Close();
        }
        DBCon.Close();

        return Visits;
    }

    public static DataTable SelectActiveVisits(IConfiguration config)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DataTable Visits = new DataTable();
        DBCon.Open();
        string query = "SELECT v.Patient_Username, Email, Phone, First_Name, Last_Name, Profile_Pic, Visit_Start_Date, Visit_End_Date, Visit_Total_Cost " +
            "FROM Visits AS v " +
            "INNER JOIN Users AS u ON v.Patient_Username = u.Username " +
            "WHERE Visit_End_Date IS NULL";
        SqlCommand Command = new SqlCommand(query, DBCon);
        SqlDataReader sdr = Command.ExecuteReader();
        if (sdr.HasRows)
        {
            Visits.Load(sdr);
            sdr.Close();
        }
        DBCon.Close();

        return Visits;
    }

    public static DataTable SelectApprovedVisits(IConfiguration config)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DataTable Visits = new DataTable();
        DBCon.Open();
        string query = "SELECT v.Patient_Username, Email, Phone, First_Name, Last_Name, Profile_Pic, Visit_Start_Date, Visit_End_Date, Visit_Total_Cost " +
            "FROM Visits AS v " +
            "INNER JOIN Users AS u ON v.Patient_Username = u.Username " +
            "WHERE Visit_End_Date IS NOT NULL";
        SqlCommand Command = new SqlCommand(query, DBCon);
        SqlDataReader sdr = Command.ExecuteReader();
        if (sdr.HasRows)
        {
            Visits.Load(sdr);
            sdr.Close();
        }
        DBCon.Close();

        return Visits;
    }

    public static DataTable SelectUserVisits(IConfiguration config, string Username)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DataTable Visits = new DataTable();
        DBCon.Open();
        string query = "SELECT * FROM Visits WHERE Patient_Username = @Username";
        SqlCommand Command = new SqlCommand(query, DBCon);
        Command.Parameters.AddWithValue("@Username", Username);
        SqlDataReader sdr = Command.ExecuteReader();
        if (sdr.HasRows)
        {
            Visits.Load(sdr);
            sdr.Close();
        }
        DBCon.Close();

        return Visits;
    }

    public static DataTable SelectUserActiveVisits(IConfiguration config, string Username)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DataTable Visits = new DataTable();
        DBCon.Open();
        string query = @"SELECT * FROM Visits 
                        WHERE Patient_Username = @Patient_Username AND Visit_End_Date IS NULL";
        SqlCommand Command = new SqlCommand(query, DBCon);
        Command.Parameters.AddWithValue("@Patient_Username", Username);
        SqlDataReader sdr = Command.ExecuteReader();
        if (sdr.HasRows)
        {
            Visits.Load(sdr);
            sdr.Close();
        }
        DBCon.Close();

        return Visits;
    }

    public static DataTable SelectVisits(IConfiguration config, string Username, DateTime Visit_Start_Date)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DataTable Visits = new DataTable();
        DBCon.Open();
        string query = @"SELECT * FROM Visits 
            INNER JOIN Patients AS p ON Visits.Patient_Username = p.Username 
            INNER JOIN Users AS u ON p.Username = u.Username 
            WHERE Patient_Username = @Patient_Username AND Visit_Start_Date = @Visit_Start_Date";
        SqlCommand Command = new SqlCommand(query, DBCon);
        Command.Parameters.AddWithValue("@Patient_Username", Username);
        Command.Parameters.AddWithValue("@Visit_Start_Date", Visit_Start_Date.ToString("yyyy-MM-dd"));
        SqlDataReader sdr = Command.ExecuteReader();
        if (sdr.HasRows)
        {
            Visits.Load(sdr);
            sdr.Close();
        }
        DBCon.Close();

        return Visits;
    }
    public int InsertVisit(IConfiguration config)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DBCon.Open();
        string query = "INSERT INTO Visits (Patient_Username, Visit_Start_Date) " +
            "values(@Patient_Username, @Visit_Start_Date)";
        SqlCommand Command = new SqlCommand(query, DBCon);
        Command.Parameters.AddWithValue("@Patient_Username", this.Patient_Username);
        Command.Parameters.AddWithValue("@Visit_Start_Date", this.Visit_Start_Date);
        int rowsAff = Command.ExecuteNonQuery();
        DBCon.Close();

        return rowsAff;
    }
    public int UpdateVisit(IConfiguration config, string Patient_Username, DateTime Visit_Start_Date)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DBCon.Open();
        string query = "Update Visits SET Visit_Total_Cost = @Visit_Total_Cost" +
            " WHERE Patient_Username = @Patient_Username AND Visit_Start_Date = @Visit_Start_Date";
        SqlCommand Command = new SqlCommand(query, DBCon);
        Command.Parameters.AddWithValue("@Visit_Total_Cost", this.Visit_Total_Cost);
        Command.Parameters.AddWithValue("@Patient_Username", this.Patient_Username);
        Command.Parameters.AddWithValue("@Visit_Start_Date", this.Visit_Start_Date);
        int rowsAff = Command.ExecuteNonQuery();
        DBCon.Close();

        return rowsAff;
    }
    public static int LockVisit(IConfiguration config, string Patient_Username, DateTime Visit_Start_Date)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DBCon.Open();
        string query = "Update Visits SET Visit_End_Date = CAST(CURRENT_TIMESTAMP AS DATE)" +
            " WHERE Patient_Username = @Patient_Username AND Visit_Start_Date = @Visit_Start_Date";
        SqlCommand Command = new SqlCommand(query, DBCon);
        Command.Parameters.AddWithValue("@Patient_Username", Patient_Username);
        Command.Parameters.AddWithValue("@Visit_Start_Date", Visit_Start_Date);
        int rowsAff = Command.ExecuteNonQuery();
        DBCon.Close();

        return rowsAff;
    }
    public static int InsertVisit(IConfiguration config, string Username)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DBCon.Open();
        string query = @"INSERT INTO Visits (Patient_Username, Visit_Start_Date) 
                         values(@Patient_Username, CAST(CURRENT_TIMESTAMP AS DATE)";
        SqlCommand Command = new SqlCommand(query, DBCon);
        Command.Parameters.AddWithValue("@Patient_Username", Username);
        int rowsAff = Command.ExecuteNonQuery();
        DBCon.Close();

        return rowsAff;
    }

    public static DateTime InitVisit(IConfiguration config,string PatientUsername,DateTime RequestDate){
        // Checks If A Visit Already Exists For The Patient
        // If It Exists It Returns The Start Time Of That Visit
        // If Not It Creates One And Return The Sent Time [RequestDate]
        DataTable Visits = new DataTable();
        SqlConnection con = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        string SelectVisitQuery = "SELECT * FROM Visits WHERE Patient_Username = @Username AND Visit_Start_Date < @Date AND Visit_End_Date IS NULL;";
        SqlCommand SelectVisitCmd = new SqlCommand(SelectVisitQuery, con);
        SelectVisitCmd.Parameters.AddWithValue("@Username", PatientUsername);
        SelectVisitCmd.Parameters.AddWithValue("@Date", RequestDate);
        con.Open();
        SqlDataReader sdr = SelectVisitCmd.ExecuteReader();
        if (sdr.HasRows)
        {
            Visits.Load(sdr);
            sdr.Close();
            con.Close();
            return Convert.ToDateTime(Visits.Rows[0]["Visit_Start_Date"]);
        }
        con.Close();

        string InsertVisitQuery = "INSERT INTO Visits(Visit_Start_Date,Patient_Username) VALUES(CAST(CURRENT_TIMESTAMP AS DATE),@Username);";
        SqlCommand InsertVisitCmd = new SqlCommand(InsertVisitQuery, con);
        InsertVisitCmd.Parameters.AddWithValue("@Username", PatientUsername);
        //InsertVisitCmd.Parameters.AddWithValue("@Date", RequestDate);
        con.Open();
        InsertVisitCmd.ExecuteNonQuery();
        return RequestDate;
    }
}
