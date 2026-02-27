using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using Microsoft.EntityFrameworkCore;

namespace RestDomain.Models;

[Table("Instructor")]
public partial class Instructor
{
    [Key]
    [Column("EmployeeID")]
    public int EmployeeId { get; set; }

    [Column(TypeName = "character varying")]
    public string? Specialization { get; set; }

    public int? ExperienceYears { get; set; }

    [Column("ManagerID")]
    public int? ManagerId { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("Instructor")]
    public virtual Employer Employee { get; set; } = null!;

    [ForeignKey("ManagerId")]
    [InverseProperty("Instructors")]
    public virtual Manager? Manager { get; set; }

    [InverseProperty("InstuctorDNavigation")]
    public virtual ICollection<TrainingProgress> TrainingProgresses { get; set; } = new List<TrainingProgress>();

    [InverseProperty("Instructor")]
    public virtual ICollection<Worker> Workers { get; set; } = new List<Worker>();
}
