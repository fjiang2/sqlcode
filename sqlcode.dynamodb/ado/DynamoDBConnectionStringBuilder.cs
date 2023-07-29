using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;

namespace sqlcode.dynamodb.ado
{
    public class DynamoDbConnectionStringBuilder : DbConnectionStringBuilder
    {
        public AWSCredentials Credentials { get; }
        public RegionEndpoint Region { get; }

        /// <summary>
        /// connectionString = "accessKey=;secretKey=;region=";
        /// </summary>
        /// <param name="connectionString"></param>
        public DynamoDbConnectionStringBuilder(string connectionString)
        {
            ConnectionString = connectionString;

            string accessKey = $"{this["accessKey"]}";
            string secretKey = $"{this["secretKey"]}";

            if (string.IsNullOrWhiteSpace(accessKey) || string.IsNullOrWhiteSpace(secretKey))
            {
                Credentials = FallbackCredentialsFactory.GetCredentials();
            }
            else
            {
                Credentials = new BasicAWSCredentials(accessKey, secretKey);
            }

            string region = $"{this["region"]}";
            if (string.IsNullOrWhiteSpace(region))
            {
                Region = RegionEndpoint.USEast1;
            }
            else
            {
                Region = RegionEndpoint.GetBySystemName(region);
            }
        }

        public DynamoDbConnectionStringBuilder(AWSCredentials credentials, RegionEndpoint region)
        {
            Credentials = credentials;
            Region = region;
        }

        public DynamoDbConnectionStringBuilder()
        {
            Credentials = FallbackCredentialsFactory.GetCredentials();
            Region = RegionEndpoint.USEast1;
        }

    }
}
