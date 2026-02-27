using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using Microsoft.EntityFrameworkCore;

namespace RestDomain.Models;

public partial class Shift:Entity
{
    //[Key]
    //[Column("ID")]
    //public int Id { get; set; }

    [Column("EmpoyeeID")]
    public int? EmpoyeeId { get; set; }

    [Column("ShiftTypeID")]
    public int? ShiftTypeId { get; set; }

    [Column(TypeName = "time with time zone")]
    public DateTimeOffset? StartTime { get; set; }

    [Column(TypeName = "time with time zone")]
    public DateTimeOffset? EndTime { get; set; }

    public DateOnly? ShiftDate { get; set; }

    [ForeignKey("EmpoyeeId")]
    [InverseProperty("Shifts")]
    public virtual Employer? Empoyee { get; set; }

    [ForeignKey("ShiftTypeId")]
    [InverseProperty("Shifts")]
    public virtual ShiftsType? ShiftType { get; set; }
}
