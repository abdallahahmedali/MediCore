using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HospitalAppl.Models;

public partial class ServiceType
{
    [Key]
    public string Service_Type_Name { get; set; } = null!;

    //public virtual ICollection<TypeDetail> TypeDetails { get; } = new List<TypeDetail>();

    public static DataTable SelectServiceTypes(IConfiguration config)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DataTable ServiceTypes = new DataTable();
        DBCon.Open();
        string query = "SELECT * FROM ServiceTypes";
        SqlCommand Command = new SqlCommand(query, DBCon);
        SqlDataReader sdr = Command.ExecuteReader();
        if (sdr.HasRows)
        {
            ServiceTypes.Load(sdr);
            sdr.Close();
        }
        DBCon.Close();

        return ServiceTypes;
    }

    public static DataTable SelectServiceTypes(IConfiguration config, string Service_Type_Name)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DataTable ServiceTypes = new DataTable();
        DBCon.Open();
        string query = "SELECT * FROM ServiceTypes WHERE Service_Type_Name = @Service_Type_Name";
        SqlCommand Command = new SqlCommand(query, DBCon);
        Command.Parameters.AddWithValue("@Service_Type_Name", Service_Type_Name);
        SqlDataReader sdr = Command.ExecuteReader();
        if (sdr.HasRows)
        {
            ServiceTypes.Load(sdr);
            sdr.Close();
        }
        DBCon.Close();

        return ServiceTypes;
    }

    public int InsertServieType(IConfiguration config)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DBCon.Open();
        string query = "INSERT INTO ServiceTypes (Service_Type_Name) values(@Service_Type_Name)";
        SqlCommand Command = new SqlCommand(query, DBCon);
        Command.Parameters.AddWithValue("@Service_Type_Name", this.Service_Type_Name);
        int rowsAff = Command.ExecuteNonQuery();
        DBCon.Close();

        return rowsAff;
    }
    public int UpdateServiceType(IConfiguration config, string Old_Service_Type_Name)
    {
        SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        DBCon.Open();
        string query = "Update ServiceTypes SET Service_Type_Name = @Service_Type_Name WHERE Service_Type_Name = @Old_Service_Type_Name";
        SqlCommand Command = new SqlCommand(query, DBCon);
        Command.Parameters.AddWithValue("@Service_Type_Name", this.Service_Type_Name);
        Command.Parameters.AddWithValue("@Old_Service_Type_Name", Old_Service_Type_Name);
        int rowsAff = Command.ExecuteNonQuery();
        DBCon.Close();

        return rowsAff;
    }

}
