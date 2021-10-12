using Amazon.DynamoDBv2.DataModel;

namespace NameSorterProcessor.Models
{
    [DynamoDBTable("namesTable")]
    public class NameModel
    {
        [DynamoDBHashKey]
        public string name { get; set; }
        public int amount { get; set; }

    }
}