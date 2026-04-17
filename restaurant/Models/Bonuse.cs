using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using Microsoft.EntityFrameworkCore;

namespace RestDomain.Models;

public partial class Bonuse:Entity
{
   // [Key]
    //[Column("ID")]
    //public int Id { get; set; }

    [Column("ManagerID")]
    public int? ManagerId { get; set; }

    [Column("EmployeeID")]
    public int? EmployeeId { get; set; }

    //[Precision(12, 2)]
    public decimal? Amount { get; set; }

    [Column(TypeName = "character varying")]
    public string? Reason { get; set; }

    [Column(TypeName = "timestamp with time zone")]
    public DateTimeOffset? DataGranted { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("Bonuses")]
    public virtual Employer? Employee { get; set; }

    [ForeignKey("ManagerId")]
    [InverseProperty("Bonuses")]
    public virtual Manager? Manager { get; set; }
}
