using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Xml.Linq;

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

        public DataTable selectProductTypes(string field="")
        {
            return field == String.Empty 
                ? selectQuery("select * from product_types")
                : selectQuery("select " + field + " from product_types");
        }

        public DataTable selectIdFromProductTypes(string field_departament_name)
        {
            return selectQuery("select id from product_types where \"Наименование отдела\"=\""+ field_departament_name+"\"");
        }

        public DataTable selectFieldProducts(string field, string value)
        {
            return selectQuery("select * from products where \""+field+"\"=\""+value+"\"");
        }

        public DataTable selectFieldProductTypes(string field, string value)
        {
            return selectQuery("select * from product_types where \""+field+"\"=\""+value+"\"");
        }

        public DataTable selectProducts(int index)
        {
            return selectQuery("select * from products where id_product_type = " + index);
        }

        public DataTable selectProductsContains(int index, string field_name, string string_find)
        {
            return selectQuery("select * from products where id_product_type="+index+" and " +
                "\""+field_name + "\" like \"%"+string_find+"%\"");
        }

        public DataTable selectProductsEquals(int index, string field_name, string string_find)
        {
            return selectQuery("select * from products where id_product_type=" + index + " and " +
                "\"" + field_name + "\"=\"" + string_find + "\"");
        }

        public bool WriteToProductTypes(string name, int row, int col)
        {
            string query = "insert into " +
                "product_types('Наименование отдела', 'Номер ряда хранения', 'Номер секции хранения')" +
                " values('" + name + "', '" + row + "', '" + col + "')";
            return executeNonQuery(query);
        }

        public bool updateProductTypes(string name, int row, int col, int id)
        {
            string query = "update product_types set \"Наименование отдела\" = \"" + name
                + "\", \"Номер ряда хранения\"=" + row
                + " , \"Номер секции хранения\"=" + col
                + " where id =\"" + id + "\"";

            return executeNonQuery(query);
        }

        public bool updateProduct(string name, string provider, string unit
            , float buy_price, float selling_price
            , int coming_product, int remainder_product, int departament_id, int id_product
            , bool updated)
        {
            if(updated) if (isExistsProduct(name, departament_id)) return false;

            string query = "update products set \"Название продукта\" = \"" + name
                + "\", \"Поставщик\"=\"" + provider+"\""
                + " , \"Единица измерения\"=\"" + unit
                + "\", \"Цена покупки\"=\"" + buy_price
                + "\", \"Цена продажи\"=\"" + selling_price
                + "\", \"Поступление товара\"=\"" + coming_product
                + "\", \"Остаток товара\"=\"" + remainder_product
                + "\", \"id_product_type\"=\"" + departament_id + "\""
                + " where id =\"" + id_product + "\"";

            return executeNonQuery(query);
        }

        public bool WriteToProducts(string name, string provider, string unit
            , float buy_price, float selling_price
            , int coming_product, int remainder_product, int departament_id)
        {
            string query = "insert into products(" +
                "'Название продукта', 'Поставщик', 'Единица измерения', " +
                "'Цена покупки', 'Цена продажи', " +
                "'Поступление товара', 'Остаток товара', 'id_product_type'" +
                ")" +
                " values('" + name + "', '" + provider + "', '" + unit + "'" +
                ", '" + buy_price + "', '" + selling_price + "'" +
                ", '" + coming_product + "', '" + remainder_product + "', '" + departament_id + "')";
            return executeNonQuery(query);
        }

        public bool isExistsProductType(string name, int row, int col)
        {
            string query = "select id from product_types where \"Наименование отдела\" = \"" + name
                + "\" or \"Номер ряда хранения\"=" + row
                + " and \"Номер секции хранения\"=" + col
                + " limit 1";
            return selectQuery(query).Rows.Count > 0;
        }
        public bool isExistsProduct(string name, int departament_id)
        {
            string query = "select id from products where \"Название продукта\" = \"" + name+"\""
                + " and \"id_product_type\"=\"" + departament_id+"\"";
            return selectQuery(query).Rows.Count > 0;
        }
        public bool isExistsProduct(int id_product)
        {
            string query = "select id from products where \"id\"=" + id_product;
            return selectQuery(query).Rows.Count > 0;
        }

        public bool deleteProductType(int id)
        {
            executeNonQuery("delete from products where id_product_type=" + id);
            return executeNonQuery("delete from product_types where id = " + id);
        }
        public bool deleteProduct(int id)
        {
            return executeNonQuery("delete from products where id=" + id);
        }

        private bool executeNonQuery(string query)
        {
            SQLiteCommand cmd = db.CreateCommand();
            cmd.CommandText = query;
            return cmd.ExecuteNonQuery() > 0;
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
