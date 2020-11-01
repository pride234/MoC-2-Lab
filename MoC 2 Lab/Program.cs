using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;

namespace MoC_2_Lab {
    class Program {
        static void Main(string[] args) {

            int alphabetlen = 33;

            StreamReader origin = new StreamReader(@"C:\Users\PRIDE\source\repos\MoC 2 Lab\text.txt");

            string text = origin.ReadToEnd();
            //string curLine = "";
            //string text = "";
            //while ((curLine = cp.ReadLine()) != null) text += curLine;
            origin.Close();

            text = text.Replace("ґ", "г");
            text = text.ToLower();

            Regex rgx = new Regex("[^а-щьюяяіїє]");
            text = rgx.Replace(text, "");
            //Console.WriteLine(text);

            StreamWriter form = new StreamWriter(@"C:\Users\PRIDE\source\repos\MoC 2 Lab\text1.txt");

            form.Write(text);
            form.Close();

            Dictionary<char, int> char_freq = new Dictionary<char, int>(alphabetlen);

            for (int i = 0; i < text.Length; i++) {

                if (char_freq.ContainsKey(text[i])) {
                    char_freq[text[i]] += 1; 
                    continue; 
                }

                char_freq.Add(text[i], 1);
            }

            Dictionary<string, int> bigrams = new Dictionary<string, int>(alphabetlen * alphabetlen);

            for (int i = 0; i < text.Length-1; i++) {

                if (bigrams.ContainsKey(text.Substring(i,2))) {
                    bigrams[text.Substring(i, 2)] += 1;
                    continue;
                }

                bigrams.Add(text.Substring(i, 2), 1);
            }

            double H_char = 0;
            double L_char = 0;

            foreach (KeyValuePair<char, int> a in char_freq) { 
                double p = (double)a.Value / text.Length;
                H_char -= p*Math.Log(p,2);
                L_char += a.Value * (a.Value - 1);
            }

            L_char /= (text.Length * (text.Length - 1));

            double H_bigrams = 0;
            double L_bigrams = 0;

            foreach (KeyValuePair<string, int> a in bigrams) {
                double p = (double)a.Value / text.Length;
                H_bigrams -= p * Math.Log(p, 2);
                L_bigrams += a.Value * (a.Value - 1);
            }

            L_bigrams /= (text.Length * (text.Length - 1));
            L_bigrams *= 4;
        }
    }
}
