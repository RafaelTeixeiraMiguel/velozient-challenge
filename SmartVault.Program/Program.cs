using Dapper;
using Microsoft.Extensions.Configuration;
using SmartVault.DataGeneration;
using SmartVault.Program.BusinessObjects;
using SmartVault.Program.Service.Document;
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
        private static DocumentService _documentService;
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                return;
            }

            try
            {
                SQLiteConfigManager configManager = new SQLiteConfigManager();
                SQLiteConnection connection = configManager.CreateConnection();

                using (connection)
                {
                    _documentService = new DocumentService(connection);

                    WriteEveryThirdFileToFile(args[0]);
                    long totalSize = GetAllFileSizes();

                    Console.WriteLine($"The total size is: {totalSize}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public static long GetAllFileSizes()
        {
            return _documentService.GetAllFileSizes();
        }

        public static void WriteEveryThirdFileToFile(string accountId)
        {
            _documentService.WriteEveryThirdFileToFile(int.Parse(accountId));
        }
    }
}