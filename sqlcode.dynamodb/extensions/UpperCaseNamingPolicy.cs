using System.Text.Json;

namespace sqlcode.dynamodb.extensions;

public class UpperCaseNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name) =>
        name.ToUpper();
}
