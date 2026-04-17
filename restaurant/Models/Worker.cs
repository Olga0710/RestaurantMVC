using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using Microsoft.EntityFrameworkCore;

namespace RestDomain.Models;

public partial class Worker
{
    [Key]
    [Column("EmployeeID")]
    public int EmployeeId { get; set; }

    [Column("ManagerID")]
    public int? ManagerId { get; set; }

    [Column("InstructorID")]
    public int? InstructorId { get; set; }

    public bool? IsMinor { get; set; }

    public bool? IsCertified { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("Worker")]
    public virtual Employer Employee { get; set; } = null!;

    [ForeignKey("InstructorId")]
    [InverseProperty("Workers")]
    public virtual Instructor? Instructor { get; set; }

    [ForeignKey("ManagerId")]
    [InverseProperty("Workers")]
    public virtual Manager? Manager { get; set; }

    [InverseProperty("Worker")]
    public virtual ICollection<TrainingProgress> TrainingProgresses { get; set; } = new List<TrainingProgress>();
}
