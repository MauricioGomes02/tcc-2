namespace Tcc2.Domain.Models.Validation;

public class ValidationResult
{
    private readonly List<InvalidField> _invalidFields;

    public ValidationResult()
    {
        _invalidFields = new List<InvalidField>();
    }

    public void Add(string fieldName, string modelName, string description)
    {
        var fieldValidation = new InvalidField(fieldName, modelName, description);
        _invalidFields.Add(fieldValidation);
    }

    public void AddRange(IEnumerable<InvalidField> invalidFields)
    {
        _invalidFields.AddRange(invalidFields);
    }

    public bool IsValid => !_invalidFields.Any();
    public IReadOnlyCollection<InvalidField> InvalidFields => _invalidFields;
}
