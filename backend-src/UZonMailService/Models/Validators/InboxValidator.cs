using FluentValidation;
using UZonMailService.Models.SQL.Emails;

namespace UZonMailService.Models.Validators
{
    /// <summary>
    /// 收件箱验证器
    /// </summary>
    public class InboxValidator : AbstractValidator<Inbox>
    {
        public InboxValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage(x => $"{x.Email} 不是有效的邮箱格式");
        }
    }
}
