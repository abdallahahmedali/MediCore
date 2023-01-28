using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using HospitalAppl.Models;

namespace HospitalAppl.Models;

public partial class RoomType
{
   [Display(Name = "Room_Type_Name")]
    [Required(ErrorMessage = "Please enter the room type name")]
    public string Room_Type_Name { get; set; } = null!;


    [Display(Name = "Room_Prefix")]
    [Required(ErrorMessage = "Please enter the room prefix name")]
    public string Room_Prefix { get; set; } = null!;

    //public virtual ICollection<Room> Rooms { get; } = new List<Room>();
    public static DataTable SelectRoomTypes(IConfiguration config)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DataTable RoomType = new DataTable();
        DBCon.Open();
        string query = "SELECT * FROM RoomTypes";
        SqlCommand Command = new SqlCommand(query, DBCon);
        SqlDataReader sdr = Command.ExecuteReader();
        if (sdr.HasRows)
        {
            RoomType.Load(sdr);
            sdr.Close();
        }
        DBCon.Close();

        return RoomType;
    }
    public static DataTable SelectRoomTypes(IConfiguration config, string Room_Type_Name)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DataTable RoomType = new DataTable();
        DBCon.Open();
        string query = "SELECT * FROM RoomTypes WHERE Room_Type_Name = @Room_Type_Name";
        SqlCommand Command = new SqlCommand(query, DBCon);
        Command.Parameters.AddWithValue("@Room_Type_Name", Room_Type_Name);
        SqlDataReader sdr = Command.ExecuteReader();
        if (sdr.HasRows)
        {
            RoomType.Load(sdr);
            sdr.Close();
        }
        DBCon.Close();

        return RoomType;
    }
    public int InsertRoomType(IConfiguration config)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DBCon.Open();
        string query = "INSERT INTO RoomTypes (Room_Type_Name,Room_Prefix) values(@Room_Type_Name, @Room_Prefix)";
        SqlCommand Command = new SqlCommand(query, DBCon);
        Command.Parameters.AddWithValue("@Room_Type_Name", this.Room_Type_Name);
        Command.Parameters.AddWithValue("@Room_Prefix",this.Room_Prefix);
        int rowsAff = Command.ExecuteNonQuery();
        DBCon.Close();

        return rowsAff;
    }
    public int UpdateRoomType(IConfiguration config, string Old_Room_Type_Name)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DBCon.Open();
        string query = "Update RoomTypes SET Room_Type_Name = @Room_Type_Name, Room_Prefix = @Room_Prefix WHERE Room_Type_Name = @Old_Room_Type_Name";
        SqlCommand Command = new SqlCommand(query, DBCon);
        Command.Parameters.AddWithValue("@Room_Type_Name", this.Room_Type_Name);
        Command.Parameters.AddWithValue("@Room_Prefix", this.Room_Prefix);
        Command.Parameters.AddWithValue("@Old_Room_Type_Name", Old_Room_Type_Name);
        int rowsAff = Command.ExecuteNonQuery();
        DBCon.Close();

        return rowsAff;
    }


}
