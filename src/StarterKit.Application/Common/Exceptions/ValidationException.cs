using FluentValidation.Results;

namespace StarterKit.Application.Common.Exceptions;

public class ValidationException : ApiException
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException()
        : base(400, "One or more validation failures have occurred.")
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(IEnumerable<ValidationFailure> failures)
        : this()
    {
        Errors = failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
    }
}