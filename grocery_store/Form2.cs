using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;



namespace grocery_store
{
    public partial class Form2 : Form
    {

        DBWrapper db;


        public Form2()
        {
            InitializeComponent();
        }

        private bool addToProductTypes(string name, int row, int col)
        {
            return !db.isExistsProductType(name, row, col) && db.WriteToProductTypes(name, row, col);
        }

        private bool updateToProductTypes(string name, int row, int col, int id)
        {
            return db.isExistsProductType(name, row, col) && db.updateProductTypes(name, row, col, id);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(textBox1.Text == String.Empty 
                || textBox2.Text == String.Empty 
                || textBox3.Text == String.Empty) {
                MessageBox.Show("Все поля должн быть заполнены!");
                return;
            }

            string name = textBox1.Text;
            name.Replace("\"", "");
            name.Replace("\'", "");

            int row = int.Parse(textBox2.Text);
            int col = int.Parse(textBox3.Text);

            Form1 main = this.Owner as Form1;
            if (main.id_product_types == 0) {
                if (!addToProductTypes(name, row, col))
                {
                    MessageBox.Show("Не удалось добавить!\r\nПовторите ввод!");
                    return;
                }
                main.is_update_product_types = true;
                MessageBox.Show("Данные успешно добавлены!");
            }
            else
            {
                if (!updateToProductTypes(name, row, col, main.id_product_types))
                {
                    MessageBox.Show("Не удалось обновить!\r\nПовторите ввод!");
                    return;
                }
                main.is_update_product_types = true;
                MessageBox.Show("Данные успешно обновлены!");
            }

            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Form2_Load(object sender, EventArgs e)
        {            
            db = new DBWrapper();
            if (!db.isOpen())
            {
                MessageBox.Show("Ошибка загрузки базы данных!");
                Application.Exit();
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (!int.TryParse(textBox2.Text, out int _)) textBox2.Clear();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (!int.TryParse(textBox3.Text, out int _)) textBox3.Clear();
        }

        private void Form2_Shown(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            Form1 form1 = this.Owner as Form1;
            if (form1.id_product_types > 0)
            {
                int id = form1.id_product_types;
                DataTable dt = db.selectFieldProductTypes("id", id.ToString());
                textBox1.Text = dt.Rows[0][1].ToString();
                textBox2.Text = dt.Rows[0][2].ToString();
                textBox3.Text = dt.Rows[0][3].ToString();
                button1.Text = "Изменить";
                return;
            }

            button1.Text = "Добавить";
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            Form1 form1 = this.Owner as Form1;
            if(form1.id_product_types > 0) form1.id_product_types = 0;
        }
    }
}
