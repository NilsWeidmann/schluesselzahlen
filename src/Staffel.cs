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
    public partial class Staffel : Form
    {
        Dateninput caller;
        int min = 6;
        int max = 14;
        Liga liga;
        bool neu;

        public Staffel(Liga l, bool neu, Dateninput caller)
        {
            InitializeComponent();
            this.neu = neu;
            this.liga = l;
            this.caller = caller;

            textBox1.Text = l.name;
            if (!neu)
                if (min < l.anzahl_teams + l.anzahl_teams % 2)
                    min = l.anzahl_teams + l.anzahl_teams % 2;
            for (int i = min; i <= max; i += 2)
                comboBox1.Items.Add(i);
            if (neu)
                comboBox1.SelectedIndex = 0;
            else
                for (int i = 0; i < comboBox1.Items.Count; i++)
                    if (Data.toInt(comboBox1.Items[i].ToString()) == l.feld)
                        comboBox1.SelectedIndex = i;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
            caller.Enabled = true;
            caller.Focus();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            liga.name = Data.clear(textBox1.Text);
            liga.feld = Data.toInt(comboBox1.SelectedItem.ToString());
            if (neu)
            {
                liga.team = new Team[max];
                liga.index = caller.liga.Count;
                caller.liga.Add(liga);
                caller.comboBox1.Items.Add(liga.name);
                caller.comboBox1.SelectedIndex = liga.index;
            }
            else
                caller.comboBox1.Items[caller.comboBox1.SelectedIndex] = liga.name;
            this.Close();
            caller.enableGUIElements();
            caller.Enabled = true;
            caller.Focus();
        }

        private void Staffel_FormClosed(object sender, FormClosedEventArgs e)
        {
            caller.Enabled = true;
            caller.Focus();
        }

        private void Staffel_Resize(object sender, EventArgs e)
        {
            caller.WindowState = this.WindowState;
            this.Focus();
        }
    }
}
