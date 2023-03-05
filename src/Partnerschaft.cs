using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Schluesselzahlen
{
    public class Partnerschaft
    {
        public Verein a;
        public char woche_a;
        public Verein b;
        public char woche_b;

        public Partnerschaft(Verein a, string woche_a, string name_b, string woche_b, Verein[] v_list)
        {
            Verein b = null;
            for (int i = 0; i < v_list.Length; i++)
                if (v_list[i].name == name_b)
                    b = v_list[i];
            if (a.index > b.index)
            {
                this.a = a;
                this.b = b;
                this.woche_a = woche_a.ToCharArray()[0];
                this.woche_b = woche_b.ToCharArray()[0];
            }
            else
            {
                this.a = b;
                this.b = a;
                this.woche_a = woche_b.ToCharArray()[0];
                this.woche_b = woche_a.ToCharArray()[0];
            }
        }

        public Partnerschaft(Verein a, char woche_a, Verein b, char woche_b)
        {
            if (a.index > b.index)
            {
                this.a = a;
                this.woche_a = woche_a;
                this.b = b;
                this.woche_b = woche_b;
            }
            else
            {
                this.a = b;
                this.woche_a = woche_b;
                this.b = a;
                this.woche_b = woche_a;
            }
        }
    }
}
