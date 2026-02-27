using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using Microsoft.EntityFrameworkCore;

namespace RestDomain.Models;

public partial class Employer:Entity
{
    //[Key]
    //[Column("ID")]
    //public int Id { get; set; }

    [Column(TypeName = "character varying")]
    public string? FirstName { get; set; }

    [Column(TypeName = "character varying")]
    public string? LastName { get; set; }

   // [Precision(12, 2)]
    public decimal? SalaryPerHour { get; set; }

    public DateOnly? CreatedAt { get; set; }

    [InverseProperty("Employee")]
    public virtual ICollection<Bonuse> Bonuses { get; set; } = new List<Bonuse>();

    [InverseProperty("Employee")]
    public virtual Instructor? Instructor { get; set; }

    [InverseProperty("Emloyee")]
    public virtual Manager? Manager { get; set; }

    [InverseProperty("Empoyee")]
    public virtual ICollection<Shift> Shifts { get; set; } = new List<Shift>();

    [InverseProperty("Empoyee")]
    public virtual Worker? Worker { get; set; }
}
