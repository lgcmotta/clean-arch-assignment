namespace OrderManagement.Application.Options;

public class HashIdsOptions
{
    public const string SectionName = "HashIds";

    public required string Salt { get; init; }

    public int MinHashLength { get; init; } = 7;

    public static HashIdsOptions Default => new() { Salt = Guid.NewGuid().ToString().Replace("-", string.Empty), };
}