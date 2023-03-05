using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using HtmlAgilityPack;

namespace Schluesselzahlen
{
    public partial class click_tt : Form
    {
        List<Liga> ll;
        List<Verein> lv;
        List<Partnerschaft> lp;
        Schluesselzahlen caller;
        
        public click_tt(Liga[] l, Verein[] v, List<Partnerschaft> p, Schluesselzahlen caller)
        {
            InitializeComponent();
            this.caller = caller;
            init();
            ll = l.ToList();
            foreach (Liga liga in ll)
                dataGridView1.Rows.Add(liga.name);
            lv = v.ToList();
            foreach (Verein verein in lv)
                dataGridView2.Rows.Add(verein.name);
            lp = p;
        }

        private void init()
        {
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView1.Columns.Clear();
            dataGridView1.Columns.Add("Liga", "Liga");
            dataGridView1.Rows.Clear();
            dataGridView1.ReadOnly = true;
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView2.Columns.Clear();
            dataGridView2.Columns.Add("Verein", "Verein");
            dataGridView2.Rows.Clear();
            dataGridView2.ReadOnly = true;
            dataGridView3.ReadOnly = true;
            dataGridView3.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView3.Columns.Clear();
            dataGridView3.Columns.Add("Team", "Team");
            dataGridView3.Rows.Clear();
        }

        private void webImportStaffeln()
        {
            this.Enabled = false;
            HtmlAgilityPack.HtmlWeb web = new HtmlAgilityPack.HtmlWeb();
            HtmlAgilityPack.HtmlDocument staffeln = null;
            HtmlAgilityPack.HtmlDocument teams = null;
            string altersklasse = "";
            string spielklasse = "";
            int index_l = ll.Count;
            int index_t = 0;

            try
            {
                string[] seperators = { "/" };
                string protocol = textBox1.Text.Split(seperators, StringSplitOptions.RemoveEmptyEntries)[0];
                string domain = textBox1.Text.Split(seperators, StringSplitOptions.RemoveEmptyEntries)[1];
                HtmlNodeCollection tables = null;
                HtmlNode staffel_table = null;
                HtmlNode team_table = null;

                staffeln = web.Load(textBox1.Text);
                tables = staffeln.DocumentNode.SelectNodes("//table");
                foreach (HtmlNode tab in tables)
                    if (tab.GetAttributeValue("class", "").Equals("matrix"))
                        staffel_table = tab;
                foreach (HtmlNode node_s in staffel_table.Descendants())
                {
                    if (node_s.Name.Equals("h2"))
                        altersklasse = Data.clear(node_s.InnerText);
                    if (node_s.Name.Equals("a") && !node_s.ParentNode.GetAttributeValue("class", "").Equals("matrix-relegation-more"))
                    {
                        spielklasse = Data.clear(node_s.InnerText);
                        Liga l = new Liga();
                        l.name = altersklasse + " " + spielklasse;
                        l.index = index_l++;
                        index_t = 0;
                        List<Team> lt = new List<Team>();

                        string reference = node_s.GetAttributeValue("href", "");
                        reference = reference.Replace("&amp;", "&");
                        string uri = protocol + "//" + domain + reference;
                        teams = web.Load(uri);
                        tables = teams.DocumentNode.SelectNodes("//table");
                        if (tables == null)
                            continue;
                        foreach (HtmlNode tab in tables)
                            if (tab.GetAttributeValue("class", "").Equals("result-set"))
                            {
                                team_table = tab;
                                break;
                            }
                        if (team_table == null)
                            continue;
                        foreach (HtmlNode node_t in team_table.Descendants())
                            if (node_t.Name.Equals("td"))
                                if (node_t.GetAttributeValue("nowrap", "").Equals("nowrap"))
                                {
                                    Team t = new Team();
                                    char[] trimChars = { ' ' };
                                    t.name = node_t.InnerText.Replace("\n", "");
                                    t.name = t.name.TrimStart(trimChars);
                                    t.name = t.name.TrimEnd(trimChars);
                                    t.name = Data.clear(t.name);
                                    t.liga = l;
                                    l.anzahl_teams++;
                                    t.woche = '-';
                                    t.index = index_t++;
                                    for (int i = 0; i < lv.Count; i++)
                                        if (Data.checkVerein(t, lv.ElementAt(i)))
                                            t.verein = lv.ElementAt(i);
                                    if (t.verein == null)
                                    {
                                        t.verein = new Verein();
                                        t.verein.name = t.name;
                                        t.verein.index = -1;
                                        t.woche = '-';
                                    }
                                    lt.Add(t);
                                }
                        while (lt.Count < 14)
                            lt.Add(null);
                        l.team = lt.ToArray();
                        l.feld = l.anzahl_teams + l.anzahl_teams % 2;
                        ll.Add(l);
                        dataGridView1.Rows.Add(l.name);
                    }
                }
                this.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Beim Dateninport ist ein Fehler aufgetreten. Bitte den Link überprüfen!");
                this.Enabled = true;
                return;
            }
        }

        private void webImportVereine()
        {
            HtmlAgilityPack.HtmlWeb web = new HtmlAgilityPack.HtmlWeb();
            HtmlAgilityPack.HtmlDocument vereine = null;

            try
            {
                vereine = web.Load(textBox2.Text);
                HtmlNode club_table = null;

                string[] seperators = { "/" };
                string domain = textBox2.Text.Split(seperators, StringSplitOptions.RemoveEmptyEntries)[1];
                int index = lv.Count;

                HtmlNodeCollection tables = vereine.DocumentNode.SelectNodes("//table");

                foreach (HtmlNode tab in tables)
                    if (tab.Attributes["class"].Value.ToString().Equals("result-set"))
                        club_table = tab;
                foreach (HtmlNode link in club_table.Descendants())
                {
                    if (link.Name.Equals("a"))
                    {
                        Verein verein = new Verein();
                        verein.name = Data.clear(link.InnerText);
                        verein.index = index++;
                        lv.Add(verein);
                        dataGridView2.Rows.Add(verein.name);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Beim Dateninport ist ein Fehler aufgetreten. Bitte den Link überprüfen!");
                return;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            lv = new List<Verein>();
            lp = new List<Partnerschaft>();
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView2.Columns.Clear();
            dataGridView2.Columns.Add("Verein", "Verein");
            dataGridView2.Rows.Clear();
            webImportVereine();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ll = new List<Liga>();
            dataGridView1.Rows.Clear();
            dataGridView3.Rows.Clear();
            webImportStaffeln();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            caller.WindowState = this.WindowState;
            this.Focus();

            // Set proper heights
            groupBox2.Top = (this.Height - 95) / 2 + 20;
            groupBox1.Height = (this.Height - 95) / 2;
            groupBox2.Height = (this.Height - 95) / 2;
            dataGridView1.Height = (this.Height - 200) / 2;
            dataGridView2.Height = (this.Height - 200) / 2;
            dataGridView3.Height = (this.Height - 200) / 2;

            // Set proper widths
            dataGridView1.Width = (this.Width - 68) / 2;
            dataGridView2.Width = this.Width - 56;
            dataGridView3.Width = (this.Width - 68) / 2;
            groupBox1.Width = this.Width - 40;
            groupBox2.Width = this.Width - 40;
            textBox1.Width = this.Width - 250;
            textBox2.Width = this.Width - 250;
            button1.Left = groupBox1.Width - 80;
            button2.Left = groupBox2.Width - 80;
            button4.Left = groupBox1.Width - 160;
            button5.Left = groupBox2.Width - 160;
            dataGridView3.Left = dataGridView1.Left + dataGridView1.Width + 12;
            button3.Left = this.Width - 110;
            button3.Top = this.Height - 70;
        }

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView3.Rows.Clear();
            if (ll != null && e.RowIndex < ll.Count)
            {
                for (int i = 0; i < ll[e.RowIndex].anzahl_teams; i++)
                    dataGridView3.Rows.Add(ll[e.RowIndex].team[i].name);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            button3.Enabled = false;
            Data.speichern(ll.ToArray(), lv.ToArray(), lp, Data.vereine, Data.staffeln, Data.beziehungen);
            caller.loadFromFile(Data.vereine, Data.staffeln, Data.beziehungen);
            button3.Enabled = true;
        }

        private void click_tt_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (button3.Enabled)
            {
                switch (MessageBox.Show("Wollen Sie die Änderungen speichern?", "Änderungen speichern", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                {
                    case DialogResult.No:
                        caller.Enabled = true;
                        caller.Focus();
                        break;
                    case DialogResult.Yes:
                        Data.speichern(ll.ToArray(), lv.ToArray(), lp, Data.vereine, Data.staffeln, Data.beziehungen);
                        caller.loadFromFile(Data.vereine, Data.staffeln, Data.beziehungen);
                        caller.Enabled = true;
                        caller.Focus();
                        break;
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        break;
                }
            }
            else
            {
                this.Visible = false;
                caller.Enabled = true;
                caller.Focus();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            webImportStaffeln();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            webImportVereine();
        }
    }
}
