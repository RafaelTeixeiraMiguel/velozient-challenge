using Dapper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SmartVault.Library;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace SmartVault.DataGeneration
{
    partial class Program
    {
        static void Main(string[] args)
        {
            var configManager = new SQLiteConfigManager();
            configManager.CreateDatabase();

            var connection = configManager.CreateConnection();

            CreateTestFile();

            using (connection)
            {
                CreateTables(connection);

                var userInserts = new StringBuilder();
                var accountInserts = new StringBuilder();
                var documentInserts = new StringBuilder();

                WriteInsertCommand(userInserts, accountInserts, documentInserts);
                ExecuteInserts(connection, userInserts, accountInserts, documentInserts);

                DisplayData(connection);
            }
        }

        static IEnumerable<DateTime> RandomDay()
        {
            DateTime start = new DateTime(1985, 1, 1);
            Random gen = new Random();
            int range = (DateTime.Today - start).Days;
            while (true)
                yield return start.AddDays(gen.Next(range));
        }

        static void CreateTables(SQLiteConnection connection)
        {
            var files = Directory.GetFiles(@"..\..\..\..\BusinessObjectSchema");
            for (int i = 0; i < files.Length; i++)
            {
                var serializer = new XmlSerializer(typeof(BusinessObject));
                var businessObject = serializer.Deserialize(new StreamReader(files[i])) as BusinessObject;
                connection.Execute(businessObject?.Script);

            }
        }

        static void CreateTestFile()
        {
            File.WriteAllText("TestDoc.txt", $"This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}");
        }

        static string RemoveLastComma(StringBuilder sb)
        {
            return sb.ToString().TrimEnd(',');
        }

        static void DisplayData(SQLiteConnection connection)
        {
            var accountData = connection.Query("SELECT COUNT(*) FROM Account;");
            Console.WriteLine($"AccountCount: {JsonConvert.SerializeObject(accountData)}");
            var documentData = connection.Query("SELECT COUNT(*) FROM Document;");
            Console.WriteLine($"DocumentCount: {JsonConvert.SerializeObject(documentData)}");
            var userData = connection.Query("SELECT COUNT(*) FROM User;");
            Console.WriteLine($"UserCount: {JsonConvert.SerializeObject(userData)}");
        }

        static void WriteInsertCommand(StringBuilder userInserts, StringBuilder accountInserts, StringBuilder documentInserts)
        {
            var documentNumber = 0;

            var documentPath = new FileInfo("TestDoc.txt").FullName;
            var document_length = new FileInfo(documentPath).Length;

            for (int i = 0; i < 100; i++)
            {
                var randomDayIterator = RandomDay().GetEnumerator();
                randomDayIterator.MoveNext();

                userInserts.Append($"('{i}', 'FName{i}', 'LName{i}', '{randomDayIterator.Current.ToString("yyyy-MM-dd")}', '{i}', 'UserName-{i}', 'e10adc3949ba59abbe56e057f20f883e', datetime('now')),");
                accountInserts.Append($"('{i}','Account{i}', datetime('now')),");

                for (int d = 0; d < 10000; d++, documentNumber++)
                {

                    documentInserts.Append($"('{documentNumber}','Document{i}-{d}.txt','{documentPath}','{document_length}','{i}', datetime('now')),");
                }
            }
        }

        static void ExecuteInserts(SQLiteConnection connection, StringBuilder userInserts, StringBuilder accountInserts, StringBuilder documentInserts)
        {
            connection.Execute($"INSERT INTO User (Id, FirstName, LastName, DateOfBirth, AccountId, Username, Password, CreatedOn) VALUES " + RemoveLastComma(userInserts));
            connection.Execute($"INSERT INTO Account (Id, Name, CreatedOn) VALUES " + RemoveLastComma(accountInserts));
            connection.Execute($"INSERT INTO Document (Id, Name, FilePath, Length, AccountId, CreatedOn) VALUES " + RemoveLastComma(documentInserts));
        }
    }
}