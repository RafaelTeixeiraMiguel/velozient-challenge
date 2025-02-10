using Microsoft.Extensions.Configuration;
using SmartVault.Program.Repositories.Document;
using SmartVault.Program.Service.Base;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace SmartVault.Program.Service.Document
{
    public class DocumentService : BaseService, IDocumentService
    {
        private readonly IDocumentRepository _repository;
        public DocumentService(SQLiteConnection connection) : base(connection)
        {
            _repository = new DocumentRepository(connection);
        }

        public void WriteEveryThirdFileToFile(int accountId)
        {
            string folderPath = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json").Build()["ThirdFileOutput"];

            Directory.CreateDirectory(folderPath);

            string file = Path.Combine(folderPath, $"{accountId}.txt");
            File.WriteAllText(file, string.Empty);

            List<BusinessObjects.Document> documents = _repository.GetByAccountId(accountId);

            for (int index = 2; index < documents.Count; index += 3)
            {
                BusinessObjects.Document document = documents[index];

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

        public long GetAllFileSizes()
        {
            List<BusinessObjects.Document> documents = _repository.GetAll();

            long totalSize = 0;

            foreach (BusinessObjects.Document document in documents)
            {
                if (File.Exists(document.FilePath))
                {
                    totalSize += new FileInfo(document.FilePath).Length;
                }
            }

            return totalSize;
        }
    }
}
