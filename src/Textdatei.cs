using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Schluesselzahlen
{
    public class Textdatei
    {
        public string sFilename;
        private String fehler;

        public Textdatei(string x)
        {
            sFilename = x;
            fehler = "Fehler beim Lesen der Datei " + sFilename + "!";
        }

        public string ReadFile(bool mustExist, List<string> m) /// Liefert den Inhalt der Datei zurück.
        {
            string sContent = "";
            try
            {
                if (File.Exists(sFilename))
                {
                    StreamReader myFile = new StreamReader(sFilename, System.Text.Encoding.Default);
                    sContent = myFile.ReadToEnd();
                    myFile.Close();
                }
                else
                {
                    if (mustExist)
                    {
                        m.Add("Die Datei " + sFilename + " existiert nicht!");
                    }
                    else
                        WriteFile("", m);
                }
            }
            catch (Exception e)
            {
                m.Add(fehler);
            }
            return sContent;
        }

        public void WriteFile(String sLines, List<string> m) /// Schreibt den übergebenen Inhalt in eine Textdatei.
        {
            try
            {
                StreamWriter myFile = new StreamWriter(sFilename);
                myFile.Write(sLines);
                myFile.Close();
            }
            catch (Exception e)
            {
                m.Add(fehler);
            }
        }

        public void Append(string sLines, List<string> m) /// Fügt den übergebenen Text an das Ende einer Textdatei an.
        {
            try
            {
                StreamWriter myFile = new StreamWriter(sFilename, true);
                myFile.Write(sLines);
                myFile.Close();
            }
            catch (Exception e)
            {
                m.Add(fehler);
            }
        }

        public string ReadLine(int iLine, bool mustExist, List<string> m) /// Liefert den Inhalt der übergebenen Zeilennummer zurück.
        {
            string sContent = "";
            float fRow = 0;
            try
            {
                if (File.Exists(sFilename))
                {
                    StreamReader myFile = new StreamReader(sFilename, System.Text.Encoding.Default);
                    while (!myFile.EndOfStream && fRow++ < iLine)
                        sContent = myFile.ReadLine();
                    myFile.Close();
                    if (fRow < iLine)
                        sContent = "";
                }
                else
                {
                    if (mustExist)
                    {
                        m.Add("Die Datei " + sFilename + " existiert nicht!");
                    }
                    else
                        WriteFile("", m);
                }
            }
            catch (Exception e)
            {
                m.Add(fehler);
            }
            return sContent;
        }

        public void WriteLine(int iLine, string sLines, bool bReplace, List<string> m) /// Schreibt den übergebenen Text in eine definierte Zeile.
        {
            string sContent = "";
            string[] delimiterstring = { "\r\n" };
            try
            {
                if (File.Exists(sFilename))
                {
                    StreamReader myFile = new StreamReader(sFilename, System.Text.Encoding.Default);
                    sContent = myFile.ReadToEnd();
                    myFile.Close();
                }
                string[] sCols = sContent.Split(delimiterstring, StringSplitOptions.None);
                if (sCols.Length >= iLine)
                {
                    if (!bReplace)
                        sCols[iLine - 1] = sLines + "\r\n" + sCols[iLine - 1];
                    else
                        sCols[iLine - 1] = sLines;
                    sContent = "";
                    for (int x = 0; x < sCols.Length - 1; x++)
                        sContent += sCols[x] + "\r\n";
                    sContent += sCols[sCols.Length - 1];
                }
                else
                {
                    for (int x = 0; x < iLine - sCols.Length; x++)
                        sContent += "\r\n";
                    sContent += sLines;
                }

                StreamWriter mySaveFile = new StreamWriter(sFilename);
                mySaveFile.Write(sContent);
                mySaveFile.Close();
            }
            catch (Exception e)
            {
                m.Add(fehler);
            }
        }
    }
}
