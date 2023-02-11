using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace grocery_store
{
    public partial class Form1 : Form
    {
        DBWrapper db;
        /////////////////////////////////////////////////////////////////

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            db = new DBWrapper();
            if (!db.isOpen())
            {
                MessageBox.Show("Ошибка загрузки базы данных!");
                Application.Exit();
            }

            dataGridView1.DataSource = null;
            dataGridView2.DataSource = null;

            SecondaryMethods.fillDataGrid(dataGridView1, db.selectProductTypes());

            int index = int.Parse(dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString());
            DataTable dt_product = db.selectProducts(index);
            SecondaryMethods.fillDataGrid(dataGridView2, dt_product);
            SecondaryMethods.fillComboBox(comboBox1, dt_product);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            db.Close();
        }

        private void dataGridView1_KeyUp(object sender, KeyEventArgs e)
        {
            dataGridView1.Rows[dataGridView1.CurrentRow.Index].Selected = true;
            if (e.KeyCode != Keys.Up && e.KeyCode != Keys.Down && e.KeyCode != Keys.Enter) return;

            int index = int.Parse(dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString());
            SecondaryMethods.fillDataGrid(dataGridView2, db.selectProducts(index));
        }

        private void dataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            dataGridView1.Rows[dataGridView1.CurrentRow.Index].Selected = true;
        }

        private void dataGridView1_Sorted(object sender, EventArgs e)
        {
            int index = int.Parse(dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString());
            SecondaryMethods.fillDataGrid(dataGridView2, db.selectProducts(index));
        }

        private void dataGridView2_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            dataGridView2.Rows[dataGridView2.CurrentRow.Index].Selected = true;
        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0) return;
            dataGridView1.Rows[e.RowIndex].Selected = true;
            int index = int.Parse(dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString());
            SecondaryMethods.fillDataGrid(dataGridView2, db.selectProducts(index));
        }

        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            int index = int.Parse(dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form2 new_form = new Form2();
            new_form.Owner = this;
            new_form.ShowDialog();
            if (is_update_product_types)
            {
                is_update_product_types = false;
                SecondaryMethods.fillDataGrid(dataGridView1, db.selectProductTypes());
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form3 new_form = new Form3();
            new_form.Owner = this;
            new_form.ShowDialog();
            if (is_update_products)
            {
                is_update_products = false;
                int index = int.Parse(dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString());

                DataTable dt_product = db.selectProducts(index);
                SecondaryMethods.fillDataGrid(dataGridView2, dt_product);
            }
        }

        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            ContextMenuStrip grid1_menu = new System.Windows.Forms.ContextMenuStrip();
            int index = dataGridView1.HitTest(e.X, e.Y).RowIndex;
            if (index >= 0)
            {
                grid1_menu.Items.Add("Удалить:\r\nотдел: \"" + dataGridView1[1, index].Value+"\"\r\nid: "+ dataGridView1[0, index].Value);
                grid1_menu.Items.Add("Добавить новый отдел");
                grid1_menu.Items.Add("Добавить товар");
                grid1_menu.Show(dataGridView1, new Point(e.X, e.Y));
                grid1_menu.ItemClicked += new ToolStripItemClickedEventHandler(grid1_menu_Item_Clicked);
            }
        }

        void grid1_menu_Item_Clicked(object sender, ToolStripItemClickedEventArgs e)
        {
            string str = e.ClickedItem.Text;
            if (str.StartsWith("Удалить"))
            {
                string[] elems = str.Split(new char[] { '\n', '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (elems.Count() == 0) return;
                string last = elems.Last<string>();
                int index = int.TryParse(last, out _) ? int.Parse(last) : 0;
                if (index == 0) return;
                if(!db.deleteProductType(index))
                {
                    MessageBox.Show("Ошибка удаления отдела!\r\nПовторите запрос!");
                    return;
                }
                SecondaryMethods.fillDataGrid(dataGridView1, db.selectProductTypes());
                int id_product_type = int.Parse(dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString());
                SecondaryMethods.fillDataGrid(dataGridView2, db.selectProducts(id_product_type));
            }
            else if (str.StartsWith("Добавить новый"))
            {
                button1_Click(sender, e);
            }
            else if (str.StartsWith("Добавить товар"))
            {
                button2_Click(sender, e);
            }
        }

        private void dataGridView2_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            ContextMenuStrip grid2_menu = new System.Windows.Forms.ContextMenuStrip();
            int index = dataGridView2.HitTest(e.X, e.Y).RowIndex;
            if (index >= 0)
            {
                grid2_menu.Items.Add("Удалить:\r\nотдел: \"" + dataGridView2[1, index].Value + "\"\r\nid: " + dataGridView2[0, index].Value);
                grid2_menu.Show(dataGridView2, new Point(e.X, e.Y));
                grid2_menu.ItemClicked += new ToolStripItemClickedEventHandler(grid2_menu_Item_Clicked);
            }
        }

        void grid2_menu_Item_Clicked(object sender, ToolStripItemClickedEventArgs e)
        {
            string str = e.ClickedItem.Text;
            if (str.StartsWith("Удалить"))
            {
                string[] elems = str.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (elems.Count() == 0) return;
                string last = elems.Last<string>();
                int index = int.TryParse(last, out _) ? int.Parse(last) : 0;
                if (index == 0) return;

                if (!db.deleteProduct(index))
                {
                    MessageBox.Show("Ошибка удаления продукта!\r\nПовторите запрос!");
                    return;
                }
                int id_product_type = int.Parse(dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString());
                SecondaryMethods.fillDataGrid(dataGridView2, db.selectProducts(id_product_type));
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(textBox1.Text == String.Empty && textBox2.Text == String.Empty) return;

            DataTable dt_find = null;
            string field_name_find = comboBox1.Items[comboBox1.SelectedIndex].ToString();

            if (textBox2.Text != String.Empty)
            {
                string str_find = textBox2.Text.Replace("\"", "\\\"");
                int id_product_type = int.Parse(dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString());
                dt_find = db.selectProductsEquals(id_product_type, field_name_find, str_find);
            }
            if((dt_find != null && dt_find.Rows.Count == 0) && textBox1.Text != String.Empty 
                || dt_find == null && textBox1.Text != String.Empty)
            {
                string str_find = textBox1.Text.Replace("\"", "\\\"");
                int id_product_type = int.Parse(dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString());
                dt_find = db.selectProductsContains(id_product_type, field_name_find, str_find);
            }
            SecondaryMethods.fillDataGrid(dataGridView2, dt_find);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            int id_product_type = int.Parse(dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString());
            SecondaryMethods.fillDataGrid(dataGridView2, db.selectProducts(id_product_type));
        }
    }


}
