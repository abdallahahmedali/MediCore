using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace HospitalAppl.Models{
    public class PatientVM{
        public PatientVM(){}

        public User User { get; set;}
        public Patient Patient { get; set;}


        public PatientVM(DataRow UserRow){
            User = new User();
            Patient = new Patient();
            User.Username = UserRow["Username"].ToString();
            User.Password = UserRow["Password"].ToString();
            User.First_Name = UserRow["First_Name"].ToString();
            User.Middle_Name = UserRow["Middle_Name"].ToString();
            User.Last_Name = UserRow["Last_Name"].ToString();
            User.Start_Date = Convert.ToDateTime(UserRow["Start_Date"]);
            User.Street = UserRow["Street"].ToString();
            User.District = UserRow["District"].ToString();
            User.City = UserRow["City"].ToString();
            User.Sex = Convert.ToBoolean(UserRow["Sex"]);
            User.Nationality = UserRow["Nationality"].ToString();
            User.National_ID = UserRow["National_ID"].ToString();
            User.Phone = UserRow["Phone"].ToString();
            User.Email = UserRow["Email"].ToString();
            User.Profile_Pic = UserRow["Profile_Pic"].ToString();
            Patient.Username = UserRow["Username"].ToString();
            if(UserRow["Patient_Birthdate"] != "") Patient.Patient_Birthdate = Convert.ToDateTime(UserRow["Patient_Birthdate"]); else Patient.Patient_Birthdate = DateTime.Now;
            Patient.Patient_History_Details = UserRow["Patient_History_Details"].ToString();
        }


    }
}