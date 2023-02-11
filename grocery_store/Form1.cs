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
            ContextMenuStrip my_menu = new System.Windows.Forms.ContextMenuStrip();
            int index = dataGridView1.HitTest(e.X, e.Y).RowIndex;
            if (index >= 0)
            {
                my_menu.Items.Add("Удалить:\r\nотдел: \"" + dataGridView1[1, index].Value+"\"\r\nid: "+ dataGridView1[0, index].Value);
                my_menu.Items.Add("Добавить новый отдел");
                my_menu.Items.Add("Добавить товар");
                my_menu.Show(dataGridView1, new Point(e.X, e.Y));
                my_menu.ItemClicked += new ToolStripItemClickedEventHandler(my_menu_Item_Clicked);
            }
        }

        void my_menu_Item_Clicked(object sender, ToolStripItemClickedEventArgs e)
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
    }


}
