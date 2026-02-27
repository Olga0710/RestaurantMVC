using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using Microsoft.EntityFrameworkCore;

namespace RestDomain.Models;

//[Index("Name", Name = "ShiftsTypes_Name_key", IsUnique = true)]
public partial class ShiftsType:Entity
{
    //[Key]
    //[Column("ID")]
    //public int Id { get; set; }

    [Column(TypeName = "character varying")]
    public string? Name { get; set; }

    [InverseProperty("ShiftType")]
    public virtual ICollection<Shift> Shifts { get; set; } = new List<Shift>();
}
