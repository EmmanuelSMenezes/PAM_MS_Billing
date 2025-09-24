using FluentValidation;

namespace Domain.Model
{
    public class UpdatePaymentRequestValidator : AbstractValidator<UpdatePaymentRequest>
    {
        public UpdatePaymentRequestValidator()
        {

            RuleFor(s => s.Payment_options_id)
             .NotEmpty().WithMessage("Id do meio de pagamento é obrigatório.")
             .NotNull().WithMessage("Id do meio de pagamento é obrigatório.");
            RuleFor(s => s.Active)
              .NotEmpty().WithMessage("Ativo é obrigatório.")
              .NotNull().WithMessage("Ativo é obrigatório.");

        }
    }
}
