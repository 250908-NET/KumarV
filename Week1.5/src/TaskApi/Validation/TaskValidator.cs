using System.ComponentModel.DataAnnotations;

namespace TaskApi.Validation;

public static class TaskValidator
{
    public static (bool ok, Dictionary<string, string[]> errors) Validate(object model)
    {
        var ctx = new ValidationContext(model); //which obj
        var results = new List<ValidationResult>();
        var valid = Validator.TryValidateObject(model, ctx, results, validateAllProperties: true); //this is what checks annotations

        var dict = results
            .GroupBy(r => r.MemberNames.FirstOrDefault() ?? string.Empty)
            .ToDictionary(g => g.Key, g => g.Select(r => r.ErrorMessage ?? "Invalid").ToArray());

        return (valid, dict);
    }
}
