using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;

namespace MoC_2_Lab {

    class Program {

        static char[] alphabet = new[] { 'а', 'б', 'в', 'г', 'д', 'е', 'є', 'ж', 'з', 'и', 'і', 'ї', 'й', 'к', 'л', 'м', 'н', 'о', 'п', 'р', 'с', 'т', 'у', 'ф', 'х', 'ц', 'ч', 'ш', 'щ', 'ь', 'ю', 'я' };

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~||
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

            for (int i = 0; i < alphabet.Length; i++)
                char_freq.Add(alphabet[i], 0);

            for (int i = 0; i < text.Length; i++)
                char_freq[text[i]] += 1; 

            Dictionary<string, int> bigrams = new Dictionary<string, int>(alphabetlen * alphabetlen);

            for (int i = 0; i < alphabet.Length; i++)
                for (int j = 0; j < alphabet.Length; j++)
                    bigrams.Add(alphabet[i].ToString()+alphabet[j].ToString(), 0);

            for (int i = 0; i < text.Length-1; i++) 
                bigrams[text.Substring(i, 2)] += 1;

            double H_char = 0;
            double I_char = 0;

            foreach (KeyValuePair<char, int> a in char_freq) { 
                double p = (double)a.Value / text.Length;
                if (a.Value != 0) H_char -= p*Math.Log(p,2);
                I_char += a.Value * (a.Value - 1);
            }

            I_char /= (text.Length * (text.Length - 1));

            double H_bigrams = 0;
            double I_bigrams = 0;

            foreach (KeyValuePair<string, int> a in bigrams) {
                double p = (double)a.Value / text.Length;
                if (a.Value != 0) H_bigrams -= p * Math.Log(p, 2);
                I_bigrams += a.Value * (a.Value - 1);
            }

            I_bigrams = 4*I_bigrams/(text.Length * (text.Length - 1));

            string[] Y1 = ToVizhener(TextToBreak(text, 10_000, 10));
            string[] Y2 = ToAffine(TextToBreak(text, 10_000, 100));

        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~||
        static string[] TextToBreak (string text, int N, int L) {

            string[] X = new string[N];
            for (int i = 0; i<N; i++) { 
                Random rnd = new Random();
                X[i] = text.Substring(rnd.Next(text.Length-L), L);
            }
            return X;
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~||
        static string[] ToVizhener (string[] X) {
            
            int N = X.Length;
            int L = X[0].Length;

            string[] Y = new string[N];
            int[] KeyLength = new int[] { 1, 5, 10 };
            Random rnd = new Random();

            for (int i = 0; i < N; i++) {
                
                int r = KeyLength[rnd.Next(KeyLength.Length)];
                char[] Key = new char[r];
                for (int j = 0; j < r; j++)
                    Key[j] = alphabet[rnd.Next(alphabet.Length)];
                for (int j = 0; j < L; j++)
                    Y[i] += alphabet[(Array.IndexOf(alphabet,X[i][j]) + Array.IndexOf(alphabet, Key[j%r]))%alphabet.Length];
            }

            return Y;
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~||
        static string[] ToAffine (string[] X) {

            int N = X.Length;
            int L = X[0].Length;
            int index = 0;

            string[] bigrams = new string[alphabet.Length*alphabet.Length];
            for (int i = 0; i < alphabet.Length; i++)
                for (int j = 0; j < alphabet.Length; j++)
                    bigrams[index++] = alphabet[i].ToString() + alphabet[j].ToString();
            string[] Y = new string[N];

            for (int i = 0; i<N; i++) {
                
                Random rnd = new Random();
                switch (rnd.Next(2)) {
                
                    case 0:
                        int a = 0;
                        while (GCD(alphabet.Length,a) != 1) a = rnd.Next(alphabet.Length);
                        int b = rnd.Next(alphabet.Length);
                        break;
                }
            }

            return null;
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~||
        static int GCD(int a, int b) {
            int Remainder;

            while (b != 0) {
                Remainder = a % b;
                a = b;
                b = Remainder;
            }

            return a;
        }
    }
}
