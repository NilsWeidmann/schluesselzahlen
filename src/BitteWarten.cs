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
    public partial class BitteWarten : Form
    {
        Schluesselzahlen caller;
        Liga[] best_l;
        Verein[] best_v;

        public BitteWarten(Schluesselzahlen caller)
        {
            this.caller = caller;
            caller.Enabled = false;
            InitializeComponent();
            initProgressBar(progressBar1);
            backgroundWorker1.RunWorkerAsync();
        }

        public void initProgressBar(ProgressBar pb)
        {
            pb.Minimum = 0;
            pb.Maximum = 1000;
            pb.Value = 0;
        }

        private void cancel(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
            returnToCaller();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            int[] konflikte = { 0, -1 };
            best_l = new Liga[Data.liga.Length];
            best_v = new Verein[Data.verein.Length];
            Data.speichern(Data.liga, Data.verein, Data.partnerschaft, Data.vereine_b, Data.staffeln_b, Data.beziehungen_b);
            if (Data.meldung.Count > 0)
            {
                MessageBox.Show(Data.meldung[0]);
                return;
            }
            Data.setOptions();
            Data.setWeeks();
            Data.copyKeys();
            Data.createPriority();
            Data.copy(Data.liga, best_l, Data.verein, best_v, Data.partnerschaft, Data.partnerschaft);
            Data.checkPlausibility(Data.liga, this, Data.meldung);
            Data.checkFatal(Data.liga, Data.meldung);
            if (Data.meldung.Count > 0)
            {
                MessageBox.Show(Data.meldung[0]);
                return;
            }
            //Data.ht.Clear();
            DateTime start = DateTime.Now;
            DateTime end = DateTime.Now;
            DateTime report = DateTime.Now;
            TimeSpan span = end - start;
            TimeSpan repspan = end -report;
            int[] schluessel = new int[Data.verein.Length * 2];
            backgroundWorker1.ReportProgress((int)(span.TotalSeconds * 100 / Data.laufzeit));
            while (span.TotalSeconds < Data.laufzeit && !Data.ht.Contains("") && konflikte[1] != 0)
            {
                if (backgroundWorker1.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                if (repspan.TotalSeconds >= 1)
                {
                    report = DateTime.Now;
                    backgroundWorker1.ReportProgress((int)(span.TotalSeconds * 100 / Data.laufzeit));
                }
                Data.findSolution(0, Data.liga, best_l, Data.verein, best_v, konflikte, schluessel);
                end = DateTime.Now;
                span = end - start;
                repspan = end - report;
            }
            backgroundWorker1.ReportProgress(0);
            if (konflikte[1] == -1)
                Data.meldung.Add("Es konnten keine Schlüsselzahlen ermittelt werden!");
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage * 10;
            int seconds = Data.laufzeit * (100 - e.ProgressPercentage) / 100 % 60;
            int minutes = Data.laufzeit * (100 - e.ProgressPercentage) / 6000;
            label2.Text = "";
            if (minutes < 10)
                label2.Text += "0" + minutes;
            else
                label2.Text += minutes;
            label2.Text += ":";
            if (seconds < 10)
                label2.Text += "0" + seconds;
            else
                label2.Text += seconds;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            returnToCaller();
            caller.showResults(best_l, best_v, e.Cancelled);
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

        private void BitteWarten_FormClosing(object sender, FormClosingEventArgs e)
        {
            backgroundWorker1.CancelAsync();
            returnToCaller();
        }

        private void BitteWarten_Resize(object sender, EventArgs e)
        {
            caller.WindowState = this.WindowState;
            this.Focus();
        }
    }
}
