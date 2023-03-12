using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Schluesselzahlen
{
    public class NumberMapper
    {
        private int[,] aehnlich_1;
        private int[,] aehnlich_2;
        private int[,,] gegenlaeufig_1;
        private int[,,] parallel_1;


        public NumberMapper(String path) {
            
            // Gegenläufig
            Textdatei gegenlaeufig = new Textdatei(path + @"\Gegenlaeufig.csv");
            int[,] feld_g = getField(gegenlaeufig, 5);
            gegenlaeufig_1 = new int[Data.team_max, Data.team_max, Data.team_max];
            for (int i = 0; i < feld_g.GetLength(0); i++)
                if (feld_g[i, 0] != 0 && feld_g[i, 1] != 0 && feld_g[i, 2] != 0)
                    gegenlaeufig_1[feld_g[i, 0] - 1, feld_g[i, 1] - 1, feld_g[i, 2] - 1] = feld_g[i, 3];

            // Parallel
            Textdatei parallel = new Textdatei(path + @"\Parallel.csv");
            int[,] feld_p = getField(parallel, 4);
            parallel_1 = new int[Data.team_max, Data.team_max, Data.team_max];
            for (int i = 0; i < feld_p.GetLength(0); i++)
                if (feld_p[i, 0] != 0 && feld_p[i, 1] != 0 && feld_p[i, 2] != 0)
                    parallel_1[feld_p[i, 0] - 1, feld_p[i, 1] - 1, feld_p[i, 2] - 1] = feld_p[i, 3];
            for (int i = Data.team_min; i <= Data.team_max; i++)
                for (int j = 0; j < i; j++)
                    parallel_1[i - 1, i - 1, j] = j + 1;

            // Vereinsintern - multipel
            Textdatei vereinsintern = new Textdatei(path + @"\Vereinsintern.csv");
            //int[,] feld_v = getField(vereinsintern, 5);
            
            // Ähnliche Zahlen
            Textdatei aehnlich = new Textdatei(path + @"\Aehnlich.csv");
            int[,] feld_a = getField(aehnlich, 4);
            aehnlich_1 = new int[Data.team_max, Data.team_max];
            aehnlich_2 = new int[Data.team_max, Data.team_max];
            for (int i = 0; i < feld_a.GetLength(0); i++)
            {
                aehnlich_1[feld_a[i, 0] - 1, feld_a[i, 1] - 1] = feld_a[i, 2];
                aehnlich_2[feld_a[i, 0] - 1, feld_a[i, 1] - 1] = feld_a[i, 3];
            }
        }

        private int[,] getField(Textdatei file, int cols)
        {
            String inhalt = file.ReadFile(true, Data.meldung);
            String[] hilf;
            String[] zeile;
            char[] split = { '\n' };
            zeile = inhalt.Split(split, StringSplitOptions.RemoveEmptyEntries);
            int[,] field = new int[zeile.Length, cols];
            for (int i = 0; i < zeile.Length; i++)
            {
                hilf = zeile[i].Split(';');
                for (int j = 0; j < hilf.Length && j < cols; j++)
                    field[i, j] = Util.toInt(hilf[j]);
            }
            return field;
        }

        public int getParallel(int fieldFrom, int fieldTo, int numberFrom)
        {
            return parallel_1[fieldFrom - 1, fieldTo - 1, numberFrom - 1];
        }

        public int getGegenlaeufig(int fieldFrom, int fieldTo, int numberFrom)
        {
            return gegenlaeufig_1[fieldFrom - 1, fieldTo - 1, numberFrom - 1];
        }

        public int[] getAehnlich(int field, int number)
        {
            return new int[] { aehnlich_1[field - 1, number - 1], aehnlich_2[field - 1, number - 1] };
        }
    }
}
