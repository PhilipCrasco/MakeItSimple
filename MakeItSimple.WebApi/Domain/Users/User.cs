using FluentValidation;
using MakeItSimple.WebApi.Domain.Setup;

namespace MakeItSimple.WebApi.Domain.Users
{
    public class User : BaseEntity
    {

        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;
        public int ? AddedBy { get; set; }
        public int ? UserRoleId { get; set; }
        public int ? DepartmentId { get; set; }

        public virtual User AddedByUser { get; set; }

        public virtual Department Department { get; set; }
        public virtual UserRole UserRole { get; set; }


    }

    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(x => x.Id).NotNull();
            RuleFor(x => x.Username).NotEmpty().WithMessage("Username is required!")
                .MinimumLength(3).WithMessage("Username must be at least 3 character long!");
        }
    }


}
