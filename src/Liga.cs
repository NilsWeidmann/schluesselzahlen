using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace Schluesselzahlen
{
    public class Liga
    {
        public String name;
        public Team[] team = new Team[Data.team_max];
        public int index;
        public int anzahl_teams = 0;
        public int feld = 0;

        public void removeOptions(int zahl)
        {
            for (int i = 0; i < anzahl_teams; i++)
                if (team[i].zahl != zahl)
                    team[i].option[zahl - 1] = false;
                else
                    for (int j = 0; j < Data.team_max; j++)
                        if (j != zahl - 1)
                            team[i].option[j] = false;
        }

        public void checkOneOption()
        {
            int t = -1;
            for (int i = 0; i < Data.team_max; i++)
            {
                t = -1;
                for (int j = 0; j < anzahl_teams; j++)
                    if (team[j].option[i])
                        if (t == -1)
                            t = j;
                        else
                        {
                            t = -1;
                            break;
                        }
                if (t >= 0 && team[t].zahl == 0)
                {
                    team[t].zahl = i + 1;
                    removeOptions(i + 1);
                    checkOneOption();
                }
            }
        }
    }  
}
