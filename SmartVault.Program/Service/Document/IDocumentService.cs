using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartVault.Program.Service.Document
{
    public interface IDocumentService
    {
        void WriteEveryThirdFileToFile(int accountId);
        long GetAllFileSizes();
    }
}
