//using Org.Apache.Http.Authentication;
using SQLite;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ejercicio2._2_Grupo2.Controllers
{
    public class FirmasController
    {
        readonly SQLiteAsyncConnection _connection;
        public FirmasController()
        {
            SQLite.SQLiteOpenFlags extensiones = SQLite.SQLiteOpenFlags.ReadWrite |
            //SQLite.SQLiteOpenFlags.ReadOnly |
                SQLite.SQLiteOpenFlags.Create |
                SQLite.SQLiteOpenFlags.SharedCache;

            _connection = new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory, "DBFirma.db3"), extensiones);
            _connection.CreateTableAsync<Models.Firmas>();
        }
        //Crud Methods
        //Create
        public async Task<int> Store(Models.Firmas firmas)
        {
            if (firmas.Id == 0)
            {
                return await _connection.InsertAsync(firmas);
            }
            else
            {
                return await _connection.UpdateAsync(firmas);
            }
        }

        //Read
        public async Task<List<Models.Firmas>> GetList()
        {
            return await _connection.Table<Models.Firmas>().ToListAsync();
        }

    }
}
