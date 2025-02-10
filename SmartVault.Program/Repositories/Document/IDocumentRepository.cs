using System;
using System.Collections.Generic;
using SmartVault.Program.BusinessObjects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartVault.Program.Repositories.Document
{
    public interface IDocumentRepository
    {
        List<BusinessObjects.Document> GetAll();
        List<BusinessObjects.Document> GetByAccountId(int accountId);
    }
}
