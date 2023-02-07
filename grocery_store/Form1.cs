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

        private void fillDataGrid(DataGridView dg, DataTable dt)
        {
            dg.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dg.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dg.Dock = DockStyle.Fill;
            dg.ReadOnly = true;
            dg.AllowUserToAddRows = false;
            dg.AllowUserToDeleteRows = false;
            dg.AllowUserToResizeColumns = false;
            dg.MultiSelect= false;
            dg.DataSource = dt;
        }

        private void fillComboBox(System.Windows.Forms.ComboBox cb, DataTable dt)
        {
            cb.Items.Clear();
            for (int i=1, ie=dt.Columns.Count-1; i < ie; ++i) cb.Items.Add(dt.Columns[i].ColumnName);
            cb.SelectedIndex = 0;
        }

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
            DataTable dt_product_types = db.selectProductTypes();

            fillDataGrid(dataGridView1, dt_product_types);

            int index = int.Parse(dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString());
            DataTable dt_product = db.selectProducts(index);
            fillDataGrid(dataGridView2, dt_product);
            fillComboBox(comboBox1, dt_product);
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
            fillDataGrid(dataGridView2, db.selectProducts(index));
        }

        private void dataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            dataGridView1.Rows[dataGridView1.CurrentRow.Index].Selected = true;
        }

        private void dataGridView1_Sorted(object sender, EventArgs e)
        {
            int index = int.Parse(dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString());
            fillDataGrid(dataGridView2, db.selectProducts(index));
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
            fillDataGrid(dataGridView2, db.selectProducts(index));
        }

        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            int index = int.Parse(dataGridView1[0, dataGridView1.CurrentRow.Index].Value.ToString());
        }

    }


}
