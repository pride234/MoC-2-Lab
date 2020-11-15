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
        static string[] bigrams = null;

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

            Dictionary<string, int> bigrams_freq = new Dictionary<string, int>(alphabetlen * alphabetlen);

            for (int i = 0; i < alphabet.Length; i++)
                for (int j = 0; j < alphabet.Length; j++)
                    bigrams_freq.Add(alphabet[i].ToString()+alphabet[j].ToString(), 0);

            for (int i = 0; i < text.Length-1; i+=2)
                bigrams_freq[text.Substring(i, 2)] += 1;

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

            foreach (KeyValuePair<string, int> a in bigrams_freq) {
                double p = (double)a.Value / text.Length;
                if (a.Value != 0) H_bigrams -= p * Math.Log(p, 2);
                I_bigrams += a.Value * (a.Value - 1);
            }

            I_bigrams = 4*I_bigrams/(text.Length * (text.Length - 1));

            string[] Y1 = ToVizhener(ToBreakText(text, 10_000, 10));
            string[] Y2 = ToAffine(ToBreakText(text, 10_000, 100));
            string[] Y3 = Uniform(ToBreakText(text, 10_000, 1_000));
            string[] Y4 = Formula(ToBreakText(text, 1_000, 10_000));

        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~||
        static string[] ToBreakText (string text, int N, int L) {

            string[] X = new string[N];
            Random rnd = new Random();
            for (int i = 0; i<N; i++) { 
                int index = rnd.Next(text.Length - L);
                X[i] = text.Substring(index, L);
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

            bigrams = new string[alphabet.Length*alphabet.Length];
            for (int i = 0; i < alphabet.Length; i++)
                for (int j = 0; j < alphabet.Length; j++)
                    bigrams[index++] = alphabet[i].ToString() + alphabet[j].ToString();
            string[] Y = new string[N];

            Random rnd = new Random();
            for (int i = 0; i<N; i++) {
                
                int a = 0;
                int b = 0;
                switch (rnd.Next(2)) {
                
                    case 0:
                        while (GCD(alphabet.Length,a) != 1) a = rnd.Next(alphabet.Length);
                        b = rnd.Next(alphabet.Length);
                        for (int j = 0; j < L; j++) 
                            Y[i] += alphabet[a*(Array.IndexOf(alphabet, X[i][j]) + b)%alphabet.Length];                       
                        continue;

                    case 1:
                        while (GCD(bigrams.Length, a) != 1) a = rnd.Next(bigrams.Length);
                        b = rnd.Next(bigrams.Length);
                        for (int j = 0; j < L-1; j+=2)
                            Y[i] += bigrams[a * (Array.IndexOf(bigrams, X[i].Substring(j,2)) + b) % bigrams.Length];
                        continue;
                }
            }
            return Y;
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~||
        static string[] Uniform (string[] X) {

            int N = X.Length;
            int L = X[0].Length;

            string[] Y = new string[N];

            Random rnd = new Random();
            for (int i = 0; i < N; i++) {

                switch (rnd.Next(2)) {

                    case 0:
                        for (int j = 0; j < L; j++)
                            Y[i] += alphabet[rnd.Next(alphabet.Length)];
                        continue;

                    case 1:
                        for (int j = 0; j < L - 1; j += 2)
                            Y[i] += bigrams[rnd.Next(bigrams.Length)];
                        continue;
                }
            }
            return Y;              
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~||
        static string[] Formula (string[] X) {

            int N = X.Length;
            int L = X[0].Length;

            string[] Y = new string[N];

            Random rnd = new Random();
            for (int i = 0; i < N; i++) {

                switch (rnd.Next(2)) {

                    case 0:
                        
                        Y[i] += alphabet[rnd.Next(alphabet.Length)];
                        Y[i] += alphabet[rnd.Next(alphabet.Length)];
                        for (int j = 2; j < L; j++)
                            Y[i] += alphabet[(Array.IndexOf(alphabet, Y[i][j-1]) + Array.IndexOf(alphabet, Y[i][j - 2]))%alphabet.Length];
                        continue;

                    case 1:
                        Y[i] += bigrams[rnd.Next(bigrams.Length)];
                        Y[i] += bigrams[rnd.Next(bigrams.Length)];
                        for (int j = 4; j < L - 1; j+=2)
                            Y[i] += bigrams[(Array.IndexOf(bigrams, Y[i].Substring(j-4, 2)) + Array.IndexOf(bigrams, Y[i].Substring(j - 2, 2))) % bigrams.Length];
                        continue;
                }
            }
            return Y;
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
