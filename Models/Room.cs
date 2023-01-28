using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace HospitalAppl.Models;

public partial class Room
{
     [Required]
    public string Room_Type_Name { get; set; } = null!;
    [Required]
    public int Room_Number { get; set; }
    [Required]
    public int Room_Capacity { get; set; }

    public int Room_Excess_Capacity { get; set; }

    public bool Room_Companion { get; set; }
    [Required]
    public double Room_Cost { get; set; }

    //public virtual RoomType Room_Type_Name_Navigation { get; set; } = null!;


    public static DataTable SelectRooms(IConfiguration config)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DataTable Rooms = new DataTable();
        DBCon.Open();
        string query = "SELECT * FROM Rooms";
        SqlCommand Command = new SqlCommand(query, DBCon);
        SqlDataReader sdr = Command.ExecuteReader();
        if (sdr.HasRows)
        {
            Rooms.Load(sdr);
            sdr.Close();
        }
        DBCon.Close();

        return Rooms;
    }
    public static DataTable SelectRooms(IConfiguration config, string Room_Type_Name, int Room_Number)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DataTable Rooms = new DataTable();
        DBCon.Open();
        string query = "SELECT * FROM Rooms WHERE Room_Type_Name = @Room_Type_Name AND Room_Number = @Room_Number";
        SqlCommand Command = new SqlCommand(query, DBCon);
        Command.Parameters.AddWithValue("@Room_Type_Name", Room_Type_Name);
        Command.Parameters.AddWithValue("@Room_Number", Room_Number);
        SqlDataReader sdr = Command.ExecuteReader();
        if (sdr.HasRows)
        {
            Rooms.Load(sdr);
            sdr.Close();
        }
        DBCon.Close();

        return Rooms;
    }

    public static DataTable SelectRooms(IConfiguration config, string Room_Type_Name)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DataTable Rooms = new DataTable();
        DBCon.Open();
        string query = "SELECT * FROM Rooms WHERE Room_Type_Name = @Room_Type_Name";
        SqlCommand Command = new SqlCommand(query, DBCon);
        Command.Parameters.AddWithValue("@Room_Type_Name", Room_Type_Name);
        SqlDataReader sdr = Command.ExecuteReader();
        if (sdr.HasRows)
        {
            Rooms.Load(sdr);
            sdr.Close();
        }
        DBCon.Close();

        return Rooms;
    }
    public int InsertRoom(IConfiguration config)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DBCon.Open();
        string query = "INSERT INTO Rooms (Room_Type_Name, Room_Number, Room_Capacity, Room_Excess_Capacity, Room_Companion, Room_Cost) " +
            "values(@Room_Type_Name, @Room_Number, @Room_Capacity, @Room_Excess_Capacity, @Room_Companion, @Room_Cost)";
        SqlCommand Command = new SqlCommand(query, DBCon);
        Command.Parameters.AddWithValue("@Room_Type_Name", this.Room_Type_Name);
        Command.Parameters.AddWithValue("@Room_Number", this.Room_Number);
        Command.Parameters.AddWithValue("@Room_Capacity", this.Room_Capacity);
        Command.Parameters.AddWithValue("@Room_Excess_Capacity", this.Room_Excess_Capacity);
        Command.Parameters.AddWithValue("@Room_Companion", this.Room_Companion);
        Command.Parameters.AddWithValue("@Room_Cost", this.Room_Cost);
        int rowsAff = Command.ExecuteNonQuery();
        DBCon.Close();

        return rowsAff;
    }
    public int UpdateRoom(IConfiguration config, string Old_Room_Type_Name, int Old_Room_Number)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DBCon.Open();
        string query = "Update Rooms SET Room_Type_Name = @Room_Type_Name, Room_Number = @Room_Number, Room_Capacity = @Room_Capacity, Room_Excess_Capacity = @Room_Excess_Capacity, Room_Companion = @Room_Companion, Room_Cost = @Room_Cost" +
            " WHERE Room_Type_Name = @Old_Room_Type_Name AND Room_Number = @Old_Room_Number";
        SqlCommand Command = new SqlCommand(query, DBCon);
        Command.Parameters.AddWithValue("@Room_Type_Name", this.Room_Type_Name);
        Command.Parameters.AddWithValue("@Room_Number", this.Room_Number);
        Command.Parameters.AddWithValue("@Room_Capacity", this.Room_Capacity);
        Command.Parameters.AddWithValue("@Room_Excess_Capacity", this.Room_Excess_Capacity);
        Command.Parameters.AddWithValue("@Room_Companion", this.Room_Companion);
        Command.Parameters.AddWithValue("@Room_Cost", this.Room_Cost);
        Command.Parameters.AddWithValue("@Old_Room_Type_Name", Old_Room_Type_Name);
        Command.Parameters.AddWithValue("@Old_Room_Number", Old_Room_Number);
        int rowsAff = Command.ExecuteNonQuery();
        DBCon.Close();

        return rowsAff;
    }



}
