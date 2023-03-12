using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Schluesselzahlen
{
    class UnitTests
    {
        public bool allTeamsHaveDifferentNumbers(Liga[] l)
        {
            foreach (Liga league in l)
                for (int i = 0; i < league.team.Length; i++)
                    for (int j = i + 1; j < league.team.Length; j++)
                        if (league.team[i].zahl == league.team[j].zahl)
                            return false;
            return true;
        }

        public bool allTeamsHaveValidNumbers(Liga[] l)
        {
            foreach (Liga league in l)
                for (int i = 0; i < league.team.Length; i++)
                    if (league.team[i].zahl < 1 || league.team[i].zahl > league.feld)
                        return false;
            return true;
        }

        public bool checkValidity(Verein[] v)
        {
            return true;
        }
    }
}
