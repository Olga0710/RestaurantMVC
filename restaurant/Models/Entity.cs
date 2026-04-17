using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RestDomain.Models
{
    public abstract class Entity
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
    }
}
