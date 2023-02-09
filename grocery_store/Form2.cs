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

        private void button1_Click(object sender, EventArgs e)
        {
            if(textBox1.Text == String.Empty 
                || textBox2.Text == String.Empty 
                || textBox3.Text == String.Empty) {
                MessageBox.Show("Все поля должн быть заполнены!");
                return;
            }

            string name = textBox1.Text;
            name.Replace("\"", "\"\"");

            int row = int.Parse(textBox2.Text);
            int col = int.Parse(textBox3.Text);

            string query = "select id from product_types where \"Наименование отдела\" = \""+name
                +"\" or \"Номер ряда хранения\"="+row
                +" and \"Номер секции хранения\"="+col;
            if (db.isNoteExists(query))
            {
                MessageBox.Show("Данная запись уже присутствует!\r\n" +
                    "Измените её или создайте новую!\r\n" +
                    "Возможно выбранная ячейка хранения уже занята!");
                return;
            }

            if (db.WriteToProductTypes(name, row, col))
            {
                Form1 main = this.Owner as Form1;
                main.is_update_product_types= true;
                MessageBox.Show("Данные успешно добавлены!");
                Close();
                return;
            }

            MessageBox.Show("Не удалось добавить!\r\nПовторите ввод!");

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
    }
}
