using Dapper;
using SmartVault.Program.BusinessObjects;
using SmartVault.Program.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartVault.Program.Repositories.Document
{
    public class DocumentRepository : BaseRepository, IDocumentRepository
    {
        public DocumentRepository(SQLiteConnection connection) : base(connection)
        {
        }

        public List<BusinessObjects.Document> GetAll()
        {
            return _connection.Query<BusinessObjects.Document>($"SELECT * FROM {nameof(Document)}").ToList();
        }

        public List<BusinessObjects.Document> GetByAccountId(int accountId)
        {
            return _connection.Query<BusinessObjects.Document>($"SELECT * FROM {nameof(Document)} WHERE AccountId = {accountId}").AsList();
        }
    }
}
