using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using Microsoft.Data.SqlClient;

namespace HospitalAppl.Models{
    public class User{
        [Key]
        public String Username{ get; set;}

        public String Password{ get; set; }

        public String First_Name{ get; set; }

        public String Middle_Name{ get; set; }
        
        public String Last_Name{ get; set; }
        
        public String Phone{ get; set; }
        
        public bool Sex{ get; set; }

        public DateTime Start_Date{ get; set; }
         
        public String Street{ get; set; }
         
        public String District{ get; set; }
        
        public String City{ get; set; }
        
        public String Nationality{ get; set; }
          
        public String National_ID{ get; set; }
         
        public String Email{ get; set; }

        public String Profile_Pic{ get; set; }
        public bool Is_Admin{ get; set; }

        [NotMapped]
        public IFormFile ProfilePictureFile{ get; set; }
        

        public static DataTable SelectUsers(IConfiguration config, string Username)
        {
            SqlConnection DBCon = new SqlConnection(config.GetConnectionString("DefaultConnection"));
            DataTable Rooms = new DataTable();
            DBCon.Open();
            string query = "SELECT * FROM Users WHERE Username = @Username";
            SqlCommand Command = new SqlCommand(query, DBCon);
            Command.Parameters.AddWithValue("@Username", Username);
            SqlDataReader sdr = Command.ExecuteReader();
            if (sdr.HasRows)
            {
                Rooms.Load(sdr);
                sdr.Close();
            }
            DBCon.Close();

            return Rooms;
        }
    }
}