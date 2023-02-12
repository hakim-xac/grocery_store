﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace grocery_store
{
    internal class SecondaryMethods
    {
        public static void fillDataGrid(DataGridView dg, DataTable dt, bool is_modified = false)
        {
            dg.DataSource = dt;
            dg.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dg.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dg.Dock = DockStyle.Fill;
            dg.AllowUserToAddRows = false;
            dg.AllowUserToDeleteRows = false;
            dg.AllowUserToResizeColumns = false;
            dg.MultiSelect = false;
            dg.ReadOnly = !is_modified;
        }

        public static void fillComboBox(System.Windows.Forms.ComboBox cb, DataTable dt)
        {
            cb.Items.Clear();
            for (int i = 1, ie = dt.Columns.Count - 1; i < ie; ++i) cb.Items.Add(dt.Columns[i].ColumnName);
            if (cb.Items.Count > 0 ) cb.SelectedIndex = 0;
        }
        public static void fillComboBox(System.Windows.Forms.ComboBox cb, Hashtable ht)
        {
            cb.Items.Clear();
            foreach (DictionaryEntry elem in ht) cb.Items.Add(elem.Key);

            if (cb.Items.Count > 0) cb.SelectedIndex = 0;
        }

        public static bool fillHashTableFromBD(Hashtable ht, DataTable dt)
        {
            foreach (DataRow row in dt.Rows) ht.Add(row[0].ToString(), row[1].ToString());
            return ht.Count == dt.Rows.Count;
        }

        public static string noSQLInjection(string str)
        {
            return str.Replace("\"", "").Replace("\'", "").Replace(";", "").Replace("%", "");
        }
    }
}
