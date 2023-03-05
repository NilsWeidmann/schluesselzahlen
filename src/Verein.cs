using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Schluesselzahlen
{
    public class Verein
    {
        public String name;
        public int index;
        public int a;
        public int b;
        public int x;
        public int y;
        public Team[] team;
        public int prio;
        public bool kapazitaet;

        public void setPrio(List<Partnerschaft> partnerschaft)
        {
            prio = 0;
            for (int i = 0; i < team.Length; i++)
                if (team[i].woche != '-' && team[i].zahl == 0)
                    prio++;
            foreach (Partnerschaft p in partnerschaft)
                if (p.a.index == index)
                {
                    for (int i = 0; i < p.b.team.Length; i++)
                        if (p.b.team[i].woche != '-' && p.b.team[i].zahl == 0)
                            prio++;
                }
                else if (p.b.index == index)
                {
                    for (int j = 0; j < p.a.team.Length; j++)
                        if (p.a.team[j].woche != '-' && p.a.team[j].zahl == 0)
                            prio++;
                }
        }

        public Verein clone()
        {
            Verein v = new Verein();
            v.name = name;
            v.index = index;
            v.a = a;
            v.b = b;
            v.x = x;
            v.y = y;
            v.team = new Team[team.Length];
            v.kapazitaet = kapazitaet;
            return v;
        }
    }
}
