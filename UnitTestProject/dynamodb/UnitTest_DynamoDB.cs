using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#if NET6_0
using sqlcode.dynamodb.clients;

namespace UnitTestProject.dynamodb
{
    [TestClass]
    public class UnitTest_DynamoDB
    {
        [TestMethod]
        public async Task Test_PartiQL_SELECT()
        {
            DbClient client = new DbClient();
            string SQL = "SELECT * FROM \"DeviceList-taiga\" WHERE TenantId='dev'";
            var rows = await client.ExecuteStatementAsync(SQL);
        }

        [TestMethod]
        public async Task Test_PartiQL_INSERT()
        {
            /*
            INSERT INTO TypesTable VALUE {'primarykey':'1', 
            'NumberType':1,
            'MapType' : {'entryname1': 'value', 'entryname2': 4}, 
            'ListType': [1,'stringval'], 
            'NumberSetType':<<1,34,32,4.5>>, 
            'StringSetType':<<'stringval','stringval2'>>
            }             
            */

            string SQL = @"INSERT INTO TypesTable value 
            {'primarykey':'1', 'NumberType':1, 
            'MapType' : {'entryname1': 'value', 'entryname2': 4}, 
            'ListType': [1,'stringval'], 
            'NumberSetType':<<1,34,32,4.5>>, 
            'StringSetType':<<'stringval','stringval2'>>}
            ";

            DbClient client = new DbClient();
            var rows = await client.ExecuteStatementAsync(SQL);
        }


        [TestMethod]
        public async Task Test_PartiQL_UPDATE()
        {
            /*
            UPDATE TypesTable 
            SET NumberType=NumberType + 100 
            SET MapType.NewMapEntry=[2020, 'stringvalue', 2.4]
            SET ListType = LIST_APPEND(ListType, [4, <<'string1', 'string2'>>])
            SET NumberSetType= SET_ADD(NumberSetType, <<345, 48.4>>)
            SET StringSetType = SET_ADD(StringSetType, <<'stringsetvalue1', 'stringsetvalue2'>>)
            WHERE primarykey='1'

            UPDATE TypesTable
            SET NumberType = NumberType - 1
            REMOVE ListType[1]
            REMOVE MapType.NewMapEntry
            SET NumberSetType = SET_DELETE(NumberSetType, << 345 >>)
            SET StringSetType = SET_DELETE(StringSetType, << 'stringsetvalue1' >>)
            WHERE primarykey = '1'
            */

            string SQL = @"UPDATE TypesTable
            SET NumberType = NumberType + 100
            SET MapType.NewMapEntry =[2020, 'stringvalue', 2.4]
            SET ListType = LIST_APPEND(ListType, [4, << 'string1', 'string2' >>])
            SET NumberSetType = SET_ADD(NumberSetType, << 345, 48.4 >>)
            SET StringSetType = SET_ADD(StringSetType, << 'stringsetvalue1', 'stringsetvalue2' >>)
            WHERE primarykey = '1'
            ";


            DbClient client = new DbClient();
            var rows = await client.ExecuteStatementAsync(SQL);
        }

        [TestMethod]
        public async Task Test_PartiQL_DELETE()
        {
            /*
              DELETE FROM "Music" WHERE "Artist" = 'Acme Band' AND "SongTitle" = 'PartiQL Rocks' 
              DELETE FROM "Music" WHERE "Artist" = 'Acme Band' AND "SongTitle" = 'PartiQL Rocks' RETURNING ALL OLD *
             */

            string SQL = "";
            DbClient client = new DbClient();
            var rows = await client.ExecuteStatementAsync(SQL);
        }


     
    }
}
#endif