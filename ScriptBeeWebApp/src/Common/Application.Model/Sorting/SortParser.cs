namespace ScriptBee.Application.Model.Sorting;

public static class SortParser
{
    public static IReadOnlyList<Sort<TField>> Parse<TField>(string? input)
        where TField : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return [];
        }

        var parts = input.Split(',', StringSplitOptions.RemoveEmptyEntries);
        var result = new List<Sort<TField>>();

        foreach (var raw in parts)
        {
            var trimmed = raw.Trim();

            var direction = trimmed.StartsWith('-') ? SortOrder.Descending : SortOrder.Ascending;

            var fieldName = trimmed.TrimStart('-', '+');

            if (Enum.TryParse<TField>(fieldName, true, out var field))
            {
                result.Add(new Sort<TField>(field, direction));
            }
        }

        return result;
    }
}
