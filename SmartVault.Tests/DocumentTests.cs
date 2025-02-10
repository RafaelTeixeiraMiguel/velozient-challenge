using Dapper;
using SmartVault.Program.Service.Document;
using System;
using System.Data.SQLite;
using System.IO;
using Xunit;

namespace Tests
{
    public class DocumentTests
    {
        private readonly SQLiteConnection connection;
        private readonly IDocumentService _documentService;
        private readonly string tempDirectory;

        public DocumentTests()
        {
            connection = new SQLiteConnection("Data Source=:memory:;Version=3;");
            connection.Open();

            _documentService = new DocumentService(connection);
            connection.Execute("CREATE TABLE Document (Id INTEGER PRIMARY KEY,Name TEXT,FilePath TEXT,Length INTEGER,AccountId INTEGER,CreatedOn TEXT);");

            tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDirectory);
        }

        [Fact]
        public void GetAllFileSizesReturnsZeroForEmptyDatabase()
        {
            long totalFileSize = _documentService.GetAllFileSizes();

            Assert.Equal(0, totalFileSize);
        }

        [Fact]
        public void GetAllFileSizesReturnsZeroForNonExistentFile()
        {
            connection.Execute("INSERT INTO Document (AccountId, FilePath) VALUES ('1', 'NonExistentFile.txt')");

            long totalFileSize = _documentService.GetAllFileSizes();

            Assert.Equal(0, totalFileSize);
        }

        [Fact]
        public void GetAllFileSizesForExistentFiles()
        {
            string filePath1 = CreateTempFile("File1.txt", "XXXXXXXXXX");
            string filePath2 = CreateTempFile("File2.txt", "XXXXXXXXXX");

            connection.Execute($"INSERT INTO Document (AccountId, FilePath) VALUES ('1', '{filePath1}'), ('1', '{filePath2}')");

            long totalFileSize = _documentService.GetAllFileSizes();

            Assert.Equal(20, totalFileSize);
        }

        [Fact]
        public void WriteEveryThirdFileToFileCreatesAndWritesFileEmptyContent()
        {
            string filePath1 = CreateTempFile("File1.txt", "TEXT 1");
            string filePath2 = CreateTempFile("File2.txt", "TEXT 2");

            connection.Execute($"INSERT INTO Document (AccountId, FilePath) VALUES ('1', '{filePath1}'), ('1', '{filePath2}')");

            _documentService.WriteEveryThirdFileToFile(1);

            string outputFilePath = Path.Combine("../../../../ThirFileOutput", "1.txt");
            Assert.True(File.Exists(outputFilePath));
            Assert.True(string.IsNullOrEmpty(File.ReadAllText(outputFilePath)));
        }

        [Fact]
        public void WriteEveryThirdFileToFileCreatesAndWritesFile()
        {
            string filePath1 = CreateTempFile("File1.txt", "TEXT");
            string filePath2 = CreateTempFile("File2.txt", "TEXT");
            string filePath3 = CreateTempFile("File3.txt", "Smith Property");
            string filePath4 = CreateTempFile("File4.txt", "TEXT");

            connection.Execute($@"
            INSERT INTO Document (AccountId, FilePath) 
            VALUES 
                ('1', '{filePath1}'), 
                ('1', '{filePath2}'), 
                ('1', '{filePath3}'),
                ('1', '{filePath4}')");

            _documentService.WriteEveryThirdFileToFile(1);

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