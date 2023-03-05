using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Threading;

namespace Schluesselzahlen
{
    public partial class Schluesselzahlen : Form
    {
        public Verein verein;

        public Schluesselzahlen()
        {
            InitializeComponent();
            this.dataGridView1.ReadOnly = false;
            enableFields();
            checkBox1.Enabled = false;
            comboBox12.Enabled = false;
            textBox2.Enabled = false;
            button3.Enabled = false;
            button6.Enabled = false;
            button14.Enabled = false;
            button15.Enabled = false;
            comboBox1.Enabled = false;
            comboBox2.Enabled = false;
            comboBox3.Enabled = false;
            textBox4.Enabled = false;
            comboBox4.Items.Add("-");
            comboBox4.Items.Add("A");
            comboBox4.Items.Add("B");
            comboBox4.Items.Add("X");
            comboBox4.Items.Add("Y");
            comboBox4.SelectedIndex = 0;
            fillFields(comboBox10);
            comboBox10.SelectedIndex = 3;
            fillFields(comboBox11);
            comboBox11.SelectedIndex = 2;
            textBox1.Text = "2";
            textBox3.Text = "0";
            textBox2.Text = Application.StartupPath;
            setFiles(Application.StartupPath);
            Data.caller = this;
            clearGrid();
        }

        public void fillFields(ComboBox cb)
        {
            cb.Items.Clear();
            cb.Items.Add("6");
            cb.Items.Add("8");
            cb.Items.Add("10");
            cb.Items.Add("12");
            cb.Items.Add("14");
            cb.SelectedIndex = -1;
        }

        public void enableFields()
        {
            textBox4.Enabled = false;
            comboBox12.Enabled = comboBox2.SelectedIndex != -1;
        }

        public void showResults(Liga[] best_l, Verein[] best_v, bool cancelled)
        {
            if (Data.meldung.Count > 0)
            {
                MessageBox.Show(Data.meldung[0]);
                Data.meldung.Clear();
                loadFromFile(Data.vereine_b, Data.staffeln_b, Data.beziehungen_b);
            }
            else if (cancelled)
                loadFromFile(Data.vereine_b, Data.staffeln_b, Data.beziehungen_b);
            else
            {
                Data.copy(best_l, Data.liga, best_v, Data.verein, Data.partnerschaft, Data.partnerschaft);
                solveConflicts(Data.liga, Data.verein);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            Data.laufzeit = Data.toInt(textBox1.Text) * 60 + Data.toInt(textBox3.Text);
            Data.ht.Clear();
            BitteWarten bw = new BitteWarten(this);
            bw.Visible = true;
        }

        public void initUI()
        {
            dataGridView1.Rows.Clear();
            checkBox1.Enabled = false;
            comboBox1.Items.Clear();
            comboBox1.SelectedIndex = -1;
            comboBox1.Text = "";
            comboBox1.Items.Clear();
            for (int i = 0; i < Data.liga.Length; i++)
                comboBox1.Items.Add(Data.liga[i].name);
            comboBox2.Items.Clear();
            comboBox2.SelectedIndex = -1;
            comboBox2.Text = "";
            comboBox3.Items.Clear();
            comboBox3.SelectedIndex = -1;
            comboBox3.Text = "";
            comboBox3.Items.Clear();
            for (int i = 0; i < Data.verein.Length; i++)
                comboBox3.Items.Add(Data.verein[i].name);
            enableFields();
        }

        public bool loadFromFile(Textdatei ver, Textdatei sta, Textdatei bez)
        {
            button6.Enabled = false;
            Data.verein = Data.getVereine(ver, Data.partnerschaft);
            Data.liga = Data.getStaffeln(Data.verein, sta);
            Data.getBeziehungen(Data.liga, bez);
            Data.allocateTeams(Data.verein, Data.liga);
            Data.fillArray();
            Data.buildArray();
            Data.getSpielplan();
            if (Data.meldung.Count > 0)
            {
                button3.Enabled = false;
                button14.Enabled = false;
                button15.Enabled = false;
                MessageBox.Show(Data.meldung[0]);
                Data.meldung.Clear();
                button6.Enabled = true;
                return false;
            }
            else
            {
                initUI();
                button3.Enabled = true;
                button14.Enabled = true;
                button15.Enabled = true;
                radioButton1.Enabled = true;
                radioButton2.Enabled = true;
                radioButton1.Checked = true;
                comboBox1.Enabled = comboBox2.Enabled = comboBox3.Enabled = true;
                button6.Enabled = true;
                return true;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            loadFromFile(Data.vereine, Data.staffeln, Data.beziehungen);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox2.Items.Clear();
            comboBox2.SelectedIndex = -1;
            comboBox2.Text = "";
            comboBox12.Items.Clear();
            comboBox12.SelectedIndex = -1;
            comboBox12.Text = "";
            if (!(comboBox1.SelectedIndex == -1))
                for (int i = 0; i < Data.liga[comboBox1.SelectedIndex].anzahl_teams; i++)
                    comboBox2.Items.Add(Data.liga[comboBox1.SelectedIndex].team[i].name);
            enableFields();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox12.Items.Clear();
            comboBox12.Items.Add("-");
            for (int i = 0; i < Data.liga[comboBox1.SelectedIndex].feld; i++)
                comboBox12.Items.Add(i + 1);
            if (comboBox2.SelectedIndex != -1)
                comboBox12.SelectedIndex = Data.liga[comboBox1.SelectedIndex].team[comboBox2.SelectedIndex].zahl;
            else
                comboBox12.SelectedIndex = -1;
            enableFields();
        }

        public void clearGrid()
        {
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Add("Liga", "Liga");
            dataGridView1.Columns.Add("Team", "Team");
            dataGridView1.Columns.Add("Schlüssel", "Schlüssel");
            dataGridView1.Columns[0].ReadOnly = true;
            dataGridView1.Columns[1].ReadOnly = true;
            dataGridView1.Columns[2].ReadOnly = true;
            comboBox3.Enabled = radioButton1.Checked;
            checkBox1.Enabled = comboBox4.Enabled = comboBox5.Enabled = comboBox6.Enabled = 
            comboBox7.Enabled = comboBox8.Enabled = radioButton1.Checked && comboBox3.SelectedIndex != -1;
            if (!radioButton1.Checked || comboBox3.SelectedIndex == -1)
                comboBox4.Text = comboBox5.Text = comboBox6.Text = comboBox7.Text = comboBox8.Text = "";
            button4.Enabled = false;
            if (radioButton1.Checked)
            {
                if (comboBox3.SelectedItem != null)
                {
                    comboBox3.Text = comboBox3.SelectedItem.ToString();
                    comboBox4.Text = comboBox4.SelectedItem.ToString();
                    comboBox5.Text = comboBox5.SelectedItem.ToString();
                    comboBox6.Text = comboBox6.SelectedItem.ToString();
                    comboBox7.Text = comboBox7.SelectedItem.ToString();
                    comboBox8.Text = comboBox8.SelectedItem.ToString();
                }
                else
                    comboBox3.Text = comboBox4.Text = comboBox5.Text = comboBox6.Text = comboBox7.Text = comboBox8.Text = "";
                
            }
            else
            {
                comboBox3.Text = comboBox4.Text = comboBox5.Text = comboBox6.Text = comboBox7.Text = comboBox8.Text = "";
                checkBox1.Checked = false;
            }
            foreach (DataGridViewColumn col in dataGridView1.Columns)
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
        }

        public void init()
        {
            int anzahl_teams = 0;
            clearGrid();
            if (radioButton1.Checked)
            {
                if (comboBox3.SelectedIndex == -1)
                {
                    checkBox1.Checked = checkBox1.Enabled = button4.Enabled = false;
                    comboBox4.Text = comboBox5.Text = comboBox6.Text = comboBox7.Text = comboBox8.Text = "";
                }
                else
                {
                    checkBox1.Checked = Data.verein[comboBox3.SelectedIndex].kapazitaet;
                    button4.Enabled = true;
                }
                if (comboBox5.Items.Count > Data.verein[comboBox3.SelectedIndex].a)
                    comboBox5.SelectedIndex = Data.verein[comboBox3.SelectedIndex].a;
                if (comboBox6.Items.Count > Data.verein[comboBox3.SelectedIndex].b)
                    comboBox6.SelectedIndex = Data.verein[comboBox3.SelectedIndex].b;
                if (comboBox7.Items.Count > Data.verein[comboBox3.SelectedIndex].x)
                    comboBox7.SelectedIndex = Data.verein[comboBox3.SelectedIndex].x;
                if (comboBox8.Items.Count > Data.verein[comboBox3.SelectedIndex].y)
                    comboBox8.SelectedIndex = Data.verein[comboBox3.SelectedIndex].y;
                for (int j = 0; j < Data.verein[comboBox3.SelectedIndex].team.Length; j++)
                {
                    String[] inhalt = new String[3];
                    inhalt[0] = Data.verein[comboBox3.SelectedIndex].team[j].liga.name;
                    inhalt[1] = Data.verein[comboBox3.SelectedIndex].team[j].name;
                    if (Data.verein[comboBox3.SelectedIndex].team[j].zahl != 0)
                        inhalt[2] = Data.verein[comboBox3.SelectedIndex].team[j].zahl.ToString();
                    else
                        inhalt[2] = "";
                    dataGridView1.Rows.Add(inhalt);
                    for (int l = 0; l < 3; l++)
                    {
                        switch (Data.verein[comboBox3.SelectedIndex].team[j].woche)
                        {
                            case 'A': dataGridView1.Rows[anzahl_teams].Cells[l].Style.BackColor = Color.Yellow; break;
                            case 'B': dataGridView1.Rows[anzahl_teams].Cells[l].Style.BackColor = Color.Orange; break;
                            case 'X': dataGridView1.Rows[anzahl_teams].Cells[l].Style.BackColor = Color.LightBlue; break;
                            case 'Y': dataGridView1.Rows[anzahl_teams].Cells[l].Style.BackColor = Color.LightGreen; break;
                            case '-': dataGridView1.Rows[anzahl_teams].Cells[l].Style.BackColor = Color.White; break;
                        }
                    }
                    anzahl_teams++;
                }
                verein = Data.verein[comboBox3.SelectedIndex];
            }
            else
            {
                for (int j = 0; j < Data.liga[comboBox1.SelectedIndex].anzahl_teams; j++)
                {
                    String[] inhalt = new String[3];
                    inhalt[0] = Data.liga[comboBox1.SelectedIndex].name;
                    inhalt[1] = Data.liga[comboBox1.SelectedIndex].team[j].name;

                    if (Data.liga[comboBox1.SelectedIndex].team[j].zahl != 0)
                        inhalt[2] = Data.liga[comboBox1.SelectedIndex].team[j].zahl.ToString();
                    else if (Data.liga[comboBox1.SelectedIndex].team[j].woche == 'A' && Data.liga[comboBox1.SelectedIndex].team[j].verein.a != 0)
                        if (Data.liga[comboBox1.SelectedIndex].feld == Data.feld[0])
                            inhalt[2] = Data.liga[comboBox1.SelectedIndex].team[j].verein.a.ToString();
                        else
                            inhalt[2] = Data.parallel_1[Data.feld[0] - 1, Data.liga[comboBox1.SelectedIndex].feld - 1, Data.liga[comboBox1.SelectedIndex].team[j].verein.a - 1].ToString();
                    else if (Data.liga[comboBox1.SelectedIndex].team[j].woche == 'B' && Data.liga[comboBox1.SelectedIndex].team[j].verein.b != 0)
                        if (Data.liga[comboBox1.SelectedIndex].feld == Data.feld[0])
                            inhalt[2] = Data.liga[comboBox1.SelectedIndex].team[j].verein.b.ToString();
                        else
                            inhalt[2] = Data.parallel_1[Data.feld[0] - 1, Data.liga[comboBox1.SelectedIndex].feld - 1, Data.liga[comboBox1.SelectedIndex].team[j].verein.b - 1].ToString();
                    else if (Data.liga[comboBox1.SelectedIndex].team[j].woche == 'X' && Data.liga[comboBox1.SelectedIndex].team[j].verein.x != 0)
                        if (Data.liga[comboBox1.SelectedIndex].feld == Data.feld[1])
                            inhalt[2] = Data.liga[comboBox1.SelectedIndex].team[j].verein.x.ToString();
                        else
                            inhalt[2] = Data.parallel_1[Data.feld[1] - 1, Data.liga[comboBox1.SelectedIndex].feld - 1, Data.liga[comboBox1.SelectedIndex].team[j].verein.x - 1].ToString();
                    else if (Data.liga[comboBox1.SelectedIndex].team[j].woche == 'Y' && Data.liga[comboBox1.SelectedIndex].team[j].verein.y != 0)
                        if (Data.liga[comboBox1.SelectedIndex].feld == Data.feld[1])
                            inhalt[2] = Data.liga[comboBox1.SelectedIndex].team[j].verein.y.ToString();
                        else
                            inhalt[2] = Data.parallel_1[Data.feld[1] - 1, Data.liga[comboBox1.SelectedIndex].feld - 1, Data.liga[comboBox1.SelectedIndex].team[j].verein.y - 1].ToString();
                    else
                        inhalt[2] = "";
                    
                    dataGridView1.Rows.Add(inhalt);
                    if (!inhalt[2].Equals(""))
                        for (int i = 0; i < 3; i++)
                            dataGridView1.Rows[j].Cells[i].Style.BackColor = Color.LightGreen;
                    else if (Data.liga[comboBox1.SelectedIndex].team[j].woche != '-')
                        for (int i = 0; i < 3; i++)
                            dataGridView1.Rows[j].Cells[i].Style.BackColor = Color.Yellow;
                }
                verein = null;
            }
            dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
        }

        private void setFiles(string path)
        {
            Data.gegenlaeufig = new Textdatei(path + @"\Gegenlaeufig.csv");
            Data.parallel = new Textdatei(path + @"\Parallel.csv");
            Data.vereinsintern = new Textdatei(path + @"\Vereinsintern.csv");
            Data.staffeln = new Textdatei(path + @"\Staffeln.csv");
            Data.staffeln_b = new Textdatei(path + @"\Staffeln_Backup.csv");
            Data.vereine = new Textdatei(path + @"\Vereine.csv");
            Data.vereine_b = new Textdatei(path + @"\Vereine_Backup.csv");
            Data.beziehungen = new Textdatei(path + @"\Beziehungen.csv");
            Data.beziehungen_b = new Textdatei(path + @"\Beziehungen_Backup.csv");
            Data.aehnlich = new Textdatei(path + @"\Aehnlich.csv");
            Data.spielplan = new Textdatei(path + @"\Spielplan.csv");
            if (comboBox10.SelectedIndex != -1 && comboBox11.SelectedIndex != -1)
            {
                button6.Enabled = true;
                button2.Enabled = true;
                button1.Enabled = true;
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
                if (comboBox3.SelectedIndex != -1)
                    init();
                else
                    checkBox1.Enabled = checkBox1.Checked = button4.Enabled = false;
            else
                checkBox1.Enabled = checkBox1.Checked = button4.Enabled = false;
        }

        private void button13_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = folderBrowserDialog1.SelectedPath;
                setFiles(textBox2.Text);
            }
        }

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == -1)
            {
                comboBox2.Items.Clear();
                comboBox2.Text = "";
                textBox4.Text = "";
                textBox4.Enabled = false;
            }
            else
            {
                comboBox2.Items.Clear();
                comboBox2.Text = "";
                for (int i = 0; i < Data.liga[comboBox1.SelectedIndex].anzahl_teams; i++)
                    comboBox2.Items.Add(Data.liga[comboBox1.SelectedIndex].team[i].name);
                textBox4.Text = Data.liga[comboBox1.SelectedIndex].feld.ToString();
                textBox4.Enabled = false;
            }
            enableFields();
            if (radioButton2.Checked)
                if (comboBox1.SelectedIndex != -1)
                    init();
        }

        private void button14_Click(object sender, EventArgs e)
        {
            Initialisieren i = new Initialisieren(this);
            this.Enabled = false;
            i.Visible = true;
            i.Focus();
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            bool frei = true;
            int key = Data.toInt(dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString());
            for (int i = 0; i < verein.team[e.RowIndex].liga.anzahl_teams; i++)
                if (verein.team[e.RowIndex].liga.team[i].zahl == key)
                    frei = false;
            if (key > 0 && key <= verein.team[e.RowIndex].liga.anzahl_teams + verein.team[e.RowIndex].liga.anzahl_teams % 2 && frei)
                verein.team[e.RowIndex].zahl = key;
            else
                dataGridView1.Rows[e.RowIndex].Cells[2].Value = "";
        }

        private void button15_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex >= 0)
                Data.liga[comboBox1.SelectedIndex].feld = Data.toInt(textBox4.Text);
            if (comboBox1.SelectedIndex >= 0 && comboBox2.SelectedIndex >= 0)
                Data.liga[comboBox1.SelectedIndex].team[comboBox2.SelectedIndex].zahl = comboBox12.SelectedIndex;
            Data.speichern(Data.liga, Data.verein, Data.partnerschaft, Data.vereine, Data.staffeln, Data.beziehungen);
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox3.SelectedIndex >= 0)
            {
                Data.verein[comboBox3.SelectedIndex].a = comboBox5.SelectedIndex;
                if (comboBox5.SelectedIndex == 0)
                    Data.verein[comboBox3.SelectedIndex].b = 0;
                else
                    Data.verein[comboBox3.SelectedIndex].b = Data.gegenlaeufig_1[Data.feld[0] - 1, Data.feld[0] - 1, comboBox5.SelectedIndex - 1];
                comboBox6.SelectedIndex = Data.verein[comboBox3.SelectedIndex].b;
            }
        }

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox3.SelectedIndex >= 0)
            {
                Data.verein[comboBox3.SelectedIndex].b = comboBox6.SelectedIndex;
                if (comboBox6.SelectedIndex == 0)
                    Data.verein[comboBox3.SelectedIndex].a = 0;
                else
                    Data.verein[comboBox3.SelectedIndex].a = Data.gegenlaeufig_1[Data.feld[0] - 1, Data.feld[0] - 1, comboBox6.SelectedIndex - 1];
                comboBox5.SelectedIndex = Data.verein[comboBox3.SelectedIndex].a;
            }
        }

        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox3.SelectedIndex >= 0)
            {
                Data.verein[comboBox3.SelectedIndex].x = comboBox7.SelectedIndex;
                if (comboBox7.SelectedIndex == 0)
                    Data.verein[comboBox3.SelectedIndex].y = 0;
                else
                    Data.verein[comboBox3.SelectedIndex].y = Data.gegenlaeufig_1[Data.feld[1] - 1, Data.feld[1] - 1, comboBox7.SelectedIndex - 1];
                comboBox8.SelectedIndex = Data.verein[comboBox3.SelectedIndex].y;
            }
        }

        private void comboBox8_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox3.SelectedIndex >= 0)
            {
                Data.verein[comboBox3.SelectedIndex].y = comboBox8.SelectedIndex;
                if (comboBox8.SelectedIndex == 0)
                    Data.verein[comboBox3.SelectedIndex].x = 0;
                else
                    Data.verein[comboBox3.SelectedIndex].x = Data.gegenlaeufig_1[Data.feld[1] - 1, Data.feld[1] - 1, comboBox8.SelectedIndex - 1];
                comboBox7.SelectedIndex = Data.verein[comboBox3.SelectedIndex].x;
            }
        }

        private void comboBox10_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox5.Items.Clear();
            comboBox5.Items.Add('-');
            for (int i = 1; i <= Data.toInt(comboBox10.SelectedItem.ToString()); i++)
                comboBox5.Items.Add(i);
            if (comboBox5.Items.Count > Data.toInt(comboBox5.Text))
                comboBox5.SelectedIndex = Data.toInt(comboBox5.Text);
            else
                comboBox5.SelectedIndex = 0;
            comboBox6.Items.Clear();
            comboBox6.Items.Add('-');
            for (int i = 1; i <= Data.toInt(comboBox10.SelectedItem.ToString()); i++)
                comboBox6.Items.Add(i);
            if (comboBox6.Items.Count > Data.toInt(comboBox6.Text))
                comboBox6.SelectedIndex = Data.toInt(comboBox6.Text);
            else
                comboBox6.SelectedIndex = 0;
            Data.feld[0] = Data.toInt(comboBox10.SelectedItem.ToString());
            checkWochen();
            if (comboBox10.SelectedIndex != -1 && comboBox11.SelectedIndex != -1 && Data.staffeln != null)
            {
                button6.Enabled = true;
                button2.Enabled = true;
                button1.Enabled = true;
            }
        }

        private void comboBox11_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox7.Items.Clear();
            comboBox7.Items.Add('-');
            for (int i = 1; i <= Data.toInt(comboBox11.SelectedItem.ToString()); i++)
                comboBox7.Items.Add(i);
            if (comboBox7.Items.Count > Data.toInt(comboBox7.Text))
                comboBox7.SelectedIndex = Data.toInt(comboBox7.Text);
            else
                comboBox5.SelectedIndex = 0;
            comboBox8.Items.Clear();
            comboBox8.Items.Add('-');
            for (int i = 1; i <= Data.toInt(comboBox11.SelectedItem.ToString()); i++)
                comboBox8.Items.Add(i);
            if (comboBox8.Items.Count > Data.toInt(comboBox8.Text))
                comboBox8.SelectedIndex = Data.toInt(comboBox8.Text);
            else
                comboBox8.SelectedIndex = 0;
            Data.feld[1] = Data.toInt(comboBox11.SelectedItem.ToString());
            checkWochen();
            if (comboBox10.SelectedIndex != -1 && comboBox11.SelectedIndex != -1 && Data.staffeln != null)
            {
                button6.Enabled = true;
                button2.Enabled = true;
                button1.Enabled = true;
            }
        }

        public static void checkWochen() 
        {
            if (Data.verein == null)
                return;
            for (int i = 0; i < Data.verein.Length; i++)
            {
                if (Data.verein[i].a > Data.feld[0])
                    Data.verein[i].a = 0;
                if (Data.verein[i].b > Data.feld[0])
                    Data.verein[i].b = 0;
                if (Data.verein[i].x > Data.feld[1])
                    Data.verein[i].x = 0;
                if (Data.verein[i].y > Data.feld[1])
                    Data.verein[i].y = 0;
            }
        }

        public void solveConflicts(Liga[] l, Verein[] v)
        {
            int[] konflikte = new int[2];
            List<Konflikt> k = new List<Konflikt>();
            Data.checkConflicts(l, k);
            if (k.Count > 0)
            {
                Alternativen a = new Alternativen(k.ToArray(), l, v, this);
                this.Enabled = false;
                a.Visible = true;
            }
            else
            {
                Data.generiereSchluesselzahlen();
                for (int i = 0; i < Data.meldung.Count; i++)
                    MessageBox.Show(Data.meldung[i]);
                Data.meldung.Clear();
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked && comboBox3.SelectedIndex != -1)
                Data.verein[comboBox3.SelectedIndex].kapazitaet = checkBox1.Checked;
        }

        private void comboBox12_SelectedIndexChanged(object sender, EventArgs e)
        {
            Data.liga[comboBox1.SelectedIndex].team[comboBox2.SelectedIndex].zahl = comboBox12.SelectedIndex;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            radioButton2.Checked = !radioButton1.Checked;
            if (radioButton1.Checked && comboBox3.SelectedIndex != -1)
                init();
            else
                clearGrid();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            radioButton1.Checked = !radioButton2.Checked;
            if (radioButton2.Checked && comboBox1.SelectedIndex != -1)
                init();
            else
                clearGrid();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            bool erfolg = loadFromFile(Data.vereine, Data.staffeln, Data.beziehungen);
            if (erfolg)
            {
                this.Enabled = false;
                Dateninput d = new Dateninput(this);
                d.Visible = true;
            }
        }

        private void Schluesselzahlen_Resize(object sender, EventArgs e)
        {
            dataGridView1.Height = this.Height - 100;
            dataGridView1.Width = this.Width - 350;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool erfolg = loadFromFile(Data.vereine, Data.staffeln, Data.beziehungen);
            if (erfolg)
            {
                this.Enabled = false;
                click_tt ctt = new click_tt(Data.liga, Data.verein, Data.partnerschaft, this);
                ctt.Visible = true;
            }
        }

        private void Schluesselzahlen_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!(Data.verein == null || Data.liga == null))
                switch (MessageBox.Show("Wollen Sie die Änderungen speichern?", "Änderungen speichern", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                {
                    case DialogResult.No:
                        break;
                    case DialogResult.Yes:
                        Data.speichern(Data.liga, Data.verein, Data.partnerschaft, Data.vereine, Data.staffeln, Data.beziehungen);
                        this.loadFromFile(Data.vereine, Data.staffeln, Data.beziehungen);
                        break;
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        break;
                }
        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (radioButton2.Checked || comboBox3.SelectedIndex == -1 || e.RowIndex == -1 || e.RowIndex >= verein.team.Length)
                    return;
                if (comboBox4.SelectedIndex >= 0)
                {
                    switch (comboBox4.SelectedIndex)
                    {
                        case 0: verein.team[e.RowIndex].woche = '-'; break;
                        case 1: verein.team[e.RowIndex].woche = 'A'; break;
                        case 2: verein.team[e.RowIndex].woche = 'B'; break;
                        case 3: verein.team[e.RowIndex].woche = 'X'; break;
                        case 4: verein.team[e.RowIndex].woche = 'Y'; break;
                    }
                    init();
                }
            }

            if (e.Button == MouseButtons.Right)
            {
                if (dataGridView1.Rows[e.RowIndex].IsNewRow)
                    return;
                Team t = null;
                if (radioButton1.Checked)
                {
                    Verein v = Data.verein[comboBox3.SelectedIndex];
                    t = v.team.ElementAt(e.RowIndex);
                }
                else if (radioButton2.Checked)
                {
                    Liga l = Data.liga[comboBox1.SelectedIndex];
                    t = l.team.ElementAt(e.RowIndex);
                }
                Zusatz z = new Zusatz(t, this);
                this.Enabled = false;
                z.Visible = true;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Partner p = new Partner(this, Data.verein[comboBox3.SelectedIndex]);
            this.Enabled = false;
            p.Visible = true;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
