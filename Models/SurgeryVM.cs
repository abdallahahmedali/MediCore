using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HospitalAppl.Models;

public partial class SurgeryVM
{
    public SurgeryVM() {}
    public Surgery Surgery { get; set;}

    public DataTable TypeNames { get; set;}

    public SurgeryVM(DataTable Types){
        this.TypeNames = new DataTable();
        this.TypeNames = Types;
    }
    
    //public virtual TypeDetail TypeNameNavigation { get; set; } = null!;
}
