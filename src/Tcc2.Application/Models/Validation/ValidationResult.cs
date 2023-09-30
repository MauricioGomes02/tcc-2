namespace Tcc2.Application.Models.Validation;

public class ValidationResult
{
    private readonly List<InvalidField> _invalidFields;

    internal ValidationResult()
    {
        _invalidFields = new List<InvalidField>();
    }

    internal void Add(string fieldName, string modelName, string description)
    {
        var fieldValidation = new InvalidField(fieldName, modelName, description);
        _invalidFields.Add(fieldValidation);
    }

    internal void AddRange(IEnumerable<InvalidField> invalidFields)
    {
        _invalidFields.AddRange(invalidFields);
    }

    public bool IsValid => !_invalidFields.Any();
    public IReadOnlyCollection<InvalidField> InvalidFields => _invalidFields;
}
