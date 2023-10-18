using System.ComponentModel.DataAnnotations.Schema;
using System.Security;

namespace MakeItSimple.WebApi.Domain.Users
{
    public class UserRole : BaseEntity
    { 
      
        public string UserRoleName { get; set; }
        public ICollection<string>Permissions { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.Now; 

        public DateTime UpdatedAt { get; set; }

        [ForeignKey ("AddedByUser")]
        public int AddedBy { get; set; } 

        public string ModifiedBy { get; set; }
        public bool IsActive { get; set; }

        public virtual User User { get; set; }
        public virtual User AddedByUser { get; set; }

    }
}
