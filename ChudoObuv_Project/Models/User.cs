using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ChudoObuv_Project.Models;

[Index("Login", Name = "UQ__Users__5E55825B005F3176", IsUnique = true)]
public partial class User
{
    [Key]
    public int Id { get; set; }

    [StringLength(1024)]
    public string LastName { get; set; } = null!;

    [StringLength(1024)]
    public string FirstName { get; set; } = null!;

    [StringLength(1024)]
    public string? MiddleName { get; set; }

    [StringLength(850)]
    public string Login { get; set; } = null!;

    [Column("Id_Role")]
    public int IdRole { get; set; }

    [ForeignKey("IdRole")]
    [InverseProperty("Users")]
    public virtual Role IdRoleNavigation { get; set; } = null!;

    [InverseProperty("IdUserNavigation")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
