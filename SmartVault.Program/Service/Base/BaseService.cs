using SmartVault.Program.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartVault.Program.Service.Base
{
    public abstract class BaseService : IBaseService
    {
        public BaseService(SQLiteConnection connection) { }
    }
}
