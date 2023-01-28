using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace HospitalAppl.Models;

public partial class RoomReservation
{
    public int Reservation_ID { get; set; }
    [Required]
    public DateTime Visit_Start_Date { get; set; }
    [Required]
    public string Patient_Username { get; set; } = null!;
    [Required]
    public string Room_Type_Name { get; set; } = null!;
    [Required]
    public int Room_Number { get; set; }

    public DateTime Reservation_Start_Date { get; set; }

    public double Reservation_Cost { get; set; }

    public bool Reservation_Is_Confirmed { get; set; }
    public bool Reservation_Is_Rejected { get; set; }

    public DateTime Reservation_End_Date { get; set; }

    //public virtual TypeDetail Type_Name_Navigation { get; set; } = null!;
    public static int Approve(IConfiguration config, int _id)
    {
        DataTable reservations = SelectRoomReservations(config);
        // update code
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DBCon.Open();
        string query = "Update RoomReservations SET Reservation_Is_Confirmed = 1 " +
            "WHERE Reservation_ID = @Reservation_ID";
        SqlCommand Command = new SqlCommand(query, DBCon);
        Command.Parameters.AddWithValue("@Reservation_ID", _id);
        int rowsAff = Command.ExecuteNonQuery();
        DBCon.Close();
        return rowsAff;
    }

    public static int Delete(IConfiguration config, int _id)
    {
        DataTable reservations = SelectRoomReservations(config);
        // update code
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DBCon.Open();
        string query = "Update RoomReservations SET Reservation_Is_Rejected = 1 " +
        "WHERE Reservation_ID = @Reservation_ID";
        SqlCommand Command = new SqlCommand(query, DBCon);
        Command.Parameters.AddWithValue("@Reservation_ID", _id);
        int rowsAff = Command.ExecuteNonQuery();
        DBCon.Close();
        return rowsAff;
    }

    public static DataTable SelectRoomReservations(IConfiguration config)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DataTable RoomRes = new DataTable();
        DBCon.Open();
        string query = "SELECT * FROM RoomReservations " +
            "INNER JOIN RoomTypes ON RoomTypes.Room_Type_Name = RoomReservations.Room_Type_Name";
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
    public static DataTable SelectActiveRoomReservations(IConfiguration config)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DataTable RoomRes = new DataTable();
        DBCon.Open();
        string query = "SELECT * FROM RoomReservations " +
            "INNER JOIN RoomTypes ON RoomTypes.Room_Type_Name = RoomReservations.Room_Type_Name " +
            "WHERE Reservation_Is_Confirmed = 0 AND Reservation_Is_Rejected = 0";
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
    public static DataTable SelectVisitRoomReservations(IConfiguration config, string Username, DateTime Visit_Start_Date)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DataTable RoomRes = new DataTable();
        DBCon.Open();
        string query = "SELECT * FROM RoomReservations " +
            "INNER JOIN RoomTypes ON RoomTypes.Room_Type_Name = RoomReservations.Room_Type_Name " +
            "WHERE Reservation_Is_Confirmed = 0 AND Reservation_Is_Rejected = 0 " +
            "AND RoomReservations.Patient_Username = @Username AND RoomReservations.Visit_Start_Date = @Visit_Start_Date";
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
    public static DataTable SelectApprovedRoomReservations(IConfiguration config)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DataTable RoomRes = new DataTable();
        DBCon.Open();
        string query = "SELECT * FROM RoomReservations " +
            "INNER JOIN RoomTypes ON RoomTypes.Room_Type_Name = RoomReservations.Room_Type_Name " +
            "WHERE Reservation_Is_Confirmed = 1";
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
    public static DataTable SelectRoomReservations(IConfiguration config, int _id)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DataTable Rooms = new DataTable();
        DBCon.Open();
        string query = "SELECT DISTINCT Reservation_ID, RoomReservations.Room_Type_Name, RoomReservations.Room_Number, Reservation_Cost , Room_Cost, Reservation_Start_Date, Reservation_End_Date, Patient_Username, Visit_Start_Date, Reservation_Is_Confirmed " +
            "FROM RoomReservations, Rooms AS R " +
            "WHERE RoomReservations.Reservation_ID = @Reservation_ID " +
            "AND RoomReservations.Room_Type_Name = R.Room_Type_Name AND RoomReservations.Room_Number = R.Room_Number";
        SqlCommand Command = new SqlCommand(query, DBCon);
        Command.Parameters.AddWithValue("@Reservation_ID", _id);
        SqlDataReader sdr = Command.ExecuteReader();
        if (sdr.HasRows)
        {
            Rooms.Load(sdr);
            sdr.Close();
        }
        DBCon.Close();

        return Rooms;
    }
    public int InsertReservation(IConfiguration config)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DBCon.Open();
        string query = "INSERT INTO RoomReservations (Room_Type_Name, Room_Number, Reservation_Start_Date, Reservation_End_Date, Reservation_Is_Confirmed, Reservation_Cost, Patient_Username, Visit_Start_Date) " +
            "values(@Room_Type_Name, @Room_Number, @Reservation_Start_Date, @Reservation_End_Date, @Reservation_Is_Confirmed, @Reservation_Cost, @Patient_Username, @Visit_Start_Date)";
        SqlCommand Command = new SqlCommand(query, DBCon);
        Command.Parameters.AddWithValue("@Room_Type_Name", this.Room_Type_Name);
        Command.Parameters.AddWithValue("@Room_Number", this.Room_Number);
        Command.Parameters.AddWithValue("@Reservation_Start_Date", this.Reservation_Start_Date);
        Command.Parameters.AddWithValue("@Reservation_End_Date", this.Reservation_End_Date);
        Command.Parameters.AddWithValue("@Reservation_Is_Confirmed", this.Reservation_Is_Confirmed);
        Command.Parameters.AddWithValue("@Reservation_Cost", this.Reservation_Cost);
        Command.Parameters.AddWithValue("@Patient_Username", this.Patient_Username);
        Command.Parameters.AddWithValue("@Visit_Start_Date", this.Visit_Start_Date);
        int rowsAff = Command.ExecuteNonQuery();
        DBCon.Close();

        return rowsAff;
    }
    public int UpdateRoomReservation(IConfiguration config)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DBCon.Open();
        string query = "Update RoomReservations SET Reservation_Cost = @Reservation_Cost, Reservation_Start_Date = @Reservation_Start_Date, Reservation_End_Date = @Reservation_End_Date, Reservation_Is_Confirmed = @Reservation_Is_Confirmed " +
            "WHERE Room_Type_Name = @Room_Type_Name AND Room_Number = @Room_Number " +
            "AND Patient_Username = @Patient_Username AND Visit_Start_Date = @Visit_Start_Date";
        SqlCommand Command = new SqlCommand(query, DBCon);
        Command.Parameters.AddWithValue("@Room_Type_Name", this.Room_Type_Name);
        Command.Parameters.AddWithValue("@Room_Number", this.Room_Number);
        Command.Parameters.AddWithValue("@Patient_Username", this.Patient_Username);
        Command.Parameters.AddWithValue("@Visit_Start_Date", this.Visit_Start_Date);
        Command.Parameters.AddWithValue("@Reservation_Cost", this.Reservation_Cost);
        Command.Parameters.AddWithValue("@Reservation_Start_Date", this.Reservation_Start_Date);
        Command.Parameters.AddWithValue("@Reservation_End_Date", this.Reservation_End_Date);
        Command.Parameters.AddWithValue("@Reservation_Is_Confirmed", this.Reservation_Is_Confirmed);
        int rowsAff = Command.ExecuteNonQuery();
        DBCon.Close();

        return rowsAff;
    }

}
