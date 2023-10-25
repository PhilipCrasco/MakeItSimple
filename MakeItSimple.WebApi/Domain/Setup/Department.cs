using MakeItSimple.WebApi.Domain.Users;
using System.ComponentModel.DataAnnotations.Schema;

namespace MakeItSimple.WebApi.Domain.Setup
{
    public class Department : BaseEntity
    {
        public string DepartmentName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; }

        [ForeignKey("AddedByUser")]
        public int ? AddedBy { get; set; }

        public string ModifiedBy { get; set; }
        public bool IsActive { get; set; }

        public virtual IEnumerable<User> Users { get; set; } = new List<User>();
        public virtual User AddedByUser { get; set; }
    }
}
