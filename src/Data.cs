using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Schluesselzahlen
{
    public class Data
    {
        public static int team_max = 14;
        public static Liga[] liga;
        public static Verein[] verein;
        public static List<Partnerschaft> partnerschaft = new List<Partnerschaft>();
        public static Schluesselzahlen caller;
        public static Textdatei gegenlaeufig;
        public static Textdatei parallel;
        public static Textdatei vereinsintern;
        public static Textdatei aehnlich;
        public static Textdatei staffeln;
        public static Textdatei staffeln_b;
        public static Textdatei vereine;
        public static Textdatei vereine_b;
        public static Textdatei beziehungen;
        public static Textdatei beziehungen_b;
        public static Textdatei spielplan;
        public static List<String> meldung = new List<String>();
        public static int[,] feld_g;
        public static int[,] feld_p;
        public static int[,] feld_v;
        public static int[,] feld_a;
        public static int[,] aehnlich_1;
        public static int[,] aehnlich_2;
        public static int[,,] gegenlaeufig_1;
        public static int[,,] parallel_1;
        public static char[, ,] spielplan_1;
        public static int[] feld = new int[2];
        public static int[] prio;
        public static int laufzeit;
        public static Hashtable ht = new Hashtable();
        public static int[] rand = { 0, 0 };

        public static int resetKeys(int l, int t, ComboBox c, bool neu)
        {
            bool[] waehlbar = new bool[team_max];
            int n = liga[l].anzahl_teams;
            int r = -1;

            if (neu)
                n++;
            for (int j = 0; j < team_max; j++)
                waehlbar[j] = true;
            for (int j = 0; j < liga[l].anzahl_teams; j++)
                if ((liga[l].team[j].zahl + 1) / 2 > (liga[l].feld) / 2)
                    liga[l].team[j].zahl = 0;
                else if (liga[l].team[j].zahl > 0 && j != t)
                    waehlbar[liga[l].team[j].zahl - 1] = false;
            c.Items.Clear();

            for (int j = 0; n > j; j += 2)
            {
                if (waehlbar[j])
                    c.Items.Add(j + 1);
                if (t != liga[l].anzahl_teams)
                    if (j == liga[l].team[t].zahl - 1)
                        r = c.Items.Count - 1;
                if (waehlbar[j + 1])
                    c.Items.Add(j + 2);
                if (t != liga[l].anzahl_teams)
                    if (j == liga[l].team[t].zahl - 2)
                        r = c.Items.Count - 1;
            }
            return r;
        }

        public static void copy(Liga[] l1, Liga[] l2, Verein[] v1, Verein[] v2, List<Partnerschaft> p1, List<Partnerschaft> p2)
        {
            for (int i = 0; i < v1.Length && v1[i] != null; i++)
            {
                v2[i] = new Verein();
                v2[i].name = v1[i].name;
                v2[i].index = v1[i].index;
                v2[i].a = v1[i].a;
                v2[i].b = v1[i].b;
                v2[i].x = v1[i].x;
                v2[i].y = v1[i].y;
                v2[i].team = new Team[v1[i].team.Length];
                v2[i].kapazitaet = v1[i].kapazitaet;
            }
            p2 = new List<Partnerschaft>();
            foreach (Partnerschaft p in p1)
                p2.Add(new Partnerschaft(v2[p.a.index], p.woche_a, v2[p.b.index], p.woche_b));

            for (int i = 0; i < l1.Length && l1[i] != null; i++)
            {
                l2[i] = new Liga();
                l2[i].anzahl_teams = l1[i].anzahl_teams;
                l2[i].index = l1[i].index;
                l2[i].name = l1[i].name;
                l2[i].team = new Team[team_max];
                l2[i].feld = l1[i].feld;
                for (int j = 0; j < l1[i].team.Length && l1[i].team[j] != null; j++)
                {
                    l2[i].team[j] = new Team();
                    l2[i].team[j].index = l1[i].team[j].index;
                    l2[i].team[j].liga = l2[i];
                    l2[i].team[j].name = l1[i].team[j].name;
                    l2[i].team[j].option = new bool[team_max];
                    for (int k = 0; k < team_max; k++)
                        l2[i].team[j].option[k] = l1[i].team[j].option[k];
                    for (int k = 0; k < l1[i].team[j].spieltag.Length; k++)
                        l2[i].team[j].spieltag[k] = l1[i].team[j].spieltag[k];
                    l2[i].team[j].woche = l1[i].team[j].woche;
                    l2[i].team[j].team = l1[i].team[j].team;
                    if (l1[i].team[j].verein.index == -1)
                    {
                        l2[i].team[j].verein = new Verein();
                        l2[i].team[j].verein.name = l1[i].team[j].verein.name;
                        l2[i].team[j].verein.index = l1[i].team[j].verein.index;
                        l2[i].team[j].verein.a = l1[i].team[j].verein.a;
                        l2[i].team[j].verein.b = l1[i].team[j].verein.b;
                        l2[i].team[j].verein.x = l1[i].team[j].verein.x;
                        l2[i].team[j].verein.y = l1[i].team[j].verein.y;
                        l2[i].team[j].verein.kapazitaet = l1[i].team[j].verein.kapazitaet;
                        l2[i].team[j].verein.team = new Team[team_max];
                    }
                    else
                    {
                        l2[i].team[j].verein = v2[l1[i].team[j].verein.index];
                        for (int k = 0; k < v2[l1[i].team[j].verein.index].team.Length; k++)
                            if (v2[l1[i].team[j].verein.index].team[k] == null)
                            {
                                v2[l1[i].team[j].verein.index].team[k] = l2[i].team[j];
                                break;
                            }
                    }
                    l2[i].team[j].zahl = l1[i].team[j].zahl;
                }
            }
        }

        public static void setOptions()
        {
            for (int i = 0; i < liga.Length; i++)
                for (int j = 0; j < liga[i].anzahl_teams; j++)
                    if (liga[i].team[j].zahl != 0)
                    {
                        for (int k = 0; k < team_max; k++)
                            liga[i].team[j].option[k] = false;
                        liga[i].team[j].option[liga[i].team[j].zahl - 1] = true;
                    }
                    else
                    {
                        for (int k = 0; k < liga[i].feld; k++)
                        {
                            if (liga[i].team[j].zahlOK(k + 1))
                                liga[i].team[j].option[k] = true;
                            else
                                liga[i].team[j].option[k] = false;
                            for (int l = 0; l < liga[i].anzahl_teams; l++)
                                if (liga[i].team[l].zahl == k + 1)
                                    liga[i].team[j].option[k] = false;
                        }
                        for (int k = liga[i].feld; k < team_max; k++)
                            liga[i].team[j].option[k] = false;
                    }
        }

        public static void setWeeks()
        {
            for (int i = 0; i < liga.Length; i++)
                for (int j = 0; j < liga[i].anzahl_teams; j++)
                    if (liga[i].team[j].zahl != 0)
                        liga[i].team[j].woche = '-';
        }

        public static int toInt(String s)
        {
            char[] zeichen = s.ToCharArray();
            int r = 0;

            for (int i = 0; i < zeichen.Length; i++)
            {
                r *= 10;
                switch (zeichen[i])
                {
                    case '0': r += 0; break;
                    case '1': r += 1; break;
                    case '2': r += 2; break;
                    case '3': r += 3; break;
                    case '4': r += 4; break;
                    case '5': r += 5; break;
                    case '6': r += 6; break;
                    case '7': r += 7; break;
                    case '8': r += 8; break;
                    case '9': r += 9; break;
                    default : r /=10; break;
                }
            }
            return r;
        }

        public static Verein[] getVereine(Textdatei ver, List<Partnerschaft> p)
        {
            int anzahl_vereine;
            String inhalt = ver.ReadLine(1, false, meldung).Replace("\r", "");
            String[] hilf;
            for (anzahl_vereine=0;!inhalt.Equals(""); anzahl_vereine++)
                inhalt = ver.ReadLine(2 + anzahl_vereine, false, meldung).Replace("\r", "");
            Verein[] v = new Verein[anzahl_vereine];
            p.Clear();
            for (int i=0;i<anzahl_vereine;i++)
            {
                inhalt = ver.ReadLine(1 + i, false, meldung).Replace("\r", "");
                inhalt = clear(inhalt);
                hilf = inhalt.Split(';');
                v[i] = new Verein();
                v[i].name = hilf[0];
                v[i].index = i;
                v[i].team = new Team[team_max];
                try
                {
                    v[i].a = Data.toInt(hilf[1]);
                    v[i].b = Data.toInt(hilf[2]);
                    v[i].x = Data.toInt(hilf[3]);
                    v[i].y = Data.toInt(hilf[4]);
                    v[i].kapazitaet = hilf[5] == "X";
                    for (int j = 6; j < hilf.Length - 2; j += 3)
                        p.Add(new Partnerschaft(v[i], hilf[j].ToCharArray()[0], v[Data.toInt(hilf[j + 1])], hilf[j + 2].ToCharArray()[0]));
                }
                catch (Exception e) { }
            }
            return v;
        }

        public static Liga[] getStaffeln(Verein[] v, Textdatei sta)
        {
            Liga[] l;
            String inhalt = sta.ReadLine(1, false, meldung);
            if (meldung.Count > 0)
                return null;
            char[] split = { ';' };
            String[] hilf = inhalt.Split(split, StringSplitOptions.RemoveEmptyEntries);
            l = new Liga[hilf.Length];
            String[] hilf2;
            for (int i = 0; i < l.Length; i++)
            {
                l[i] = new Liga();
                hilf2 = hilf[i].Split('[');
                l[i].name = hilf2[0].TrimEnd(' ');
                l[i].index = i;
                if (hilf2.Length > 1)
                    l[i].feld = Data.toInt(hilf2[1]);
            }
            for (int i = 0; i < team_max; i++)
            {
                inhalt = sta.ReadLine(2 + i, false, meldung);
                if (meldung.Count > 0)
                    return l;
                hilf = inhalt.Split(';');
                for (int j = 0; j < hilf.Length; j++)
                    if (!hilf[j].Equals("")) 
                    {
                        hilf[j] = clear(hilf[j]);
                        if (hilf[j].Contains("Dummy"))
                            continue;
                        l[j].team[i] = new Team();
                        l[j].anzahl_teams++;
                        hilf2 = hilf[j].Split('[');
                        l[j].team[i].name = hilf2[0].TrimEnd(' ');
                        if (hilf2.Length > 1)
                            l[j].team[i].zahl = toInt(hilf2[1]);
                        l[j].team[i].liga = l[j];
                        l[j].team[i].index = i;
                        for (int k = 0; k < v.Length; k++) 
                            if (checkVerein(l[j].team[i], v[k])) 
                                l[j].team[i].verein = v[k];
                        if (l[j].team[i].verein == null)
                        {
                            l[j].team[i].verein = new Verein();
                            l[j].team[i].verein.name = l[j].team[i].name;
                            l[j].team[i].verein.index = -1;
                            l[j].team[i].verein.team = new Team[team_max];
                            l[j].team[i].woche = '-';
                        }
                    }
            }
            return l;
        }

        public static void fillArray()
        {
            // Gegenläufig
            String inhalt = gegenlaeufig.ReadFile(true, meldung);
            String[] hilf;
            String[] zeile;
            char[] split = {'\n'};
            zeile = inhalt.Split(split, StringSplitOptions.RemoveEmptyEntries);
            feld_g = new int[zeile.Length,5];
            for (int i = 0; i < zeile.Length; i++)
            {
                hilf = zeile[i].Split(';');
                for (int j = 0; j < hilf.Length && j < 5; j++)
                    feld_g[i, j] = toInt(hilf[j]);
            }
            // Parallel
            inhalt = parallel.ReadFile(true, meldung);
            zeile = inhalt.Split(split, StringSplitOptions.RemoveEmptyEntries);
            feld_p = new int[zeile.Length, 4];
            for (int i = 0; i < zeile.Length; i++)
            {
                hilf = zeile[i].Split(';');
                for (int j = 0; j < hilf.Length && j < 4; j++)
                    feld_p[i, j] = toInt(hilf[j]);
            }
            // Vereinsintern - multipel
            /*inhalt = vereinsintern.ReadFile(false);
            zeile = inhalt.Split(split, StringSplitOptions.RemoveEmptyEntries);
            feld_v = new int[zeile.Length, 5];
            for (int i = 0; i < zeile.Length; i++)
            {
                hilf = zeile[i].Split(';');
                for (int j = 0; j < hilf.Length && j < 5; j++)
                    feld_v[i, j] = toInt(hilf[j]);
            }*/
            // Ähnliche Zahlen
            inhalt = aehnlich.ReadFile(true, meldung);
            zeile = inhalt.Split(split, StringSplitOptions.RemoveEmptyEntries);
            feld_a = new int[zeile.Length, 4];
            for (int i = 0; i < zeile.Length; i++)
            {
                hilf = zeile[i].Split(';');
                for (int j = 0; j < hilf.Length && j < 4; j++)
                    feld_a[i, j] = toInt(hilf[j]);
            }
        }

        public static int anzahlZahlen(Liga[] l)
        {
            int zahlen = 0;
            int teams = 0;

            for (int i = 0; i < l.Length; i++)
                for (int j = 0; j < l[i].anzahl_teams; j++)
                {
                    if (l[i].team[j].zahl != 0)
                        zahlen++;
                    teams++;
                }
            return zahlen;
        }

        public static int durchlauf()
        {
            for (int i = 0; i < liga.Length; i++)
                for (int j = 0; j < liga[i].anzahl_teams; j++)
                    {
                        if (liga[i].team[j].zahl == 0)
                            liga[i].team[j].getZahl();
                        if (liga[i].team[j].zahl != 0)
                            liga[i].removeOptions(liga[i].team[j].zahl);
                        liga[i].checkOneOption();
                    }
            return anzahlZahlen(liga);
        }

        public static bool fertig()
        {
            for (int i = 0; i < liga.Length; i++)
                for (int j = 0; j < liga[i].anzahl_teams; j++)
                    if (liga[i].team[j].zahl == 0)
                        return false;
            return true;
        }

        public static bool fehlerDa()
        {
            bool hilf;
            for (int i = 0; i < liga.Length; i++)
                for (int j = 0; j < liga[i].anzahl_teams; j++)
                {
                    hilf = false;
                    for (int k = 0; k < team_max; k++)
                        hilf = hilf || liga[i].team[j].option[k];
                    if (!hilf)
                    {
                        meldung.Add("Keine moegliche Schluesselzahl fuer " + liga[i].team[j].name + " (" + liga[i].name + ") gefunden!");
                        return true;
                    }
                }
            return false;
        }

        public static String clear(String s)
        {
            s = s.Replace("Ä", "Ae");
            s = s.Replace("Ö", "Oe");
            s = s.Replace("Ü", "Ue");
            s = s.Replace("ä", "ae");
            s = s.Replace("ö", "oe");
            s = s.Replace("ü", "ue");
            s = s.Replace("ß", "ss");
            return s;
        }

        public static void speichern(Liga[] l, Verein[] v, List<Partnerschaft> p, Textdatei ver, Textdatei sta, Textdatei bez)
        {
            String hilf;
            int zeile = 0;
            bez.WriteFile("", meldung);
            for (int i=0; i<l.Length; i++)
                for (int j = 0; j < l[i].anzahl_teams; j++)
                {
                    zeile++;
                    hilf = "";
                    hilf += i + ";" + j + ";" + l[i].team[j].woche + ";";
                    for (int k = 0; k < l[i].team[j].spieltag.Length; k++)
                        hilf += l[i].team[j].spieltag[k] + ";";
                    hilf += "\n";
                    bez.Append(hilf, meldung);
                }
            ver.WriteFile("", meldung);
            for (int i = 0; i < v.Length; i++)
            {
                hilf = v[i].name + ";" + v[i].a + ";" + v[i].b + ";" + v[i].x + ";" + v[i].y + ";";
                if (v[i].kapazitaet)
                    hilf += "X;";
                else
                    hilf += ";";
                foreach (Partnerschaft pt in p)
                    if (pt.a == v[i])
                        hilf += pt.woche_a + ";" + pt.b.index + ";" + pt.woche_b + ";";
                hilf += "\n";
                ver.Append(hilf, meldung);
            }
            sta.WriteFile("", meldung);
            String inhalt = "";
            for (int i = 0; i < l.Length; i++)
                inhalt += l[i].name + " [" + l[i].feld + "];";
            inhalt += "\n";
            sta.Append(inhalt, meldung);
            for (int i = 0; i < team_max; i++)
            {
                inhalt = "";
                for (int j = 0; j < l.Length; j++)
                    if (l[j].team[i] != null && l[j].team[i].zahl != 0)
                        inhalt += l[j].team[i].name + " [" + l[j].team[i].zahl + "];";
                    else if (l[j].team[i] != null && l[j].team[i].zahl == 0)
                        inhalt += l[j].team[i].name + ";";
                    else
                        inhalt += ";";
                inhalt += "\n";
                sta.Append(inhalt, meldung);
            }
        }

        public static void getBeziehungen(Liga[] l, Textdatei bez)
        {
            String inhalt;
            String[] hilf;
            String[] zeile;
            inhalt = bez.ReadFile(false, meldung);

            if (inhalt.Equals(""))
                return;
            zeile = inhalt.Split('\n');
            for (int i = 0; i < zeile.Length; i++)
            {
                zeile[i] = zeile[i].Replace("\r","");
                hilf = zeile[i].Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                try
                {
                    Team t = l[toInt(hilf[0])].team[toInt(hilf[1])];
                    t.woche = hilf[2].ToCharArray()[0];
                    for (int j = 3; j < hilf.Length && j < 16; j++)
                        t.spieltag[j - 3] = hilf[j].ToCharArray()[0];
                }
                catch (Exception e) { }
            }
        }

        public static void getSpielplan()
        {
            String inhalt;
            String[] hilf;
            String[] zeile;
            inhalt = spielplan.ReadFile(false, meldung);

            if (inhalt.Equals(""))
                return;
            zeile = inhalt.Split('\n');
            spielplan_1 = new char[team_max, team_max, team_max];
            for (int i = 0; i < zeile.Length; i++)
            {
                hilf = zeile[i].Split(';');
                for (int j=2; j<hilf.Length; j++)
                    try
                    {
                        spielplan_1[toInt(hilf[0]) - 1, toInt(hilf[1]) - 1, j - 2] = hilf[j].ToCharArray()[0];
                    }
                    catch (Exception e)
                    {
                    }
            }
        }

        public static void loese()
        {
            int alt = 0;
            int neu = durchlauf();
            while (alt != neu)
            {
                alt = neu;
                neu = durchlauf();
                if (fertig())
                    return;
            }
        }

        public static void generiereSchluesselzahlen()
        {
            setOptions();
            loese();
            bool fertig = Data.fertig();
            while (!fertig)
                for (int i = 0; i < liga.Length; i++)
                    for (int j = 0; j < liga[i].anzahl_teams; j++)
                        for (int k = 0; k < liga[i].feld; k++)
                            if (liga[i].team[j].option[k] && !fertig)
                            {
                                liga[i].team[j].zahl = k + 1;
                                for (int l = 0; l < liga[i].feld; l++)
                                    if (l != k)
                                        liga[i].team[j].option[l] = false;
                                for (int l = 0; l < liga[i].anzahl_teams; l++)
                                    if (liga[i].team[l] != liga[i].team[j])
                                        liga[i].team[l].option[k] = false;
                                loese();
                                fertig = Data.fertig();
                            }
            speichern(Data.liga, Data.verein, Data.partnerschaft, Data.vereine, Data.staffeln, Data.beziehungen);
            caller.initUI();
            if (fehlerDa())
                meldung.Add("Die Generierung ist aufgrund eines logischen Fehlers nicht moeglich!");
            else
                meldung.Add("Die Schluesselzahlen wurden erfolgreich generiert!");
        }

        public static void buildArray()
        {
            aehnlich_1 = new int[14, 14];
            aehnlich_2 = new int[14, 14];
            gegenlaeufig_1 = new int[14,14,14];
            parallel_1 = new int[14, 14, 14];
            for (int i = 0; i < feld_a.GetLength(0); i++)
            {
                aehnlich_1[feld_a[i, 0]-1, feld_a[i, 1]-1] = feld_a[i, 2];
                aehnlich_2[feld_a[i, 0]-1, feld_a[i, 1]-1] = feld_a[i, 3];
            }
            for (int i = 0; i < feld_g.GetLength(0); i++)
                if (feld_g[i, 0] != 0 && feld_g[i, 1] != 0 && feld_g[i, 2] != 0)
                    gegenlaeufig_1[feld_g[i, 0] - 1, feld_g[i, 1] - 1, feld_g[i, 2] - 1] = feld_g[i, 3];
            for (int i = 0; i < feld_p.GetLength(0); i++)
                if (feld_p[i, 0] != 0 && feld_p[i, 1] != 0 && feld_p[i, 2] != 0)
                    parallel_1[feld_p[i, 0] - 1, feld_p[i, 1] - 1, feld_p[i, 2] - 1] = feld_p[i, 3];
            for (int i = 6; i <= 14; i++)
                for (int j = 0; j < i; j++)
                    parallel_1[i - 1, i - 1, j] = j + 1;
        }

        public static bool checkFatal(Liga[] l, int[] konflikte)
        {
            int[] belegung;
            konflikte[0] = 0;
            for (int i = 0; i < l.Length; i++)
            {
                belegung = new int[l[i].feld];
                
                // Widerspricht die Zahl eines Teams den vorgegebenen Spieltagen?
                for (int j = 0; j < l[i].anzahl_teams; j++)
                    if (l[i].team[j].zahl > 0)
                    {
                        belegung[l[i].team[j].zahl - 1]++;
                        if (!l[i].team[j].zahlOK(l[i].team[j].zahl))
                            return true;
                    }
                
                // Treten unlösbare Konflikte auf?
                for (int j = 0; j < l[i].feld; )
                    if (belegung[j] > 3)
                        return true;
                    else if (belegung[j] > 1)
                    {
                        konflikte[0]++;
                        if (konflikte[1] != -1 && konflikte[0] >= konflikte[1])
                            return true;
                        for (int m = 0; m < l[i].anzahl_teams; m++)
                            for (int k = m + 1; k < l[i].anzahl_teams; k++)
                                if (l[i].team[m].zahl == j+1 && l[i].team[k].zahl == j+1 && l[i].team[m].verein != l[i].team[k].verein
                                 && l[i].team[m].verein.kapazitaet && l[i].team[k].verein.kapazitaet)
                                    return true;
                        if (aehnlich_1[l[i].feld - 1, j] != 0 && belegung[aehnlich_1[l[i].feld - 1, j] - 1] == 0)
                            belegung[aehnlich_1[l[i].feld - 1, j] - 1]++;
                        else if (aehnlich_2[l[i].feld - 1, j] != 0 && belegung[aehnlich_2[l[i].feld - 1, j] - 1] == 0)
                            belegung[aehnlich_2[l[i].feld - 1, j] - 1]++;
                        else
                            return true;
                        belegung[j]--;
                    }
                    else
                        j++;
                    if (konflikte[1] != -1 && konflikte[0] >= konflikte[1])
                        return true;
            }    
            return false;
        }

        public static void checkFatal(Liga[] l, List<string> meldung)
        {
            int[] belegung;
            for (int i = 0; i < l.Length; i++)
            {
                belegung = new int[l[i].feld];
                // Belegung berechnen
                for (int j = 0; j < l[i].anzahl_teams; j++)
                    if (l[i].team[j].zahl > 0)
                        belegung[l[i].team[j].zahl - 1]++;

                // Treten unlösbare Konflikte auf?
                for (int j = 0; j < l[i].feld; )
                    if (belegung[j] > 3)
                        meldung.Add("Die Schlüsselzahl " + (j++ + 1) + " wurde in der " + l[i] + " mehr als dreimal vergeben!");
                    else if (belegung[j] > 1)
                    {
                        for (int m = 0; m < l[i].anzahl_teams; m++)
                            for (int k = m + 1; k < l[i].anzahl_teams; k++)
                                if (l[i].team[m].zahl == j + 1 && l[i].team[k].zahl == j + 1 && l[i].team[m].verein != l[i].team[k].verein
                                 && l[i].team[m].verein.kapazitaet && l[i].team[k].verein.kapazitaet)
                                    meldung.Add("Konflikt zwischen " + l[i].team[m].name + " und " + l[i].team[k].name + " in der " + l[i].name + "!");
                        if (aehnlich_1[l[i].feld - 1, j] != 0 && belegung[aehnlich_1[l[i].feld - 1, j] - 1] == 0)
                            belegung[aehnlich_1[l[i].feld - 1, j] - 1]++;
                        else if (aehnlich_2[l[i].feld - 1, j] != 0 && belegung[aehnlich_2[l[i].feld - 1, j] - 1] == 0)
                            belegung[aehnlich_2[l[i].feld - 1, j] - 1]++;
                        else
                            meldung.Add("Unlösbarer Konflikt in der " + l[i].name + "(Schlüsselzahl " + (j + 1) + ")!");
                        belegung[j]--;
                    }
                    else
                        j++;
            }
        }

        public static void checkConflicts(Liga[] l, List<Konflikt> k)
        {
            int[] belegung;
            Konflikt konflikt;
            int team = 0;
            int zahl = 0;
            for (int i = 0; i < l.Length; i++)
            {
                belegung = new int[l[i].feld];
                for (int j = 0; j < l[i].anzahl_teams; j++)
                    if (l[i].team[j].zahl > 0)
                        belegung[l[i].team[j].zahl - 1]++;
                for (int j = 0; j < l[i].feld; j++)
                    if (belegung[j] > 1)
                    {
                        team = 0;
                        zahl = 0;
                        konflikt = new Konflikt();
                        konflikt.wunsch = j + 1;
                        konflikt.t = new Team[belegung[j]];
                        for (int x = 0; x < l[i].anzahl_teams; x++)
                            if (l[i].team[x].zahl == j + 1)
                                konflikt.t[team++] = l[i].team[x];
                        konflikt.zahl[zahl++] = j + 1;
                        if (belegung[aehnlich_1[l[i].feld - 1, j] - 1] == 0)
                            konflikt.zahl[zahl++] = aehnlich_1[l[i].feld - 1, j];
                        if (belegung[aehnlich_2[l[i].feld - 1, j] - 1] == 0)
                            konflikt.zahl[zahl++] = aehnlich_2[l[i].feld - 1, j];
                        if (konflikt.zahl[belegung[j] - 1] == 0)
                        {
                            konflikt.zahl[1] = aehnlich_1[l[i].feld - 1, j];
                            konflikt.zahl[2] = aehnlich_2[l[i].feld - 1, j];
                        }
                        k.Add(konflikt);
                    }
            } 
        }

        public static bool partnerOK(Verein[] ver, int v, int p, int x, int[] schluessel)
        {
            int zahl_a = 0;
            int zahl_b = 0;
            int feld_a = 0;
            int feld_b = 0;
            bool okay = true;

            if (x == 0)
            {
                ver[v].a = schluessel[p * 2];
                ver[v].b = gegenlaeufig_1[feld[0] - 1, feld[0] - 1, schluessel[p * 2] - 1];
            }
            else if (x == 1)
            {
                ver[v].x = schluessel[p * 2 + 1];
                ver[v].y = gegenlaeufig_1[feld[1] - 1, feld[1] - 1, schluessel[p * 2 + 1] - 1];
            }
            for (int i = 0; i < partnerschaft.Count; i++)
            {
                if (partnerschaft[i].a.index != ver[v].index && partnerschaft[i].b.index != ver[v].index)
                    continue;
                switch (partnerschaft[i].woche_a)
                {
                    case 'A': zahl_a = partnerschaft[i].a.a; feld_a = feld[0]; break;
                    case 'B': zahl_a = partnerschaft[i].a.b; feld_a = feld[0]; break;
                    case 'X': zahl_a = partnerschaft[i].a.x; feld_a = feld[1]; break;
                    case 'Y': zahl_a = partnerschaft[i].a.y; feld_a = feld[1]; break;
                }
                switch (partnerschaft[i].woche_b)
                {
                    case 'A': zahl_b = partnerschaft[i].b.a; feld_b = feld[0]; break;
                    case 'B': zahl_b = partnerschaft[i].b.b; feld_b = feld[0]; break;
                    case 'X': zahl_b = partnerschaft[i].b.x; feld_b = feld[1]; break;
                    case 'Y': zahl_b = partnerschaft[i].b.y; feld_b = feld[1]; break;
                }

                if (zahl_a == 0 || zahl_b == 0)
                    continue;
                if (parallel_1[feld_a - 1, feld_b - 1, zahl_a - 1] == zahl_b)
                    continue;
                okay = false;
                break;
            }
            if (x == 0)
                ver[v].a = ver[v].b = 0;
            else if (x == 1)
                ver[v].x = ver[v].y = 0;
            return okay;
        }

        public static void setZusatz(Liga[] l, Liga[] best_l, Verein[] ver, Verein[] best_ver, int[] kon, int[] schluessel)
        {
            string value;

            for (int i = 0; i < l.Length; i++)
                for (int j = 0; j < l[i].anzahl_teams; j++)
                    if (l[i].team[j].zahl == 0 && l[i].team[j].woche == '-' && l[i].team[j].zusatz())
                    {
                        for (int k = 0; k < l[i].feld; k++)
                            if (l[i].team[j].zahlOK(k + 1))
                            {
                                l[i].team[j].zahl = k + 1;
                                if (checkFatal(l, kon))
                                    kon[0] = -1;
                                else
                                    setZusatz(l, best_l, ver, best_ver, kon, schluessel);
                            }
                        l[i].team[j].zahl = 0;
                        kon[0] = -1;
                        return;
                    }
            if (kon[1] == kon[0])
                return;
            kon[1] = kon[0];
            copy(l, best_l, ver, best_ver, Data.partnerschaft, Data.partnerschaft);
            value = getValue(schluessel);
            if (!ht.Contains(value))
                ht.Add(value, value);
        }

        public static void findSolution(int p, Liga[] l, Liga[] best_l, Verein[] ver, Verein[] best_ver, int[] kon, int[] schluessel)
        {
            int[] neu = { 0, 0 };
            bool[] gesetzt = new bool[2];
            string value;

            if (p < ver.Length)
            {
                int v = prio[p];

                if (ver[v].prio == 0)
                    setZusatz(l, best_l, ver, best_ver, kon, schluessel);
                else
                {
                    for (int j = 0; j < ver[v].team.Length; j++)
                        if (ver[v].team[j].verein.index == ver[v].index)
                            switch (ver[v].team[j].woche)
                            {
                                case 'A': neu[0]++; break;
                                case 'B': neu[0]++; break;
                                case 'X': neu[1]++; break;
                                case 'Y': neu[1]++; break;
                            }
                    gesetzt[0] = ver[v].a != 0 || neu[0] == 0;
                    gesetzt[1] = ver[v].x != 0 || neu[1] == 0;
                    for (int x = 0; x < 3; x++)
                    {
                        if (x == 2)
                        {
                            findSolution(p + 1, l, best_l, ver, best_ver, kon, schluessel);
                            break;
                        }
                        if (gesetzt[x])
                            continue;
                        rand[x] %= feld[x];
                        schluessel[p * 2 + x] = ++rand[x];
                        value = getValue(schluessel);
                        if (!partnerOK(ver, v, p, x, schluessel))
                            if (!ht.Contains(value))
                                ht.Add(value, value);
                        for (int j = 0; ht.Contains(value); j++)
                        {
                            if (j == feld[x])
                            {
                                kon[0] = -1;
                                fillHashTable(schluessel, p * 2 + x, feld[x]);
                                x = 3;
                                break;
                            }
                            rand[x] %= feld[x];
                            schluessel[p * 2 + x] = ++rand[x];
                            value = getValue(schluessel);
                            if (!partnerOK(ver, v, p, x, schluessel))
                                if (!ht.Contains(value))
                                    ht.Add(value, value);
                        }

                        if (x == 0)
                        {
                            ver[v].a = schluessel[p * 2];
                            ver[v].b = gegenlaeufig_1[feld[0] - 1, feld[0] - 1, schluessel[p * 2] - 1];
                            for (int k = 0; k < ver[v].team.Length; k++)
                                if (ver[v].team[k].woche == 'A')
                                {
                                    if (ver[v].team[k].liga.feld == feld[0])
                                        ver[v].team[k].zahl = ver[v].a;
                                    else
                                        ver[v].team[k].zahl = parallel_1[feld[0] - 1, ver[v].team[k].liga.feld - 1, ver[v].a - 1];
                                }
                                else if (ver[v].team[k].woche == 'B')
                                {
                                    if (ver[v].team[k].liga.feld == feld[0])
                                        ver[v].team[k].zahl = ver[v].b;
                                    else
                                        ver[v].team[k].zahl = parallel_1[feld[0] - 1, ver[v].team[k].liga.feld - 1, ver[v].b - 1];
                                }
                        }
                        else if (x == 1)
                        {
                            ver[v].x = schluessel[p * 2 + 1];
                            ver[v].y = gegenlaeufig_1[feld[1] - 1, feld[1] - 1, schluessel[p * 2 + 1] - 1];
                            for (int k = 0; k < ver[v].team.Length; k++)
                                if (ver[v].team[k].woche == 'X')
                                {
                                    if (ver[v].team[k].liga.feld == feld[1])
                                        ver[v].team[k].zahl = ver[v].x;
                                    else
                                        ver[v].team[k].zahl = parallel_1[feld[1] - 1, ver[v].team[k].liga.feld - 1, ver[v].x - 1];
                                }
                                else if (ver[v].team[k].woche == 'Y')
                                {
                                    if (ver[v].team[k].liga.feld == feld[1])
                                        ver[v].team[k].zahl = ver[v].y;
                                    else
                                        ver[v].team[k].zahl = parallel_1[feld[1] - 1, ver[v].team[k].liga.feld - 1, ver[v].y - 1];
                                }
                        }
                        else if (x == 3)
                            break;

                        if (checkFatal(l, kon))
                        {
                            kon[0] = -1;
                            value = getValue(schluessel);
                            ht.Add(value, value);
                            break;
                        }
                    }

                    if (!gesetzt[0])
                    {
                        ver[v].a = 0;
                        ver[v].b = 0;
                        for (int k = 0; k < ver[v].team.Length; k++)
                            if (ver[v].team[k].woche == 'A')
                                ver[v].team[k].zahl = 0;
                            else if (ver[v].team[k].woche == 'B')
                                ver[v].team[k].zahl = 0;
                    }
                    if (!gesetzt[1])
                    {
                        ver[v].x = 0;
                        ver[v].y = 0;
                        for (int k = 0; k < ver[v].team.Length; k++)
                            if (ver[v].team[k].woche == 'X')
                                ver[v].team[k].zahl = 0;
                            else if (ver[v].team[k].woche == 'Y')
                                ver[v].team[k].zahl = 0;
                    }
                    kon[0] = -1;
                    schluessel[p * 2] = 0;
                    schluessel[p * 2 + 1] = 0;
                }
            }
            else
                setZusatz(l, best_l, ver, best_ver, kon, schluessel);
        }

        public static void fillHashTable(int[] schluessel, int pos, int feld)
        {
            string value;
            for (int i = 1; i <= feld; i++)
            {
                schluessel[pos] = i;
                value = getValue(schluessel);
                ht.Remove(value);
            }
            schluessel[pos] = 0;
            value = getValue(schluessel);
            ht.Add(value, value);
        }

        public static void copyKeys()
        {
            for (int i = 0; i < verein.Length; i++) {
                for (int k = 0; k < verein[i].team.Length; k++)
                    if (verein[i].team[k].zahl == 0)
                        switch (verein[i].team[k].woche)
                        {
                            case 'A':
                                if (verein[i].team[k].liga.feld == feld[0])
                                    verein[i].team[k].zahl = verein[i].a;
                                else if (verein[i].a > 0)
                                    verein[i].team[k].zahl = parallel_1[feld[0] - 1, verein[i].team[k].liga.feld - 1, verein[i].a - 1];
                                break;
                            case 'B':
                                if (verein[i].team[k].liga.feld == feld[0])
                                    verein[i].team[k].zahl = verein[i].b;
                                else if (verein[i].b > 0)
                                    verein[i].team[k].zahl = parallel_1[feld[0] - 1, verein[i].team[k].liga.feld - 1, verein[i].b - 1];
                                break;
                            case 'X':
                                if (verein[i].team[k].liga.feld == feld[1])
                                    verein[i].team[k].zahl = verein[i].x;
                                else if (verein[i].x > 0)
                                    verein[i].team[k].zahl = parallel_1[feld[1] - 1, verein[i].team[k].liga.feld - 1, verein[i].x - 1];
                                break;
                            case 'Y':
                                if (verein[i].team[k].liga.feld == feld[1])
                                    verein[i].team[k].zahl = verein[i].y;
                                else if (verein[i].y > 0)
                                    verein[i].team[k].zahl = parallel_1[feld[1] - 1, verein[i].team[k].liga.feld - 1, verein[i].y - 1];
                                break;
                            case '-':
                                verein[i].team[k].zahl = 0;
                                break;
                        }
                //setPartner(verein, i);
            }
        }

        public static void allocateTeams(Verein[] v, Liga[] l)
        {
            if (v == null || l == null)
                return;
            List<Team> al;
            for (int i = 0; i < v.Length; i++)
            {
                al = new List<Team>();
                for (int j = 0; j < l.Length; j++)
                    for (int k = 0; k < l[j].anzahl_teams; k++)
                        if (l[j].team[k].verein.index == v[i].index)
                            al.Add(l[j].team[k]);
                v[i].team = al.ToArray();
            }
        }

        public static void createPriority()
        {
            prio = new int[verein.Length];
            for (int i = 0; i < verein.Length; i++)
                verein[i].setPrio(partnerschaft);
            int counter = 0;
            int max_prio = 0;
            while (counter < verein.Length)
            {
                max_prio = 0;
                for (int i = 0; i < verein.Length; i++)
                    if (verein[i].prio > max_prio && (counter == 0 || verein[i].prio < verein[prio[counter - 1]].prio))
                        max_prio = verein[i].prio;
                for (int i = 0; i < verein.Length; i++)
                    if (verein[i].prio == max_prio)
                        prio[counter++] = verein[i].index;
            }
        }

        public static bool checkVerein(Team t, Verein v)
        {
            string team = t.name;
            string verein = v.name;
            t.team = "";
            try
            {
                string[] wort = team.Split(' ');
                string endung = wort[wort.Length - 1];
                string pattern = "IVX";
                bool team_endung = true;
                foreach (char c in endung)
                    if (pattern.IndexOf(c) < 0)
                        team_endung = false;
                if (team_endung)
                {
                    t.team = endung;
                    team = team.Remove(team.Length - endung.Length - 1);
                }

                char[] zeichen = team.ToCharArray();
                for (int i = 0; i < zeichen.Length; i++)
                    if (!verein.Contains(zeichen[i]))
                        return false;
                    else
                    {
                        if (verein.IndexOf(zeichen[i]) != 0)
                            if (Char.IsLetter(zeichen[i]) && Char.IsLetter(verein.ToCharArray()[verein.IndexOf(zeichen[i]) - 1]))
                                return false;
                        verein = verein.Substring(verein.IndexOf(zeichen[i]) + 1);
                    }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static void checkPlausibility(Liga[] liga, Form caller, List<string> m)
        {
            bool plausible;
            for (int i=0; i<liga.Length; i++)
                for (int j = 0; j < liga[i].anzahl_teams; j++)
                {
                    plausible = false;
                    if (liga[i].team[j].zahl == 0)
                        for (int k = 0; k < liga[i].feld; k++)
                            plausible |= liga[i].team[j].zahlOK(k + 1);
                    else
                        plausible = liga[i].team[j].zahlOK(liga[i].team[j].zahl);
                    if (!plausible)
                        m.Add("Der Spielplan für " + liga[i].team[j].name + " in der " + liga[i].name + " ist inkonsistent!");
                }
            if (m.Count > 0)
                return;
            for (int i = 0; i < verein.Length; i++)
                for (int j = 0; j < verein[i].team.Length; j++)
                    if (verein[i].team[j].woche != '-' && verein[i].team[j].zusatz())
                        for (int k = j + 1; k < verein[i].team.Length; k++)
                            if (verein[i].team[k].woche != '-' && verein[i].team[k].zusatz())
                                if (verein[i].team[j].woche == verein[i].team[k].woche)
                                {
                                    for (int l = 0; l < team_max; l++)
                                        if (verein[i].team[j].spieltag[l] != verein[i].team[k].spieltag[l])
                                            m.Add("Spielplan und Spielwochen für den Verein " + verein[i].name + " sind inkonsitstent!");
                                }
                                else if (verein[i].team[j].woche == 'A' && verein[i].team[k].woche == 'B'
                                      || verein[i].team[j].woche == 'B' && verein[i].team[k].woche == 'A'
                                      || verein[i].team[j].woche == 'X' && verein[i].team[k].woche == 'Y'
                                      || verein[i].team[j].woche == 'Y' && verein[i].team[k].woche == 'X')
                                    for (int l = 0; l < team_max; l++)
                                        if (verein[i].team[j].spieltag[l] == verein[i].team[k].spieltag[l] && verein[i].team[j].spieltag[l] != '-')
                                            m.Add("Spielplan und Spielwochen für den Verein " + verein[i].name + " sind inkonsitstent!");
        }

        public static string getValue(int[] schluessel)
        {
            string result = "";
            for (int i = 0; i < schluessel.Length; i++)
            {
                if (schluessel[i] == 0)
                    continue;
                if (schluessel[i] < 10)
                    result += " ";
                result += schluessel[i];
            }
            return result;
        }
    }
}