using FluentValidation;

namespace Domain.Model
{
    public class CreateCardRequestValidator : AbstractValidator<CreateCardRequest>
    {
        public CreateCardRequestValidator()
        {
            RuleFor(s => s.Consumer_id)
              .NotNull().WithMessage("Id do consumidor é obrigatório.")
              .NotEmpty().WithMessage("Id do consumidor  é obrigatório.");

            RuleFor(s => s.Encrypted)
             .NotNull().WithMessage("Criptografia é obrigatório.")
             .NotEmpty().WithMessage("Criptografia é obrigatório.");

        }
    }
}
