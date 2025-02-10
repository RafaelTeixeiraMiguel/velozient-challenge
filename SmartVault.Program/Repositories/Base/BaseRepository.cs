using SmartVault.Program.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartVault.Program.Repositories.Base
{
    public abstract class BaseRepository : IBaseRepository<BaseBusinessObject>
    {
        protected readonly SQLiteConnection _connection;
        public BaseRepository(SQLiteConnection connection)
        {
            _connection = connection;
        }
    }
}
