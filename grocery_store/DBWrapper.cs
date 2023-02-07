﻿using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

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
            return selectQuery("select id from product_types limit 1").Rows.Count != 0;
        }

        public DataTable selectProductTypes()
        {
            return selectQuery("select * from product_types");
        }
        public DataTable selectProducts(int index)
        {
            return selectQuery("select * from products where id_product_type = " + index);
        }

        public bool WriteToProductTypes(string name, int row, int col)
        {
            SQLiteCommand cmd = db.CreateCommand();
            string query = "insert into product_types('Наименование отдела', 'Номер ряда хранения', 'Номер секции хранения')" +
                " values('"+ name +"', '"+ row +"', '"+ col +"')";
            cmd.CommandText = query;
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool isNoteExists(string query)
        {
            return selectQuery(query).Rows.Count > 0;
        }





        private DataTable selectQuery(string query)
        {
            SQLiteDataAdapter ad;
            DataTable dt = new DataTable();
            try
            {
                SQLiteCommand cmd = db.CreateCommand();
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
