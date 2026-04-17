using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using Microsoft.EntityFrameworkCore;

namespace RestDomain.Models;

public partial class Manager
{
    [Key]
    [Column("EmployeeID")]
    public int EmployeeId { get; set; }

    [Column(TypeName = "character varying")]
    public string? Departmet { get; set; }

    public int? OfficePhone { get; set; }

    [InverseProperty("Manager")]
    public virtual ICollection<Bonuse> Bonuses { get; set; } = new List<Bonuse>();

    [ForeignKey("EmployeeId")]
    [InverseProperty("Manager")]
    public virtual Employer Employee { get; set; } = null!;

    [InverseProperty("Manager")]
    public virtual ICollection<Instructor> Instructors { get; set; } = new List<Instructor>();

    [InverseProperty("Manager")]
    public virtual ICollection<Worker> Workers { get; set; } = new List<Worker>();
}
