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
    public partial class Dateninput : Form
    {
        int team_max;
        bool automatic;
        CheckBox[] external;
        TextBox[] team_name;
        TextBox[] team_ident;
        Button[] reset;
        public List<Verein> verein;
        public List<Liga> liga;
        public List<Partnerschaft> partnerschaft;
        Schluesselzahlen caller;

        public Dateninput(Schluesselzahlen caller)
        {
            InitializeComponent();
            this.caller = caller;
            team_max = Data.team_max;
            automatic = false;
            external = new CheckBox[team_max];
            team_name = new TextBox[team_max];
            team_ident = new TextBox[team_max];
            reset = new Button[team_max];
            initGrid();
            assignGUIElements();
            enableGUIElements();
            loadData();
        }

        private void initGrid()
        {
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Add("Verein", "Verein");
            dataGridView1.Columns.Add("A", "A");
            dataGridView1.Columns.Add("B", "B");
            dataGridView1.Columns.Add("X", "X");
            dataGridView1.Columns.Add("Y", "Y");
            dataGridView1.Columns.Add("Kap.", "Kap.");
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            dataGridView1.AutoResizeColumns();
            dataGridView1.SelectionMode = DataGridViewSelectionMode.CellSelect;
            foreach (DataGridViewColumn col in dataGridView1.Columns)
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
        }

        private void loadData()
        {
            button18.Enabled = false;
            String[] values = new String[6];
            partnerschaft = new List<Partnerschaft>();
            Verein[] verein_array = Data.getVereine(Data.vereine, partnerschaft);
            initGrid();
            for (int i = 0; i < verein_array.Length; i++)
            {
                values[0] = verein_array[i].name;
                values[1] = verein_array[i].a.ToString() == "0" ? "" : verein_array[i].a.ToString();
                values[2] = verein_array[i].b.ToString() == "0" ? "" : verein_array[i].b.ToString();
                values[3] = verein_array[i].x.ToString() == "0" ? "" : verein_array[i].x.ToString();
                values[4] = verein_array[i].y.ToString() == "0" ? "" : verein_array[i].y.ToString();
                if (verein_array[i].kapazitaet)
                    values[5] = "X";
                else
                    values[5] = "";
                dataGridView1.Rows.Add(values);
            }

            Liga[] liga_array = Data.getStaffeln(verein_array, Data.staffeln);
            Data.getBeziehungen(liga_array, Data.beziehungen);
            Data.allocateTeams(verein_array, liga_array);

            verein = verein_array.ToList<Verein>();
            liga = liga_array.ToList<Liga>();
            comboBox1.Items.Clear();
            for (int i = 0; i < liga_array.Length; i++)
                comboBox1.Items.Add(liga_array[i].name);
            comboBox1.SelectedIndex = -1;
            button18.Enabled = true;
        }

        private void Dateninput_FormClosing(object sender, FormClosingEventArgs e)
        {
            switch (MessageBox.Show("Wollen Sie die Änderungen speichern?", "Änderungen speichern", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
            {
                case DialogResult.No:
                    caller.Enabled = true;
                    caller.Focus();
                    break;
                case DialogResult.Yes:
                    Data.speichern(liga.ToArray(), verein.ToArray(), partnerschaft, Data.vereine, Data.staffeln, Data.beziehungen);
                    caller.loadFromFile(Data.vereine, Data.staffeln, Data.beziehungen);
                    caller.Enabled = true;
                    caller.Focus();
                    break;
                case DialogResult.Cancel:
                    e.Cancel = true;
                    break;
            }
        }

        private void splitContainer1_Resize(object sender, EventArgs e)
        {
            splitContainer1.SplitterDistance = splitContainer1.Width - 400;
        }

        private void button18_Click(object sender, EventArgs e)
        {
            Data.speichern(liga.ToArray(), verein.ToArray(), partnerschaft, Data.vereine, Data.staffeln, Data.beziehungen);
            caller.loadFromFile(Data.vereine, Data.staffeln, Data.beziehungen);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            enableGUIElements();
        }

        public void enableGUIElements()
        {
            automatic = true;
            if (comboBox1.SelectedIndex == -1)
                for (int i = 0; i < team_max; i++)
                {
                    external[i].Enabled = false;
                    external[i].Checked = false;
                    team_name[i].Enabled = false;
                    team_name[i].Text = "";
                    team_ident[i].Enabled = false;
                    team_ident[i].Text = "";
                    reset[i].Enabled = false;
                }
            else
            {
                for (int i = 0; i < liga.ElementAt(comboBox1.SelectedIndex).anzahl_teams; i++)
                {
                    external[i].Enabled = false;
                    team_ident[i].Enabled = true;
                    reset[i].Enabled = true;
                    
                    if (verein.Contains(liga.ElementAt(comboBox1.SelectedIndex).team[i].verein))
                    {
                        external[i].Checked = false;
                        team_name[i].Enabled = false;
                    }
                    else
                    {
                        external[i].Checked = true;
                        team_name[i].Enabled = true;
                    }

                    team_name[i].Text = liga.ElementAt(comboBox1.SelectedIndex).team[i].verein.name;
                    team_ident[i].Text = liga.ElementAt(comboBox1.SelectedIndex).team[i].team;

                }
                for (int i = liga.ElementAt(comboBox1.SelectedIndex).anzahl_teams; i < team_max; i++)
                {
                    external[i].Enabled = false;
                    external[i].Checked = false;
                    team_name[i].Enabled = false;
                    team_name[i].Text = "";
                    team_ident[i].Enabled = false;
                    team_ident[i].Text = "";
                    reset[i].Enabled = false;
                }
                if (liga.ElementAt(comboBox1.SelectedIndex).anzahl_teams < liga.ElementAt(comboBox1.SelectedIndex).feld)
                    external[liga.ElementAt(comboBox1.SelectedIndex).anzahl_teams].Enabled = true; ;
            }
            automatic = false;
        }

        private void deleteTeam(Liga l, int t)
        {
            l.anzahl_teams--;
            for (int i = t; i < l.anzahl_teams; i++)
                l.team[i] = l.team[i + 1];
            l.team[l.anzahl_teams] = null;
            enableGUIElements();
        }

        private void createExternal(Liga l, int t)
        {
            if (l.anzahl_teams == t)
                l.anzahl_teams++;
            l.team[t] = new Team();
            l.team[t].index = t;
            l.team[t].liga = l;
            l.team[t].woche = '-';
            l.team[t].zahl = 0;
            l.team[t].verein = new Verein();
            l.team[t].verein.index = -1;
            enableGUIElements();
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (comboBox1.SelectedIndex == -1)
                return;
            Liga l = liga.ElementAt(comboBox1.SelectedIndex);
            if (e.ColumnIndex == 0 && l.anzahl_teams < l.feld)
            {
                l.team[l.anzahl_teams] = new Team();
                l.team[l.anzahl_teams].index = l.anzahl_teams;
                l.team[l.anzahl_teams].name = verein.ElementAt(e.RowIndex).name;
                l.team[l.anzahl_teams].team = "";
                l.team[l.anzahl_teams].liga = l;
                l.team[l.anzahl_teams].verein = verein.ElementAt(e.RowIndex);
                l.team[l.anzahl_teams].woche = '-';
                l.team[l.anzahl_teams].zahl = 0;
                l.anzahl_teams++;
                enableGUIElements();
            }
        }

        private void buttonI_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < team_max; i++)
                if (sender.Equals(reset[i]))
                    deleteTeam(liga.ElementAt(comboBox1.SelectedIndex), i);
        }

        private void checkBoxI_CheckedChanged(object sender, EventArgs e)
        {
            if (!automatic)
                for (int i=0; i<team_max; i++)
                    if (sender.Equals(external[i]))
                    {
                        if (external[i].Checked)
                            createExternal(liga.ElementAt(comboBox1.SelectedIndex), i);
                        else
                            deleteTeam(liga.ElementAt(comboBox1.SelectedIndex), i);
                    }
        }

        private void textBoxI_Leave(object sender, EventArgs e)
        {
            for (int i = 0; i < team_max; i++)
                if (sender.Equals(team_name[i]) || sender.Equals(team_ident[i]))
                {
                    team_name[i].Text = Data.clear(team_name[i].Text);
                    team_ident[i].Text = Data.clear(team_ident[i].Text);
                    liga.ElementAt(comboBox1.SelectedIndex).team[i].name = team_name[i].Text + " " + team_ident[i].Text;
                    liga.ElementAt(comboBox1.SelectedIndex).team[i].team = team_ident[i].Text;
                    liga.ElementAt(comboBox1.SelectedIndex).team[i].verein.name = team_name[i].Text;
                }
        }

        private void dataGridView1_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Rows[e.RowIndex].IsNewRow && e.ColumnIndex != 0)
                return;
            String value = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].EditedFormattedValue.ToString();
            int zahl = Data.toInt(value);
            switch (e.ColumnIndex)
            {
                case 0:
                    if (value.Equals(""))
                        break;
                    verein.ElementAt(e.RowIndex).name = Data.clear(value);
                    dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = verein.ElementAt(e.RowIndex).name;
                    break;
                case 1:
                    if (zahl > 0 && zahl <= Data.feld[0])
                    {
                        verein.ElementAt(e.RowIndex).a = zahl;
                        verein.ElementAt(e.RowIndex).b = Data.gegenlaeufig_1[Data.feld[0]-1, Data.feld[0]-1, zahl-1];
                    }
                    else
                    {
                        verein.ElementAt(e.RowIndex).a = 0;
                        verein.ElementAt(e.RowIndex).b = 0;
                    }
                    if (verein.ElementAt(e.RowIndex).a == 0)
                        dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "";
                    else
                        dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = verein.ElementAt(e.RowIndex).a.ToString();
                    if (verein.ElementAt(e.RowIndex).b == 0)
                        dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex + 1].Value = "";
                    else
                        dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex + 1].Value = verein.ElementAt(e.RowIndex).b.ToString();
                    break;
                case 2:
                    if (zahl > 0 && zahl <= Data.feld[0])
                    {
                        verein.ElementAt(e.RowIndex).b = zahl;
                        verein.ElementAt(e.RowIndex).a = Data.gegenlaeufig_1[Data.feld[0]-1, Data.feld[0]-1, zahl-1];
                    }
                    else
                    {
                        verein.ElementAt(e.RowIndex).a = 0;
                        verein.ElementAt(e.RowIndex).b = 0;
                    }
                    if (verein.ElementAt(e.RowIndex).a == 0)
                        dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex - 1].Value = "";
                    else
                        dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex - 1].Value = verein.ElementAt(e.RowIndex).a.ToString();
                    if (verein.ElementAt(e.RowIndex).b == 0)
                        dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "";
                    else
                        dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = verein.ElementAt(e.RowIndex).b.ToString();
                    break;
                case 3:
                    if (zahl > 0 && zahl <= Data.feld[1])
                    {
                        verein.ElementAt(e.RowIndex).x = zahl;
                        verein.ElementAt(e.RowIndex).y = Data.gegenlaeufig_1[Data.feld[1]-1, Data.feld[1]-1, zahl-1];
                    }
                    else
                    {
                        verein.ElementAt(e.RowIndex).x = 0;
                        verein.ElementAt(e.RowIndex).y = 0;
                    }
                    if (verein.ElementAt(e.RowIndex).x == 0)
                        dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "";
                    else
                        dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = verein.ElementAt(e.RowIndex).x.ToString();
                    if (verein.ElementAt(e.RowIndex).y == 0)
                        dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex + 1].Value = "";
                    else
                        dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex + 1].Value = verein.ElementAt(e.RowIndex).y.ToString();
                    break;
                case 4:
                    if (zahl > 0 && zahl <= Data.feld[1])
                    {
                        verein.ElementAt(e.RowIndex).y = zahl;
                        verein.ElementAt(e.RowIndex).x = Data.gegenlaeufig_1[Data.feld[1]-1, Data.feld[1]-1, zahl-1];
                    }
                    else
                    {
                        verein.ElementAt(e.RowIndex).y = 0;
                        verein.ElementAt(e.RowIndex).x = 0;
                    }
                    if (verein.ElementAt(e.RowIndex).x == 0)
                        dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex - 1].Value = "";
                    else
                        dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex - 1].Value = verein.ElementAt(e.RowIndex).x.ToString();
                    if (verein.ElementAt(e.RowIndex).y == 0)
                        dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "";
                    else
                        dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = verein.ElementAt(e.RowIndex).y.ToString();
                    break;
                case 5:
                    if (value.Equals(""))
                        verein.ElementAt(e.RowIndex).kapazitaet = false;
                    else
                        verein.ElementAt(e.RowIndex).kapazitaet = true;
                    dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = verein.ElementAt(e.RowIndex).kapazitaet ? "X" : "";
                    break;
            }
        }

        private void dataGridView1_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            Verein v = new Verein();
            v.index = e.Row.Index - 1;
            for (int i = e.Row.Index - 1; i < verein.Count; i++)
                verein.ElementAt(i).index++;
            verein.Insert(e.Row.Index - 1, v);
        }

        private void dataGridView1_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            foreach (Partnerschaft p in partnerschaft)
                if (p.a.index == e.Row.Index - 1 || p.b.index == e.Row.Index - 1)
                    partnerschaft.Remove(p);
            verein.RemoveAt(e.Row.Index - 1);
            for (int i = e.Row.Index - 1; i < verein.Count; i++)
                verein.ElementAt(i).index--;
            
        }
        
        private void assignGUIElements()
        {
            external[0] = checkBox1;
            external[1] = checkBox2;
            external[2] = checkBox3;
            external[3] = checkBox4;
            external[4] = checkBox5;
            external[5] = checkBox6;
            external[6] = checkBox7;
            external[7] = checkBox8;
            external[8] = checkBox9;
            external[9] = checkBox10;
            external[10] = checkBox11;
            external[11] = checkBox12;
            external[12] = checkBox13;
            external[13] = checkBox14;
            team_name[0] = textBox1;
            team_name[1] = textBox2;
            team_name[2] = textBox3;
            team_name[3] = textBox4;
            team_name[4] = textBox5;
            team_name[5] = textBox6;
            team_name[6] = textBox7;
            team_name[7] = textBox8;
            team_name[8] = textBox9;
            team_name[9] = textBox10;
            team_name[10] = textBox11;
            team_name[11] = textBox12;
            team_name[12] = textBox13;
            team_name[13] = textBox14;
            team_ident[0] = textBox15;
            team_ident[1] = textBox16;
            team_ident[2] = textBox17;
            team_ident[3] = textBox18;
            team_ident[4] = textBox19;
            team_ident[5] = textBox20;
            team_ident[6] = textBox21;
            team_ident[7] = textBox22;
            team_ident[8] = textBox23;
            team_ident[9] = textBox24;
            team_ident[10] = textBox25;
            team_ident[11] = textBox26;
            team_ident[12] = textBox27;
            team_ident[13] = textBox28;
            reset[0] = button1;
            reset[1] = button2;
            reset[2] = button3;
            reset[3] = button4;
            reset[4] = button5;
            reset[5] = button6;
            reset[6] = button7;
            reset[7] = button8;
            reset[8] = button9;
            reset[9] = button10;
            reset[10] = button11;
            reset[11] = button12;
            reset[12] = button13;
            reset[13] = button14;
        }

        private void button15_Click(object sender, EventArgs e)
        {
            Liga l = new Liga();
            Staffel s = new Staffel(l, true, this);
            s.Visible = true;
            this.Enabled = false;
        }

        private void button16_Click(object sender, EventArgs e)
        {
            Staffel s;
            if (comboBox1.SelectedIndex != -1)
            {
                s = new Staffel(liga[comboBox1.SelectedIndex], false, this);
                s.Visible = true;
                this.Enabled = false;
            }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex != -1)
                switch (MessageBox.Show("Wollen Sie die " + liga[comboBox1.SelectedIndex].name + " löschen?", "Liga löschen", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                {
                    case DialogResult.No:
                        break;
                    case DialogResult.Yes:
                        liga.RemoveAt(comboBox1.SelectedIndex);
                        for (int i = comboBox1.SelectedIndex; i < liga.Count; i++)
                            liga[i].index--;
                        comboBox1.Items.RemoveAt(comboBox1.SelectedIndex);
                        enableGUIElements();
                        this.Enabled = true;
                        this.Focus();
                        break;
                    case DialogResult.Cancel:
                        break;
                    case DialogResult.Abort:
                        break;
                }
        }

        private void Dateninput_Resize(object sender, EventArgs e)
        {
            caller.WindowState = this.WindowState;
            this.Focus();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
