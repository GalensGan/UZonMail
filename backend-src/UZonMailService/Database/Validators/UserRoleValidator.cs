using FluentValidation;
using UZonMailService.UzonMailDB.SQL.Permission;

namespace UZonMailService.Database.Validators
{
    public class UserRoleValidator : AbstractValidator<UserRole>
    {
        public UserRoleValidator()
        {
            RuleFor(x => x.UserId).GreaterThan(0).WithMessage("请指定用户");
            RuleFor(x=>x.Roles).NotEmpty().WithMessage("请至少选择一个角色");
        }
    }
}
