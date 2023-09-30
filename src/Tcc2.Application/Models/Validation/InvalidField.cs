namespace Tcc2.Application.Models.Validation;

public class InvalidField
{
    public InvalidField(string fieldName, string modelName, string description)
    {
        FieldName = fieldName;
        ModelName = modelName;
        Description = description;
    }

    public string FieldName { get; private set; }
    public string ModelName { get; private set; }
    public string Description { get; private set; }
}
