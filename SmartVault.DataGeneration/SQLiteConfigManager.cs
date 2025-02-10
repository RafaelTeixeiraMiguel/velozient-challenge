using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartVault.DataGeneration
{
    public class SQLiteConfigManager
    {
        private readonly IConfigurationRoot? configuration;
        private readonly string? databaseName;

        public SQLiteConfigManager()
        {
            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json").Build();

            databaseName = configuration["DatabaseFileName"];
        }
        public void CreateDatabase()
        {
            SQLiteConnection.CreateFile(databaseName);
        }

        public SQLiteConnection CreateConnection()
        {
            if (!File.Exists(databaseName))
                CreateDatabase();

            SQLiteConnection connection = new SQLiteConnection(string.Format(configuration?["ConnectionStrings:DefaultConnection"] ?? "", databaseName));
            return connection ;
        }
    }
}
