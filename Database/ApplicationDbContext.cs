//using SQLite;
//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Moneybank.Database
//{
//    public class ApplicationDbContext
//    {
//        public SQLiteAsyncConnection _dbConnection;

//        //Application DB
//        public readonly static string nameSpace = "Moneybank.database.";

//        public const string DatabaseFileName = "Moneybank.Database.db3";
//        public static string databasepath => Path.Combine(FileSystem.AppDataDirectory, DatabaseFileName);

//        public const SQLite.SQLiteOpenFlags Flags =
//            SQLite.SQLiteOpenFlags.ReadWrite |
//            SQLite.SQLiteOpenFlags.Create |
//            SQLite.SQLiteOpenFlags.SharedCache;

//        public ApplicationDbContext()
//        {
//            if (_dbConnection is null)
//            {
//                _dbConnection = new SQLiteAsyncConnection(databasepath, Flags);
//            }
//        }
//    }
//}
