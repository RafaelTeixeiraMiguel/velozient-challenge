using Dapper;
using Microsoft.Extensions.Configuration;
using SmartVault.DataGeneration;
using SmartVault.Program.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using System.Linq;

namespace SmartVault.Program
{
    public class Program
    {
        private static SQLiteConnection _connection;
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                return;
            }

            try
            {
                BuildConnection();

                using (_connection)
                {
                    WriteEveryThirdFileToFile(args[0]);
                    GetAllFileSizes();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public static void BuildConnection(SQLiteConnection? connection = null)
        {
            SQLiteConfigManager configManager = new SQLiteConfigManager();
            _connection = connection ?? configManager.CreateConnection();
        }

        public static long GetAllFileSizes()
        {
            List<Document> documents = _connection.Query<Document>($"SELECT * FROM {nameof(Document)}").ToList();

            long totalSize = 0;

            foreach (Document document in documents)
            {
                if (File.Exists(document.FilePath))
                {
                    totalSize += new FileInfo(document.FilePath).Length;
                }
            }

            Console.WriteLine($"The total size is: {totalSize}");
            return totalSize;
        }

        public static void WriteEveryThirdFileToFile(string accountId)
        {
            string folderPath = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json").Build()["ThirdFileOutput"];

            Directory.CreateDirectory(folderPath);

            string file = Path.Combine(folderPath, $"{accountId}.txt");

            File.WriteAllText(file, string.Empty);

            List<Document> documents = _connection.Query<Document>($"SELECT * FROM {nameof(Document)} WHERE AccountId = {accountId}").AsList();

            for (int index = 2; index < documents.Count; index += 3)
            {
                Document document = documents[index];

                if (File.Exists(document.FilePath))
                {
                    string fileContent = File.ReadAllText(document.FilePath);

                    if (fileContent.Contains("Smith Property"))
                    {
                        File.AppendAllText(file, fileContent);
                    }
                }
            }
        }
    }
}