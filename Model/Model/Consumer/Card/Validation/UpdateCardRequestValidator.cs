using FluentValidation;

namespace Domain.Model
{
    public class UpdateCardRequestValidator : AbstractValidator<UpdateCardRequest>
    {
        public UpdateCardRequestValidator()
        {
            RuleFor(s => s.Card_id)
              .NotNull().WithMessage("Id do cartão é obrigatório.")
              .NotEmpty().WithMessage("Id do cartão  é obrigatório.");
            RuleFor(s => s.Consumer_id)
              .NotNull().WithMessage("Id do consumidor é obrigatório.")
              .NotEmpty().WithMessage("Id do consumidor  é obrigatório.");
            RuleFor(s => s.Encrypted)
            .NotNull().WithMessage("Criptografia é obrigatório.")
            .NotEmpty().WithMessage("Criptografia é obrigatório.");

        }
    }
}
