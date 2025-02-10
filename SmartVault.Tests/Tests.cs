using Dapper;
using System;
using System.Data.SQLite;
using System.IO;
using Xunit;

namespace Tests
{
    public class Tests
    {
        private readonly SQLiteConnection connection;
        private readonly string tempDirectory;

        public Tests()
        {
            connection = new SQLiteConnection("Data Source=:memory:;Version=3;");
            connection.Open();

            connection.Execute("CREATE TABLE Document (Id INTEGER PRIMARY KEY,Name TEXT,FilePath TEXT,Length INTEGER,AccountId INTEGER,CreatedOn TEXT);");

            SmartVault.Program.Program.BuildConnection(connection);
            tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDirectory);
        }

        [Fact]
        public void GetAllFileSizesReturnsZeroForEmptyDatabase()
        {
            long totalFileSize = SmartVault.Program.Program.GetAllFileSizes();

            Assert.Equal(0, totalFileSize);
        }

        [Fact]
        public void GetAllFileSizesReturnsZeroForNonExistentFile()
        {
            connection.Execute("INSERT INTO Document (AccountId, FilePath) VALUES ('1', 'NonExistentFile.txt')");

            long totalFileSize = SmartVault.Program.Program.GetAllFileSizes();

            Assert.Equal(0, totalFileSize);
        }

        [Fact]
        public void GetAllFileSizesForExistentFiles()
        {
            string filePath1 = CreateTempFile("File1.txt", "XXXXX");
            string filePath2 = CreateTempFile("File2.txt", "XXXXXXXXXX");

            connection.Execute($"INSERT INTO Document (AccountId, FilePath) VALUES ('1', '{filePath1}'), ('1', '{filePath2}')");

            long totalFileSize = SmartVault.Program.Program.GetAllFileSizes();

            Assert.Equal(15, totalFileSize);
        }

        [Fact]
        public void WriteEveryThirdFileToFileCreatesAndWritesFileEmptyContent()
        {
            string filePath1 = CreateTempFile("File1.txt", "Content 1");
            string filePath2 = CreateTempFile("File2.txt", "Content 2");

            connection.Execute($"INSERT INTO Document (AccountId, FilePath) VALUES ('1', '{filePath1}'), ('1', '{filePath2}')");

            SmartVault.Program.Program.WriteEveryThirdFileToFile("1");

            string outputFilePath = Path.Combine("../../../../ThirFileOutput", "1.txt");
            Assert.True(File.Exists(outputFilePath));
            Assert.True(string.IsNullOrEmpty(File.ReadAllText(outputFilePath)));
        }

        [Fact]
        public void WriteEveryThirdFileToFileCreatesAndWritesFile()
        {
            string filePath1 = CreateTempFile("File1.txt", "Not relevant");
            string filePath2 = CreateTempFile("File2.txt", "Not relevant");
            string filePath3 = CreateTempFile("File3.txt", "Smith Property");
            string filePath4 = CreateTempFile("File4.txt", "Not relevant");

            connection.Execute($@"
            INSERT INTO Document (AccountId, FilePath) 
            VALUES 
                ('1', '{filePath1}'), 
                ('1', '{filePath2}'), 
                ('1', '{filePath3}'),
                ('1', '{filePath4}')");

            SmartVault.Program.Program.WriteEveryThirdFileToFile("1");

            string outputFilePath = Path.Combine("../../../../ThirFileOutput", "1.txt");
            Assert.True(File.Exists(outputFilePath));

            string outputContent = File.ReadAllText(outputFilePath);
            Assert.Contains("Smith Property", outputContent);
        }

        private string CreateTempFile(string fileName, string content)
        {
            string filePath = Path.Combine(tempDirectory, fileName);
            File.WriteAllText(filePath, content);
            return filePath;
        }
    }
}