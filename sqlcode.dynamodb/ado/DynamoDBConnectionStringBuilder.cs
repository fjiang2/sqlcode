using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using mudu.aws.core;

namespace sqlcode.dynamodb.ado
{
    public class DynamoDbConnectionStringBuilder : DbConnectionStringBuilder
    {
        public AWSCredentials Credentials { get; }
        public RegionEndpoint Region { get; }
        public IAccount Account { get; }

        /// <summary>
        /// connectionString = "Data Source=dynamoDB;Initial Catalog=dev;accessKey=;secretKey=;region=;";
        /// </summary>
        /// <param name="connectionString"></param>
        public DynamoDbConnectionStringBuilder(string connectionString)
        {
            this.Account = new Account
            {
                Name = "DynamoDB",
            };

            base.ConnectionString = connectionString;
            if (this.ContainsKey("accessKey") && this.ContainsKey("secretKey"))
            {
                string accessKey = $"{this["accessKey"]}";
                string secretKey = $"{this["secretKey"]}";

                Credentials = new BasicAWSCredentials(accessKey, secretKey);

                Account.AccessKey = accessKey;
                Account.SecretKey = secretKey;
            }
            else
            {
                Credentials = FallbackCredentialsFactory.GetCredentials();
            }

            if (this.ContainsKey("region"))
            {
                string region = $"{this["region"]}";
                Region = RegionEndpoint.GetBySystemName(region);
                Account.Region = region;
            }
            else
            {
                Region = RegionEndpoint.USEast1;
                Account.Region = Region.SystemName;
            }
        }

        public DynamoDbConnectionStringBuilder(AWSCredentials credentials, RegionEndpoint region, string? initialCatalog = null)
        {
            this.Credentials = credentials;
            this.Region = region;

            var x = credentials.GetCredentials();
            this.ConnectionString = $"data source=dynamoDB;InitialCatalog={initialCatalog};accessKey={x.AccessKey};secretKey={x.SecretKey};region={region.SystemName};";
            this.Account = new Account
            {
                Name = "DynamoDB",
                Region = region.SystemName,
            };
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
