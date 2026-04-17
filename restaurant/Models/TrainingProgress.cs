using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using Microsoft.EntityFrameworkCore;

namespace RestDomain.Models;

[Table("TrainingProgress")]
public partial class TrainingProgress : Entity
{
    public int? InstuctorD { get; set; }

    [Column("WorkerID")]
    public int? WorkerId { get; set; }

    [Display(Name = "Оцінка КЛС (%)")]
    [Range(0, 100, ErrorMessage = "Вкажіть відсоток від 0 до 100")]
    public int? KnowledgeLevel { get; set; }

    [Display(Name = "Станція")]
    [Required(ErrorMessage = "Оберіть станцію")]
    public string? Station { get; set; } // Додаємо це поле

    public DateOnly? ReviewDate { get; set; }

    [ForeignKey("InstuctorD")]
    [InverseProperty("TrainingProgresses")]
    public virtual Instructor? InstuctorDNavigation { get; set; }

    [ForeignKey("WorkerId")]
    [InverseProperty("TrainingProgresses")]
    public virtual Worker? Worker { get; set; }
}
