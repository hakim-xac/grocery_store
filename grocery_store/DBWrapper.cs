using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grocery_store
{
    internal class DBWrapper
    {
        private SQLiteConnection db;

        public DBWrapper()
        {
            db = new SQLiteConnection("Data Source=grocery_store.db");
            db.Open();
        }

        public bool isOpen()
        {
            return selectQuery("select id from product_types").Rows.Count != 0;
        }

        public DataTable selectProductTypes()
        {
            return selectQuery("select * from product_types");
        }
        public DataTable selectProducts(int index)
        {
            return selectQuery("select * from products where id_product_type = " + index);
        }

        private DataTable selectQuery(string query)
        {
            SQLiteDataAdapter ad;
            DataTable dt = new DataTable();
            try
            {
                SQLiteCommand cmd;
                cmd = db.CreateCommand();
                cmd.CommandText = query;
                ad = new SQLiteDataAdapter(cmd);
                ad.Fill(dt);
            }
            catch { }
            return dt;
        }

        ~DBWrapper()
        {
            Close();
        }

        public void Close()
        {
            db.Close();
        }
        
    }
}
