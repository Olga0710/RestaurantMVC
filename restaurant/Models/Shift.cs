using System.ComponentModel.DataAnnotations.Schema;

namespace RestDomain.Models;

public partial class Shift : Entity
{
    // Якщо Entity не має атрибута Identity, додаємо його тут:
    //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    
    //[Column("ID")]
    //public  int Id { get; set; }

    [Column("EmployeeID")]
    public int? EmployeeId { get; set; }

    [Column("ShiftTypeID")]
    public int? ShiftTypeId { get; set; }

    [Column(TypeName = "time")]
    public TimeSpan? StartTime { get; set; }

    [Column(TypeName = "time")]
    public TimeSpan? EndTime { get; set; }

    public DateOnly? ShiftDate { get; set; }

    [ForeignKey("EmployeeId")]
    public virtual Employer? Employee { get; set; }

    [ForeignKey("ShiftTypeId")]
    public virtual ShiftsType? ShiftType { get; set; }
}