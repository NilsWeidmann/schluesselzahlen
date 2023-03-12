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
    public partial class Alternativen : Form
    {
        public Konflikt[] k;
        public Liga[] l;
        public Verein[] v;
        public int i;
        public Schluesselzahlen caller;

        public Alternativen(Konflikt[] k, Liga[] l, Verein[] v, Schluesselzahlen caller)
        {
            InitializeComponent();
            this.k = k;
            this.l = l;
            this.v = v;
            this.caller = caller;
            comboBox1.Items.Clear();
            for (int j = 0; j < k.Length; j++)
                comboBox1.Items.Add("Konflikt " + (j + 1));
            comboBox1.SelectedIndex = 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Liga[] best_l = new Liga[l.Length];
            Verein[] best_v = new Verein[v.Length];
            int[] konflikte = new int[2];
            konflikte[0] = -1;
            konflikte[1] = -1;
            int[] schluessel = new int[Data.verein.Length * 2];
            for (int i=0; i<k.Length; i++)
                for (int j=0; j<k[i].t.Length; j++)
                    for (int x=0; x<k.Length; x++)
                        for (int y=0; y<k[x].t.Length; y++)
                            if (k[i].t[j].liga == k[x].t[y].liga && k[i].t[j].zahl == k[x].t[y].zahl)
                                if (i != x)
                                {
                                    MessageBox.Show("Vergeben Sie die Zahl " + k[i].t[j].zahl + " in der " + k[i].t[j].liga.name + " nur einmal!");
                                    return;
                                }
                                else if (j != y)
                                {

                                    MessageBox.Show("Lösen Sie zunächst Konflikt " + (i + 1) + "!");
                                    return;
                                }
            this.Visible = false;
            Data.createPriority();
            Data.copy(l, best_l, v, best_v, Data.partnerschaft, Data.partnerschaft);
            Data.findSolution(0, l, best_l, v, best_v, konflikte, schluessel);
            Data.copy(best_l, l, best_v, v, Data.partnerschaft, Data.partnerschaft);
            caller.solveConflicts(l, v);
            caller.Enabled = true;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            i = comboBox1.SelectedIndex;
            textBox1.Text = k[i].t[0].name;
            textBox2.Text = k[i].t[1].name;
            if (k[i].t.Length == 3)
                textBox3.Text = k[i].t[2].name;
            else
                textBox3.Text = "";
            textBox7.Text = k[i].t[0].liga.name;

            ComboBox[] combo = { comboBox2, comboBox3, comboBox4 };
            TextBox[] text = { textBox4, textBox5, textBox6 };

            for (int box = 0; box < 3; box++)
            {
                if (box < k[i].t.Length)
                {
                    combo[box].Items.Clear();
                    combo[box].Text = k[i].t[box].zahl.ToString();
                    text[box].Text = k[i].wunsch.ToString();
                    for (int j = 0; j < 3; j++)
                        if (k[i].zahl[j] != 0)
                        {
                            combo[box].Items.Add(k[i].zahl[j]);
                            if (k[i].t[box].zahl == k[i].zahl[j])
                                combo[box].SelectedIndex = j;
                        }
                    combo[box].Enabled = true;
                }
                else
                {
                    combo[box].Items.Clear();
                    combo[box].Text = "";
                    text[box].Text = "";
                    combo[box].SelectedIndex = -1;
                    combo[box].Enabled = false;
                }
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            resetSelectedIndices((ComboBox)sender);
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            resetSelectedIndices((ComboBox)sender);
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            resetSelectedIndices((ComboBox)sender);
        }

        private void resetSelectedIndices(ComboBox sender)
        {
            ComboBox[] combo = { comboBox2, comboBox3, comboBox4 };

            for (int box = 0; box < 3; box++)
            {
                if (combo[box] != sender && sender.SelectedIndex == combo[box].SelectedIndex)
                    combo[box].SelectedIndex = -1;
                if (combo[box] == sender && combo[box].SelectedIndex != -1)
                    k[i].t[box].zahl = Data.toInt(combo[box].SelectedItem.ToString());
            }
        }

        private void Alternativen_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Enabled = false;
            switch (MessageBox.Show("Wollen Sie die Änderungen speichern?", "Änderungen speichern", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
            {
                case DialogResult.No:
                    try
                    {
                        caller.loadFromFile(Data.vereine_b, Data.staffeln_b, Data.beziehungen_b);
                    }
                    catch (Exception ex)
                    {
                        caller.loadFromFile(Data.vereine, Data.staffeln, Data.beziehungen);
                    }
                    prepareCaller();
                    break;
                case DialogResult.Yes:
                    Data.speichern(Data.liga, Data.verein, Data.partnerschaft, Data.vereine, Data.staffeln, Data.beziehungen);
                    caller.loadFromFile(Data.vereine, Data.staffeln, Data.beziehungen);
                    prepareCaller();
                    break;
                case DialogResult.Cancel:
                    e.Cancel = true;
                    break;
            }
            this.Enabled = true;
        }

        private void prepareCaller()
        {
            caller.comboBox1.Text = "";
            caller.comboBox1.SelectedIndex = -1;
            caller.comboBox2.Text = "";
            caller.comboBox2.SelectedIndex = -1;
            caller.comboBox3.Text = "";
            caller.comboBox12.Text = "";
            caller.dataGridView1.Columns.Clear();
            caller.Enabled = true;
            caller.Focus();
        }

        private void Alternativen_Resize(object sender, EventArgs e)
        {
            caller.WindowState = this.WindowState;
            this.Focus();
        }
    }
}
