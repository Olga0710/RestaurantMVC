using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestDomain.Models;

public partial class ShiftsType : Entity
{
    [Column(TypeName = "character varying")]
    [Required(ErrorMessage = "Код позиції є обов'язковим")]
    [Display(Name = "Код позиції")]
    public string? Name { get; set; }

    [Display(Name = "Повний опис")]
    public string? Description { get; set; }

    [InverseProperty("ShiftType")]
    public virtual ICollection<Shift> Shifts { get; set; } = new List<Shift>();
}