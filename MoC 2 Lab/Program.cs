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
        static string text = "";
        static Dictionary<char, int> Dictionary_char_freq = null;
        static Dictionary<string, int> Dictionary_bigrams_freq = null;
        static List<KeyValuePair<char, int>> List_char_freq = null;
        static List<KeyValuePair<string, int>> List_bigrams_freq = null;
        //static List<char> Afrq_char = new List<char>();
        //static List<string> Afrq_bigrams = new List<string>();
        //static int most_freq = 5; //for criteria 2.0 and 2.1
        //static int k_f = 2; //for criteria 2.1

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~||
        static void Main(string[] args) {

            int alphabetlen = 33;

            StreamReader origin = new StreamReader(@"C:\Users\PRIDE\source\repos\MoC 2 Lab\text.txt");

            text = origin.ReadToEnd();
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

            Dictionary_char_freq = new Dictionary<char, int>(alphabetlen);

            for (int i = 0; i < alphabet.Length; i++)
                Dictionary_char_freq.Add(alphabet[i], 0);

            for (int i = 0; i < text.Length; i++)
                Dictionary_char_freq[text[i]] += 1; 

            Dictionary_bigrams_freq = new Dictionary<string, int>(alphabetlen * alphabetlen);

            for (int i = 0; i < alphabet.Length; i++)
                for (int j = 0; j < alphabet.Length; j++)
                    Dictionary_bigrams_freq.Add(alphabet[i].ToString()+alphabet[j].ToString(), 0);

            for (int i = 0; i < text.Length-1; i+=2)
                Dictionary_bigrams_freq[text.Substring(i, 2)] += 1;

            double H_char = 0;
            double I_char = 0;

            foreach (KeyValuePair<char, int> a in Dictionary_char_freq) { 
                double p = (double)a.Value / text.Length;
                if (a.Value != 0) H_char -= p*Math.Log(p,2);
                I_char += a.Value * (a.Value - 1);
            }

            I_char /= (text.Length * (text.Length - 1));

            double H_bigrams = 0;
            double I_bigrams = 0;

            foreach (KeyValuePair<string, int> a in Dictionary_bigrams_freq) {
                double p = (double)a.Value / text.Length;
                if (a.Value != 0) H_bigrams -= p * Math.Log(p, 2);
                I_bigrams += a.Value * (a.Value - 1);
            }

            I_bigrams = 4*I_bigrams/(text.Length * (text.Length - 1));

            List_char_freq = Dictionary_char_freq.ToList();
            List_bigrams_freq = Dictionary_bigrams_freq.ToList();

            List_char_freq.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));
            List_bigrams_freq.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));

            string[] Y1 = ToVizhener(ToBreakText(text, 10_000, 100), false);
            Criteria2_0(Y1, false, 5);
            string[] Y2 = ToAffine(ToBreakText(text, 10_000, 100), false);
            string[] Y3 = Uniform(ToBreakText(text, 10_000, 1_000), false);
            string[] Y4 = Formula(ToBreakText(text, 1_000, 10_000), false);

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
        static string[] ToVizhener (string[] X, bool bit) {
            
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
        static string[] ToAffine (string[] X, bool bit) {

            int N = X.Length;
            int L = X[0].Length;

            int index = 0;

            if (bigrams == null) {

                bigrams = new string[alphabet.Length * alphabet.Length];
                for (int i = 0; i < alphabet.Length; i++)
                    for (int j = 0; j < alphabet.Length; j++)
                        bigrams[index++] = alphabet[i].ToString() + alphabet[j].ToString();
            } 

            string[] Y = new string[N];

            Random rnd = new Random();
            int a = 0;
            int b = 0;

            switch (bit) {

                case false:
                    for (int i = 0; i < N; i++) {

                        while (GCD(alphabet.Length, a) != 1) a = rnd.Next(alphabet.Length);
                        b = rnd.Next(alphabet.Length);
                        for (int j = 0; j < L; j++)
                            Y[i] += alphabet[a * (Array.IndexOf(alphabet, X[i][j]) + b) % alphabet.Length];
                    }
                    return Y;

                case true:
                    for (int i = 0; i < N; i++) {
                        while (GCD(bigrams.Length, a) != 1) a = rnd.Next(bigrams.Length);
                        b = rnd.Next(bigrams.Length);
                        for (int j = 0; j < L - 1; j += 2)
                            Y[i] += bigrams[a * (Array.IndexOf(bigrams, X[i].Substring(j, 2)) + b) % bigrams.Length];
                    }
                    return Y;
                default:
                    return null;
            }
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~||
        static string[] Uniform (string[] X, bool bit) {

            int N = X.Length;
            int L = X[0].Length;

            string[] Y = new string[N];

            Random rnd = new Random();

            switch (bit) {

                case false:
                    for (int i = 0; i < N; i++) {
                        for (int j = 0; j < L; j++)
                            Y[i] += alphabet[rnd.Next(alphabet.Length)];
                    }
                    return Y;

                case true:
                    for (int i = 0; i < N; i++) {
                        for (int j = 0; j < L - 1; j += 2)
                            Y[i] += bigrams[rnd.Next(bigrams.Length)];
                    }
                    return Y;
            }
            return null;
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~||
        static string[] Formula (string[] X, bool bit) {

            int N = X.Length;
            int L = X[0].Length;

            string[] Y = new string[N];

            Random rnd = new Random();

            switch (bit) {

                case false:
                    for (int i = 0; i < N; i++) {

                        Y[i] += alphabet[rnd.Next(alphabet.Length)];
                        Y[i] += alphabet[rnd.Next(alphabet.Length)];
                        for (int j = 2; j < L; j++)
                            Y[i] += alphabet[(Array.IndexOf(alphabet, Y[i][j - 1]) + Array.IndexOf(alphabet, Y[i][j - 2])) % alphabet.Length];
                    }
                    return Y;

                case true:
                    for (int i = 0; i < N; i++) {
                        Y[i] += bigrams[rnd.Next(bigrams.Length)];
                        Y[i] += bigrams[rnd.Next(bigrams.Length)];
                        for (int j = 4; j < L - 1; j += 2)
                            Y[i] += bigrams[(Array.IndexOf(bigrams, Y[i].Substring(j - 4, 2)) + Array.IndexOf(bigrams, Y[i].Substring(j - 2, 2))) % bigrams.Length];
                    }
                    return Y;
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

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~||
        static int[] Criteria2_0 (string[] Y, bool bit, int most_freq) {

            int N = Y.Length;
            int L = Y[0].Length;
            int[] H = new int[2];

            Console.WriteLine("Criteria 2.0 is operating...");
            List<string> Afrq = new List<string>(most_freq);

            switch (bit) {

                case false:
                    for (int i = 0; i < most_freq; i++)
                        Afrq.Add(List_char_freq.ElementAt(alphabet.Length-i-1).Key.ToString());
                    break;

                case true:
                    for (int i = 0; i < most_freq; i++)
                        Afrq.Add(List_bigrams_freq.ElementAt(bigrams.Length - i - 1).Key);
                    break;

                default:
                    return null;
            }

            for (int i = 0; i < N; i++) {

                bool cont = true;
                foreach (var item in Afrq) {
                    cont &= Y[i].Contains(item);
                }
                if (cont)
                    H[0]++;
                else
                    H[1]++;
            }
            return H;
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~||
        static int[] Criteria2_1(string[] Y, bool bit, int most_freq, int k_f) {
            
            int N = Y.Length;
            int L = Y[0].Length;
            int[] H = new int[2];

            if (k_f >= most_freq) {
                Console.WriteLine("Invalid k_f!");
                return null;
            }

            Console.WriteLine("Criteria 2.1 is operating...");
            List<string> Afrq = new List<string>(most_freq);

            switch (bit) {

                case false:
                    for (int i = 0; i < most_freq; i++)
                        Afrq.Add(List_char_freq.ElementAt(alphabet.Length - i - 1).Key.ToString());
                    break;

                case true:
                    for (int i = 0; i < most_freq; i++)
                        Afrq.Add(List_bigrams_freq.ElementAt(bigrams.Length - i - 1).Key);
                    break;

                default:
                    return null;
            }

            for (int i = 0; i < N; i++) {

                List<string> Aaf = new List<string>();
                foreach (var item in Afrq) 
                    if (Y[i].Contains(item)) Aaf.Add(item);
                
                if (Aaf.Count > k_f)
                    H[0]++;
                else
                    H[1]++;
            }
            return H;
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~||
        static int[] Criteria2_2(string[] Y, bool bit, int most_freq) {

            int N = Y.Length;
            int L = Y[0].Length;
            int[] H = new int[2];

            Console.WriteLine("Criteria 2.2 is operating...");

            switch (bit) {
                default:
                    break;

                case false:
                    for (int i = 0; i < N; i++) {
                        List<string> Afrq = new List<string>(most_freq);
                        for (int j = 0; j < most_freq; j++)
                            Afrq.Add(List_char_freq.ElementAt(alphabet.Length - j - 1).Key.ToString());
                        
                        Dictionary<char, double> Prob = CalculateCharProb(Y[i]);
  
                        bool cont = false;
                        foreach (var item in Afrq) {
                            if(Prob[item[0]] < Dictionary_char_freq[item[0]] / text.Length) { 
                                H[1]++;
                                cont = true;
                                break;
                            }
                        }
                        if (cont) continue;
                        H[0]++;
                    }
                    return H;

                case true:
                    for (int i = 0; i < N; i++) {
                        List<string> Afrq = new List<string>(most_freq);
                        for (int j = 0; j < most_freq; j++)
                            Afrq.Add(List_bigrams_freq.ElementAt(bigrams.Length - j - 1).Key.ToString());

                        Dictionary<string, double> Prob = CalculateBigramsProb(Y[i]);
                        bool cont = false;
                        foreach (var item in Afrq) {
                            if (Prob[item] < 2*Dictionary_bigrams_freq[item] / text.Length) {
                                H[1]++;
                                cont = true;
                                break;
                            }
                        }
                        if (cont) continue;
                        H[0]++;
                    }
                    return H;
            }
            return null;
        }
        
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~||
        static int[] Criteria2_3(string[] Y, bool bit, int most_freq) {

            int N = Y.Length;
            int L = Y[0].Length;
            int[] H = new int[2];

            Console.WriteLine("Criteria 2.3 is operating...");

            List<string> Afrq = new List<string>(most_freq); 
            double Kf = 0;

            switch (bit) {
                default:
                    return null;

                case false:
                    
                    for (int j = 0; j < most_freq; j++)
                        Afrq.Add(List_char_freq.ElementAt(alphabet.Length - j - 1).Key.ToString());

                    Kf = 0;
                    foreach (var item in Afrq) 
                        Kf += Dictionary_char_freq[item[0]];                    
                    Kf /= text.Length;
                    for (int i = 0; i < N; i++) {

                        Dictionary<char, double> Prob = CalculateCharProb(Y[i]);

                        double Ff = 0;
                        foreach (var item in Afrq) 
                            Ff += Prob[item[0]];
                        if (Ff < Kf) 
                            H[1]++;
                        else 
                            H[0]++;
                    }
                    return H;

                case true:
                    for (int j = 0; j < most_freq; j++)
                        Afrq.Add(List_bigrams_freq.ElementAt(bigrams.Length - j - 1).Key.ToString());

                    Kf = 0;
                    foreach (var item in Afrq) 
                        Kf += Dictionary_char_freq[item[0]];
                    Kf /= text.Length;
                    for (int i = 0; i < N; i++) {

                        Dictionary<string, double> Prob = CalculateBigramsProb(Y[i]);

                        double Ff = 0;
                        foreach (var item in Afrq) 
                            Ff += Prob[item];
                        Kf = 2 * Kf / text.Length;
                        if (Ff < Kf)
                            H[1]++;
                        else
                            H[0]++;
                    }
                    return H;
            }
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~||
        static Dictionary<char, double> CalculateCharProb(string Y) {

            Dictionary<char, double> Result = new Dictionary<char, double>();

            for (int i = 0; i < alphabet.Length; i++)
                Result.Add(alphabet[i], 0);

            for (int i = 0; i < Y.Length; i++)
                Result[Y[i]]++;

            for (int i = 0; i < alphabet.Length; i++)
                Result[alphabet[i]] /= Y.Length;

            return Result;
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~||
        static Dictionary<string, double> CalculateBigramsProb(string Y) {

            Dictionary<string, double> Result = new Dictionary<string, double>();

            for (int i = 0; i < bigrams.Length; i++)
                Result.Add(bigrams[i], 0);

            for (int i = 0; i < Y.Length; i+=2)
                Result[Y.Substring(i, 2)]++;

            for (int i = 0; i < bigrams.Length; i++)
                Result[bigrams[i]] = 2*Result[bigrams[i]] / Y.Length;

            return Result;
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~||
        static void CalculatingAfrq () {



        }
    }
}
