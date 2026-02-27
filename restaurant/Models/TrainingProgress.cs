using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using Microsoft.EntityFrameworkCore;

namespace RestDomain.Models;

[Table("TrainingProgress")]
public partial class TrainingProgress:Entity
{
    //[Key]
    //[Column("ID")]
    //public int Id { get; set; }

    public int? InstuctorD { get; set; }

    [Column("WorkerID")]
    public int? WorkerId { get; set; }

    public int? KnowledgeLevel { get; set; }

    public DateOnly? ReviewDate { get; set; }

    [ForeignKey("InstuctorD")]
    [InverseProperty("TrainingProgresses")]
    public virtual Instructor? InstuctorDNavigation { get; set; }

    [ForeignKey("WorkerId")]
    [InverseProperty("TrainingProgresses")]
    public virtual Worker? Worker { get; set; }
}
