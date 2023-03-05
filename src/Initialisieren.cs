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
    public partial class Initialisieren : Form
    {
        public Schluesselzahlen caller;

        public Initialisieren(Schluesselzahlen caller)
        {
            InitializeComponent();
            this.caller = caller;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            try
            {
                caller.loadFromFile(Data.vereine_b, Data.staffeln_b, Data.beziehungen_b);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Es ist kein Backup vorhanden!");
                caller.loadFromFile(Data.vereine, Data.staffeln, Data.beziehungen);
            }
            returnToCaller();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            returnToCaller();
        }

        private void Initialisieren_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Enabled = false;
            returnToCaller();
        }

        private void loescheVereine()
        {
            List<Verein> lv = Data.verein.ToList();
            List<Verein> lv_d = new List<Verein>();
            foreach (Verein v in lv)
                if (v.team == null || v.team.Length == 0)
                    lv_d.Add(v);
            foreach (Verein v in lv_d)
                lv.Remove(v);
            Data.verein = lv.ToArray();
            caller.comboBox3.Items.Clear();
            for (int i = 0; i < Data.verein.Length; i++)
                caller.comboBox3.Items.Add(Data.verein[i].name);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            loescheVereine();
            returnToCaller();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            for (int i = 0; i < Data.liga.Length; i++)
                for (int j = 0; j < Data.liga[i].anzahl_teams; j++)
                    Data.liga[i].team[j].woche = '-';
            returnToCaller();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            for (int i = 0; i < Data.liga.Length; i++)
                for (int j = 0; j < Data.liga[i].anzahl_teams; j++)
                    Data.liga[i].team[j].zahl = 0;
            returnToCaller();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            this.Enabled = false;
            for (int i = 0; i < Data.verein.Length; i++)
                Data.verein[i].a = Data.verein[i].b = Data.verein[i].x = Data.verein[i].y = 0;
            returnToCaller();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            for (int i = 0; i < Data.liga.Length; i++)
                for (int j = 0; j < Data.liga[i].anzahl_teams; j++)
                    for (int k = 0; k < Data.team_max; k++)
                        Data.liga[i].team[j].spieltag[k] = '-';
            returnToCaller();
        }

        private void returnToCaller()
        {
            this.Visible = false;
            caller.comboBox1.Text = "";
            caller.comboBox1.SelectedIndex = -1;
            caller.comboBox2.Text = "";
            caller.comboBox2.SelectedIndex = -1;
            caller.comboBox3.Text = "";
            caller.comboBox3.SelectedIndex = -1;
            caller.comboBox12.Text = "";
            caller.dataGridView1.Columns.Clear();
            caller.clearGrid();
            caller.Enabled = true;
            caller.Focus();
        }

        private void Initialisieren_Resize(object sender, EventArgs e)
        {
            caller.WindowState = this.WindowState;
            this.Focus();
        }
    }
}
