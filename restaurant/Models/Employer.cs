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

    public string Role { get; set; } = "Worker";
    public DateOnly? CreatedAt { get; set; } = DateOnly.FromDateTime(DateTime.Now);

    [InverseProperty("Employee")]
    public virtual ICollection<Bonuse> Bonuses { get; set; } = new List<Bonuse>();

    [Required(ErrorMessage = "Номер телефону обов'язковий")]
    [Phone(ErrorMessage = "Невірний формат номера")]
    public string? PhoneNumber { get; set; }

    [InverseProperty("Employee")]
    public virtual Instructor? Instructor { get; set; }

    [InverseProperty("Employee")]
    public virtual Manager? Manager { get; set; }

    [InverseProperty("Employee")]
    public virtual ICollection<Shift> Shifts { get; set; } = new List<Shift>();

    [InverseProperty("Employee")]
    public virtual Worker? Worker { get; set; }

    [Column(TypeName = "character varying")]
    public string? Email { get; set; } // Додай цей рядок
}
