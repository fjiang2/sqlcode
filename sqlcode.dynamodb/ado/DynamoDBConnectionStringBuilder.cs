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
        /// connectionString = "Data Source=dynamoDB;Initial Catalog=dev;accessKey=;secretKey=;region=;";
        /// </summary>
        /// <param name="connectionString"></param>
        public DynamoDbConnectionStringBuilder(string connectionString)
        {
            base.ConnectionString = connectionString;

            string accessKey = $"{this["accessKey"]}";
            string secretKey = $"{this["secretKey"]}";

            if (string.IsNullOrWhiteSpace(accessKey) || string.IsNullOrWhiteSpace(secretKey))
                Credentials = FallbackCredentialsFactory.GetCredentials();
            else
                Credentials = new BasicAWSCredentials(accessKey, secretKey);

            string region = $"{this["region"]}";
            if (string.IsNullOrWhiteSpace(region))
                Region = RegionEndpoint.USEast1;
            else
                Region = RegionEndpoint.GetBySystemName(region);
        }

        public DynamoDbConnectionStringBuilder(AWSCredentials credentials, RegionEndpoint region, string? initialCatalog = null)
        {
            this.Credentials = credentials;
            this.Region = region;

            var x = credentials.GetCredentials();
            this.ConnectionString = $"data source=dynamoDB;InitialCatalog={initialCatalog};accessKey={x.AccessKey};secretKey={x.SecretKey};region={region.SystemName};";
        }

        public DynamoDbConnectionStringBuilder()
            : this(FallbackCredentialsFactory.GetCredentials(), RegionEndpoint.USEast1)
        {
        }

        public string InitialCatalog
        {
            get => (string)this["Initial Catalog"];
            set => this["Initial Catalog"] = value;
        }

        public string DataSource
        {
            get => (string)this["Data Source"];
            set => this["Data Source"] = value;
        }
    }
}
