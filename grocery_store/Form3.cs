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

namespace grocery_store
{
    public partial class Form3 : Form
    {
        DBWrapper db;
        Hashtable ht = new Hashtable();

        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            db = new DBWrapper();
            if (!db.isOpen() 
                || !SecondaryMethods.fillHashTableFromBD(ht, db.selectFieldProductTypes("\"Наименование отдела\", \"id\"")))
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

            name_product.Replace("\"", "\"\"");
            provider.Replace("\"", "\"\"");
            unit.Replace("\"", "\"\"");


            string query = "select id from products where \"Название продукта\" = \"" + name_product
                + "\" and \"Поставщик\"=" + provider
                + " and \"Цена покупки\"=" + buy_price
                + " and \"Цена продажи\"=" + selling_price
                + " and \"id_product_type\"=" + departament_id
                ;

            if (db.isNoteExists(query))
            {
                MessageBox.Show("Данная запись уже присутствует!\r\n" +
                    "Измените её или создайте новую!\r\n" +
                    "Возможно выбранная ячейка хранения уже занята!");
                return;
            }

            if (db.WriteToProducts(name_product, provider, unit
                , buy_price, selling_price
                , coming_product, remainder_product, departament_id))
            {
                Form1 main = this.Owner as Form1;
                main.is_update_products = true;
                MessageBox.Show("Данные успешно добавлены!");
                Close();
                return;
            }
            

            MessageBox.Show("Не удалось добавить!\r\nПовторите ввод!");
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
    }
}
