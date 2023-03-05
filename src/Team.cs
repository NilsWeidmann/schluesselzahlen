using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Text;

namespace Schluesselzahlen
{
    public class Team
    {
        public String name;
        public Liga liga;
        public Verein verein;
        public String team;
        public int zahl = 0;
        public int index;
        public bool[] option;
        public int anzahl_optionen;
        public char woche = '-';
        public char[] spieltag;

        public Team()
        {
            option = new bool[Data.team_max];
            spieltag = new char[Data.team_max];
            for (int i = 0; i < Data.team_max; i++)
                spieltag[i] = '-';
        }

        public void setAnzahl_optionen()
        {
            anzahl_optionen = 0;
            for (int k = 0; k < Data.team_max; k++)
                if (option[k])
                    anzahl_optionen++;
        }

        public void getZahl()
        {
            int zahl = 0;
            for (int i = 0; i < Data.team_max; i++)
                if (option[i])
                    if (zahl == 0)
                        zahl = i + 1;
                    else
                        return;
            this.zahl = zahl;
        }

        public bool zusatz()
        {
            for (int i = 0; i < spieltag.Length; i++)
                if (spieltag[i] == 'H' || spieltag[i] == 'A')
                    return true;
            return false;
        }

        public bool zahlOK(int zahl)
        {
            if (zahl == 0)
                return true;
            for (int i = 0; i < spieltag.Length; i++)
                if (Data.spielplan_1[liga.feld - 1, zahl - 1, i] == 'H' && spieltag[i] == 'A'
                ||  Data.spielplan_1[liga.feld - 1, zahl - 1, i] == 'A' && spieltag[i] == 'H')
                    return false;
            return true;
        }
    }
}
