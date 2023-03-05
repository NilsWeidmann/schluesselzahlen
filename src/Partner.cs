using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Schluesselzahlen
{
    public partial class Partner : Form
    {
        Schluesselzahlen caller;
        Verein v;
        Partnerschaft p_akt;

        public Partner(Schluesselzahlen caller, Verein v)
        {
            InitializeComponent();
            this.caller = caller;
            this.v = v;
            dataGridView1.Columns.Clear();
            dataGridView1.Columns.Add("Verein A", "Verein A");
            dataGridView1.Columns[0].ReadOnly = true;
            DataGridViewColumn dgvc = new DataGridViewComboBoxColumn();
            dgvc.HeaderText = "Woche";
            dataGridView1.Columns.Add(dgvc);
            dataGridView1.Columns.Add(" ", " ");
            dataGridView1.Columns[2].ReadOnly = true;
            dgvc = new DataGridViewComboBoxColumn();
            dgvc.HeaderText = "Verein B";
            dataGridView1.Columns.Add(dgvc);
            dgvc = new DataGridViewComboBoxColumn();
            dgvc.HeaderText = "Woche";
            dataGridView1.Columns.Add(dgvc);

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            dataGridView1.Rows.Clear();
            foreach (Partnerschaft p in Data.partnerschaft)
                if (p.a.index == v.index || p.b.index == v.index)
                {
                    p_akt = p;
                    dataGridView1.Rows.Add();
                }
        }

        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            char woche;

            if (dataGridView1.Rows[e.RowIndex].Cells.Count < 5)
                return;

            // Aktueller Verein
            dataGridView1.Rows[e.RowIndex].Cells[0].Value = v.name;

            // Woche aktueller Verein
            DataGridViewComboBoxCell dgvcbc = new DataGridViewComboBoxCell();
            dgvcbc.FlatStyle = FlatStyle.Flat;
            dgvcbc.Items.Add("A");
            dgvcbc.Items.Add("B");
            dgvcbc.Items.Add("X");
            dgvcbc.Items.Add("Y");
            if (!dataGridView1.Rows[e.RowIndex].IsNewRow)
            {
                if (p_akt.a.index == v.index)
                    woche = p_akt.woche_a;
                else
                    woche = p_akt.woche_b;
                switch (woche)
                {
                    case 'A': dgvcbc.Value = dgvcbc.Items[0]; break;
                    case 'B': dgvcbc.Value = dgvcbc.Items[1]; break;
                    case 'X': dgvcbc.Value = dgvcbc.Items[2]; break;
                    case 'Y': dgvcbc.Value = dgvcbc.Items[3]; break;
                }
            }
            dataGridView1.Rows[e.RowIndex].Cells[1] = dgvcbc;

            // Gleichheitszeichen
            dataGridView1.Rows[e.RowIndex].Cells[2].Value = "=";

            // Partnerverein
            dgvcbc = new DataGridViewComboBoxCell();
            dgvcbc.FlatStyle = FlatStyle.Flat;
            for (int i = 0; i < Data.verein.Length; i++)
                dgvcbc.Items.Add(Data.verein[i].name);
            if (!dataGridView1.Rows[e.RowIndex].IsNewRow)
                if (p_akt.a.index == v.index)
                    dgvcbc.Value = dgvcbc.Items[p_akt.b.index];
                else
                    dgvcbc.Value = dgvcbc.Items[p_akt.a.index];
            dataGridView1.Rows[e.RowIndex].Cells[3] = dgvcbc;

            // Woche Partnerverein
            dgvcbc = new DataGridViewComboBoxCell();
            dgvcbc.FlatStyle = FlatStyle.Flat;
            dgvcbc.Items.Add("A");
            dgvcbc.Items.Add("B");
            dgvcbc.Items.Add("X");
            dgvcbc.Items.Add("Y");
            if (!dataGridView1.Rows[e.RowIndex].IsNewRow)
            {
                if (p_akt.a.index == v.index)
                    woche = p_akt.woche_b;
                else
                    woche = p_akt.woche_a;
                switch (woche)
                {
                    case 'A': dgvcbc.Value = dgvcbc.Items[0]; break;
                    case 'B': dgvcbc.Value = dgvcbc.Items[1]; break;
                    case 'X': dgvcbc.Value = dgvcbc.Items[2]; break;
                    case 'Y': dgvcbc.Value = dgvcbc.Items[3]; break;
                }
            }
            dataGridView1.Rows[e.RowIndex].Cells[4] = dgvcbc;

            // AutoResizeColumns
            dataGridView1.AutoResizeColumns();
        }

        private void Partner_FormClosing(object sender, FormClosingEventArgs e)
        {
            switch (MessageBox.Show("Wollen Sie die Änderungen speichern?", "Änderungen speichern", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
            {
                case DialogResult.Yes:
                    this.Visible = false;
                    List<Partnerschaft> p_remove = new List<Partnerschaft>();
                    foreach (Partnerschaft p in Data.partnerschaft)
                        if (p.a.index == v.index || p.b.index == v.index)
                            p_remove.Add(p);
                    foreach (Partnerschaft p in p_remove)
                        Data.partnerschaft.Remove(p);
                    for (int i = 0; i < dataGridView1.Rows.Count; i++)
                        if (dataGridView1.Rows[i].Cells[1].Value != null && dataGridView1.Rows[i].Cells[3].Value != null && dataGridView1.Rows[i].Cells[4].Value != null)
                            Data.partnerschaft.Add(new Partnerschaft(v, (string)dataGridView1.Rows[i].Cells[1].Value, (string)dataGridView1.Rows[i].Cells[3].Value,
                                                  (string)dataGridView1.Rows[i].Cells[4].Value, Data.verein));
                            
                    caller.Enabled = true;
                    caller.Focus();
                    break;
                case DialogResult.No:
                    this.Visible = false;
                    caller.Enabled = true;
                    caller.Focus();
                    break;
                case DialogResult.Cancel:
                    e.Cancel = true;
                    break;
                case DialogResult.Abort:
                    e.Cancel = true;
                    break;
            }
        }

        private void Partner_Resize(object sender, EventArgs e)
        {
            caller.WindowState = this.WindowState;
            this.Focus();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            for (int i=0; i<dataGridView1.Rows.Count; i++)
                if (dataGridView1.Rows[i].Cells[0].Value.Equals(dataGridView1.Rows[i].Cells[3].Value))
                {
                    dataGridView1.Rows[i].Cells[3].Value = null;
                    MessageBox.Show("Partnerschaften mit sich selbst sind unzulässig!");
                    return;
                }
        }
    }
}
