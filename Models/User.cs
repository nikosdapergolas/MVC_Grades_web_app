using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VathmologioMVC.Models;


[Table("users")]
public partial class User
{
    [Key]
    [Column("username")]
    [StringLength(45)]
    public string Username { get; set; } = null!;

    [Column("password")]
    [StringLength(100)]
    public string Password { get; set; } = null!;

    [Column("role")]
    [StringLength(45)]
    public string Role { get; set; } = null!;

    public virtual ICollection<Professor> Professors { get; } = new List<Professor>();

    public virtual ICollection<Secretary> Secretaries { get; } = new List<Secretary>();

    public virtual ICollection<Student> Students { get; } = new List<Student>();
}
