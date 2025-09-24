using FluentValidation;

namespace Domain.Model
{
    public class CreatePaymentRequestValidator : AbstractValidator<CreatePaymentRequest>
    {
        public CreatePaymentRequestValidator()
        {
            RuleFor(s => s.Description)
              .NotEmpty().WithMessage("Descrição é obrigatório.")
              .NotNull().WithMessage("Descrição é obrigatório.");

        }
    }
}
