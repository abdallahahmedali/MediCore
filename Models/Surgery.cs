using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HospitalAppl.Models;

public partial class Surgery
{
  
    public int Surgery_ID { get; set; }
    public DateTime Visit_Start_Date { get; set; }
    public string Patient_Username { get; set; } = null!;
    public string Type_Name { get; set; } = null!;
    public string Room_Type_Name { get; set; } = null!;
    public int Room_Number { get; set; }
    public string? Doctor_Username { get; set; } = null!;

    public DateTime Surgery_Date { get; set; }

    public double Surgery_Cost { get; set; }

    public bool Surgery_Is_Confirmed { get; set; }

    //public virtual TypeDetail TypeNameNavigation { get; set; } = null!;

    public static int Approve(IConfiguration config, int _id)
    {
        //DataTable reservations = SelectRoomReservations(config);
        // update code
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DBCon.Open();
        string query = "Update Surgeries SET Surgery_Is_Confirmed = 1 " +
            "WHERE Surgery_ID = @Surgery_ID";
        SqlCommand Command = new SqlCommand(query, DBCon);
        Command.Parameters.AddWithValue("@Surgery_ID", _id);
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
        string query = "Update Surgeries SET Surgery_Is_Rejected = 1 " +
            "WHERE Surgery_ID = @Surgery_ID";
        SqlCommand Command = new SqlCommand(query, DBCon);
        Command.Parameters.AddWithValue("@Surgery_ID", _id);
        int rowsAff = Command.ExecuteNonQuery();
        DBCon.Close();
        return rowsAff;
    }

    public static DataTable SelectSurgeries(IConfiguration config)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DataTable RoomRes = new DataTable();
        DBCon.Open();
        string query = "SELECT * FROM Surgeries " +
            "LEFT JOIN RoomTypes ON RoomTypes.Room_Type_Name = Surgeries.Room_Type_Name";
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
    public static DataTable SelectActiveSurgeries(IConfiguration config)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DataTable RoomRes = new DataTable();
        DBCon.Open();
        string query = "SELECT * FROM Surgeries " +
            "LEFT JOIN RoomTypes ON RoomTypes.Room_Type_Name = Surgeries.Room_Type_Name " +
            "WHERE Surgery_Is_Confirmed = 0 AND Surgery_Is_Rejected = 0";
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
    public static DataTable SelectVisitActiveSurgeries(IConfiguration config, string Username, DateTime Visit_Start_Date)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DataTable RoomRes = new DataTable();
        DBCon.Open();
        string query = "SELECT * FROM Surgeries " +
            "LEFT JOIN RoomTypes ON RoomTypes.Room_Type_Name = Surgeries.Room_Type_Name " +
            "WHERE Surgeries.Patient_Username = @Username " +
            "AND Surgeries.Visit_Start_Date = @Visit_Start_Date";
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

    public static DataTable SelectApprovedSurgeries(IConfiguration config)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DataTable RoomRes = new DataTable();
        DBCon.Open();
        string query = "SELECT * FROM Surgeries " +
            "LEFT JOIN RoomTypes ON RoomTypes.Room_Type_Name = Surgeries.Room_Type_Name " +
        "WHERE Surgery_Is_Confirmed = 1 AND Surgery_Is_Rejected = 0";
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
    public static DataTable SelectSurgeries(IConfiguration config, int _id)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DataTable Rooms = new DataTable();
        DBCon.Open();
        string query = "SELECT DISTINCT Surgeries.Doctor_Username, Surgery_ID, Surgery_Cost, Surgery_Date, Patient_Username, Visit_Start_Date, Type_Name, Surgery_Is_Confirmed, Surgeries.Room_Type_Name, Surgeries.Room_Number " +
            "From Surgeries " +
            "LEFT JOIN Rooms ON Rooms.Room_Type_Name = Surgeries.Room_Type_Name AND Rooms.Room_Number = Surgeries.Room_Number " +
            "WHERE Surgeries.Surgery_ID = @Surgery_ID";
        SqlCommand Command = new SqlCommand(query, DBCon);
        Command.Parameters.AddWithValue("@Surgery_ID", _id);
        SqlDataReader sdr = Command.ExecuteReader();
        if (sdr.HasRows)
        {
            Rooms.Load(sdr);
            sdr.Close();
        }
        DBCon.Close();

        return Rooms;
    }
    public int InsertSurgery(IConfiguration config)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DBCon.Open();
        string query = "INSERT INTO Surgeries (Room_Type_Name, Type_Name, Room_Number, Surgery_Date, Surgery_Is_Confirmed, Surgery_Cost, Patient_Username, Visit_Start_Date,Doctor_Username) " +
            "values(@Room_Type_Name, @Type_Name, @Room_Number, @Surgery_Date, @Surgery_Is_Confirmed, @Surgery_Cost, @Patient_Username, @Visit_Start_Date,@Doctor_Username)";
        SqlCommand Command = new SqlCommand(query, DBCon);
        Command.Parameters.AddWithValue("@Room_Type_Name", this.Room_Type_Name);
        Command.Parameters.AddWithValue("@Type_Name", this.Type_Name);
        Command.Parameters.AddWithValue("@Room_Number", this.Room_Number);
        Command.Parameters.AddWithValue("@Surgery_Date", this.Surgery_Date);
        Command.Parameters.AddWithValue("@Surgery_Is_Confirmed", this.Surgery_Is_Confirmed);
        Command.Parameters.AddWithValue("@Surgery_Cost", this.Surgery_Cost);
        Command.Parameters.AddWithValue("@Patient_Username", this.Patient_Username);
        Command.Parameters.AddWithValue("@Doctor_Username", this.Doctor_Username);
        Command.Parameters.AddWithValue("@Visit_Start_Date", this.Visit_Start_Date);
        int rowsAff = Command.ExecuteNonQuery();
        DBCon.Close();

        return rowsAff;
    }
    public int UpdateSurgery(IConfiguration config)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DBCon.Open();
        string query = "Update Surgeries SET Doctor_Username = @Doctor_Username, Room_Number = @Room_Number, Surgery_Date = @Surgery_Date, Surgery_Is_Confirmed = @Surgery_Is_Confirmed " +
            "WHERE Surgery_ID = @Surgery_ID";
        SqlCommand Command = new SqlCommand(query, DBCon);
        //Command.Parameters.AddWithValue("@Check_Cost", this.Check_Cost);
        Command.Parameters.AddWithValue("@Surgery_Date", this.Surgery_Date);
        Command.Parameters.AddWithValue("@Surgery_Is_Confirmed", this.Surgery_Is_Confirmed);
        Command.Parameters.AddWithValue("@Room_Number", this.Room_Number);
        Command.Parameters.AddWithValue("@Doctor_Username", this.Doctor_Username);
        Command.Parameters.AddWithValue("@Surgery_ID", this.Surgery_ID);
        int rowsAff = Command.ExecuteNonQuery();
        DBCon.Close();

        return rowsAff;
    }


}
