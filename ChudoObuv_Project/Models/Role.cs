using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChudoObuv_Project.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }

        [StringLength(1024)]
        public string RoleName { get; set; } = null!;

        [InverseProperty("IdRoleNavigation")]
        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}
