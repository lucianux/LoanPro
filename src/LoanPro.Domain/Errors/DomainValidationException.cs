namespace LoanPro.Domain.Errors;

public sealed class DomainValidationException : Exception
{
    public IReadOnlyList<string> Errors { get; }

    public DomainValidationException(IEnumerable<string> errors)
        : base("Domain validation failed.")
    {
        Errors = errors.ToList().AsReadOnly();
    }
}
