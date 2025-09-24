using FluentValidation;

namespace Domain.Model
{
  public class CreateConsumerRequestValidator : AbstractValidator<CreateConsumerRequest>
  {
    public CreateConsumerRequestValidator()
    {
      RuleFor(s => s.Legal_name)
        .NotNull().WithMessage("Nome completo é obrigatório.")
        .NotEmpty().WithMessage("Nome completo é obrigatório.");
      
      RuleFor(s => s.Document)
        .NotNull().WithMessage("Numero do documento é obrigatório.")
        .NotEmpty().WithMessage("Numero do documento é obrigatório.")
        .MinimumLength(11).MaximumLength(14).WithMessage("Numero documento deve conter no mínimo 11 e no maximo 14 caracteres.");

      RuleFor(s => s.Legal_name)
        .NotNull().WithMessage("Nome é obrigatório.")
        .NotEmpty().WithMessage("Nome é obrigatório.");

      RuleFor(s => s.User_id)
        .NotNull().WithMessage("Id do Usuário é obrigatório.")
        .NotEmpty().WithMessage("Id do Usuário é obrigatório.");

      RuleFor(s => s.Phone_number)
        .MinimumLength(10).MaximumLength(11).WithMessage("O Telefone deve conter no mínimo 10 digitos e no maximo 11.")
        .NotNull().WithMessage("Telefone é obrigatório.")
        .NotEmpty().WithMessage("Telefone é obrigatório.");

    }
  }
}
