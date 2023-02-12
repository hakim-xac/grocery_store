using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace grocery_store
{
    public partial class Form3 : Form
    {
        DBWrapper db;
        Hashtable ht = new Hashtable();
        string field_product_name = "";
        public Form3()
        {
            InitializeComponent();
        }

        private bool addToProducts(string name, string provider, string unit
                , float buy_price, float selling_price
                , int coming_product, int remainder_product, int departament_id)
        {
            return !db.isExistsProduct(name, departament_id)
                && db.WriteToProducts(name, provider, unit
                , buy_price, selling_price
                , coming_product, remainder_product, departament_id);
        }

        private bool updateToProducts(string name, string provider, string unit
                , float buy_price, float selling_price
                , int coming_product, int remainder_product, int departament_id, int id_product
                , bool updated)
        {
            return db.isExistsProduct(id_product) 
                && db.updateProduct(name, provider, unit
                , buy_price, selling_price
                , coming_product, remainder_product, departament_id, id_product
                , updated);
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            db = new DBWrapper();
            if (!db.isOpen() 
                || !SecondaryMethods.fillHashTableFromBD(ht, db.selectProductTypes("\"Наименование отдела\", \"id\"")))
            {
                MessageBox.Show("Ошибка загрузки базы данных!");
                Application.Exit();
            }
            SecondaryMethods.fillComboBox(comboBox1, ht); 
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == String.Empty 
                || textBox2.Text == String.Empty 
                || textBox3.Text == String.Empty 
                || textBox4.Text == String.Empty 
                || textBox5.Text == String.Empty 
                || textBox6.Text == String.Empty 
                || textBox7.Text == String.Empty)
            {
                MessageBox.Show("Все поля должн быть заполнены!");
                return;
            }

            string name_product = textBox1.Text;
            string provider = textBox2.Text;
            string unit = textBox5.Text;

            float buy_price, selling_price;
            float.TryParse(textBox3.Text, NumberStyles.Any, new CultureInfo("en-US"), out buy_price);
            float.TryParse(textBox4.Text, NumberStyles.Any, new CultureInfo("en-US"), out selling_price);

            int coming_product = int.Parse(textBox6.Text);
            int remainder_product = int.Parse(textBox7.Text);
            int departament_id = int.Parse((string)ht[comboBox1.Items[comboBox1.SelectedIndex].ToString()]);
            DataTable dt = db.selectIdFromProductTypes(comboBox1.Items[comboBox1.SelectedIndex].ToString());

            name_product = SecondaryMethods.noSQLInjection(name_product);
            provider = SecondaryMethods.noSQLInjection(provider);
            unit = SecondaryMethods.noSQLInjection(unit);

            Form1 main = this.Owner as Form1;
            int id_products = main.id_products;
            if (id_products == 0)
            {
                if (!addToProducts(name_product, provider, unit
                , buy_price, selling_price
                , coming_product, remainder_product, departament_id))
                {
                    MessageBox.Show("Не удалось добавить!\r\nПовторите ввод!");
                    return;
                }
                main.is_update_products = true;
                MessageBox.Show("Данные успешно добавлены!");
            }
            else
            {
                bool updated = field_product_name != String.Empty && !field_product_name.Equals(textBox1.Text);
                if (!updateToProducts(name_product, provider, unit
                , buy_price, selling_price
                , coming_product, remainder_product, departament_id, id_products
                , updated))
                {
                    MessageBox.Show("Не удалось обновить!\r\nПовторите ввод!");
                    return;
                }
                main.is_update_products = true;
                MessageBox.Show("Данные успешно обновлены!");
            }

            Close();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (!float.TryParse(textBox3.Text, NumberStyles.Any, new CultureInfo("en-US"), out _)) textBox3.Clear();
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            if (!float.TryParse(textBox4.Text, NumberStyles.Any, new CultureInfo("en-US"), out _)) textBox4.Clear();
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            if (!int.TryParse(textBox6.Text, out int _)) textBox6.Clear();
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            if (!int.TryParse(textBox7.Text, out int _)) textBox7.Clear();
        }

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            Form1 form1 = this.Owner as Form1;
            if (form1.id_product_types > 0) form1.id_product_types = 0;
            if (form1.id_products > 0) form1.id_products = 0;
        }

        private void Form3_Shown(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            textBox6.Clear();
            textBox7.Clear();
            Form1 form1 = this.Owner as Form1;

            comboBox1.SelectedIndex = 0;
            string name = form1.name_product_type;

            for (int i = 0, ie = comboBox1.Items.Count; i < ie; ++i)
            {
                if (comboBox1.Items[i].Equals(name)) { comboBox1.SelectedIndex = i; break; }
            }

            if (form1.id_products > 0)
            {
                int id = form1.id_products;
                DataTable dt = db.selectFieldProducts("id", id.ToString());
                textBox1.Text = field_product_name = dt.Rows[0][1].ToString();
                textBox2.Text = dt.Rows[0][2].ToString();
                textBox3.Text = dt.Rows[0][3].ToString();
                textBox4.Text = dt.Rows[0][4].ToString();
                textBox5.Text = dt.Rows[0][5].ToString();
                textBox6.Text = dt.Rows[0][6].ToString();
                textBox7.Text = dt.Rows[0][7].ToString();
                button1.Text = "Изменить";
                return;
            }

            button1.Text = "Добавить";
        }
    }
}
