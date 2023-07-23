using System.ComponentModel;

namespace sqlcode.dynamodb.clients
{
    public enum DynamoDataType
    {
        [Description("String")]
        S,

        [Description("Number")]
        N,

        [Description("Binary")]
        B
    }

}
