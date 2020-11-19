using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;

namespace MoC_2_Lab {

    class Program {

        static char[] alphabet = new[] { 'а', 'б', 'в', 'г', 'д', 'е', 'є', 'ж', 'з', 'и', 'і', 'ї', 'й', 'к', 'л', 'м', 'н', 'о', 'п', 'р', 'с', 'т', 'у', 'ф', 'х', 'ц', 'ч', 'ш', 'щ', 'ь', 'ю', 'я' };
        static string[] bigrams = new string[alphabet.Length*alphabet.Length];
        static string text = "";
        static Dictionary<char, int> Dictionary_char_freq = null;
        static Dictionary<string, int> Dictionary_bigrams_freq = null;
        static List<KeyValuePair<char, int>> List_char_freq = null;
        static List<KeyValuePair<string, int>> List_bigrams_freq = null;
        static double I_char = 0;
        static double I_bigrams = 0;
        static double compress = 0;
        static SharpLZW.LZWEncoder Arch = new SharpLZW.LZWEncoder();
        static int criteria2_0_PAR = 5;
        static int criteria2_1_PAR = 5;
        static int criteria2_2_PAR = 5;
        static int criteria2_3_PAR = 5;
        static int criteria4_PAR = 5;
        static int criteria5_PAR = 5;
        static int criteriaStruct_PAR = 5;

        //static List<char> Afrq_char = new List<char>();
        //static List<string> Afrq_bigrams = new List<string>();
        //static int most_freq = 5; //for criteria 2.0 and 2.1
        //static int k_f = 2; //for criteria 2.1

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~||
        static void Main(string[] args) {

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

            Dictionary_char_freq = new Dictionary<char, int>(alphabet.Length);

            for (int i = 0; i < alphabet.Length; i++)
                Dictionary_char_freq.Add(alphabet[i], 0);

            for (int i = 0; i < text.Length; i++)
                Dictionary_char_freq[text[i]] += 1; 

            Dictionary_bigrams_freq = new Dictionary<string, int>(alphabet.Length * alphabet.Length);

            int index = 0;
            for (int i = 0; i < alphabet.Length; i++)
                for (int j = 0; j < alphabet.Length; j++) { 
                    Dictionary_bigrams_freq.Add(alphabet[i].ToString()+alphabet[j].ToString(), 0); 
                    bigrams[index++] = alphabet[i].ToString() + alphabet[j].ToString();
                }

            for (int i = 0; i < text.Length-1; i+=2)
                Dictionary_bigrams_freq[text.Substring(i, 2)] += 1;

            double H_char = 0;

            foreach (KeyValuePair<char, int> a in Dictionary_char_freq) { 
                double p = (double)a.Value / text.Length;
                if (a.Value != 0) H_char -= p*Math.Log(p,2);
                I_char += a.Value * (a.Value - 1);
            }

            I_char /= (text.Length * (text.Length - 1));

            double H_bigrams = 0;

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

            //string[] Y1 = ToVizhener(ToBreakText(text, 10_000, 1000), false);
            //Criteria2_0(Y1, false, 5);
            //string[] Y2 = ToAffine(ToBreakText(text, 10_000, 100), false);
            //string[] Y3 = Uniform(ToBreakText(text, 10_000, 1_000), false);
            //string[] Y4 = Formula(ToBreakText(text, 1_000, 10_000), false);
            
            compress = (double)Arch.EncodeToByteList(Arch.Encode(text)).Count()/ASCIIEncoding.ASCII.GetByteCount(text);

            Dictionary<string[], string[]> Dic = new Dictionary<string[], string[]>();

            //Dic.Add(ToBreakText(text, 10_000, 10), ToVizhener(ToBreakText(text, 10_000, 10), false));
            //Dic.Add(ToBreakText(text, 10_000, 10), ToVizhener(ToBreakText(text, 10_000, 10), true));

            Arch.EncodeToByteList(Arch.Encode(ToBreakText(text, 3)[0]));

            foreach (var item in new bool[]{true, false }){

                for (int i = 0; i < 4; i++) {
                    string[] X = ToBreakText(text, i);
                    int N = X.Length;
                    int L = X[0].Length;

                    double H1 = (double)Criteria2_0(X, item)[1] / N;
                    Console.WriteLine("Open text; Bigrams = {3}; Criteria2_0; N = {0}; L = {1}; H1 = {2};", N, L, H1, item);

                    for (int j = 0; j < 4; j++) {
                        double H0 = 0;

                        foreach (var item2 in new bool[] {true, false}) {
                            string[] Y = ToDistort(X, j, item);

                            H0 = (double)Criteria2_0(Y, item)[0] / N;
                            Console.WriteLine("Cipher text {4}; Bigrams = {3}; Criteria2_0; N = {0}; L = {1}; H0 = {2}", N, L, H0, item, j);

                            H0 = (double)Criteria2_1(Y, item)[0] / N;
                            Console.WriteLine("Cipher text {4}; Bigrams = {3}; Criteria2_1; N = {0}; L = {1}; H0 = {2}", N, L, H0, item, j);

                            H0 = (double)Criteria2_2(Y, item)[0] / N;
                            Console.WriteLine("Cipher text {4}; Bigrams = {3}; Criteria2_2; N = {0}; L = {1}; H0 = {2}", N, L, H0, item, j);

                            H0 = (double)Criteria2_3(Y, item)[0] / N;
                            Console.WriteLine("Cipher text {4}; Bigrams = {3}; Criteria2_3; N = {0}; L = {1}; H0 = {2}", N, L, H0, item, j);

                            H0 = (double)Criteria4(Y, item)[0] / N;
                            Console.WriteLine("Cipher text {4}; Bigrams = {3}; Criteria4; N = {0}; L = {1}; H0 = {2}", N, L, H0, item, j);

                            H0 = (double)Criteria5(Y, item)[0] / N;
                            Console.WriteLine("Cipher text {4}; Bigrams = {3}; Criteria5; N = {0}; L = {1}; H0 = {2}", N, L, H0, item, j);

                            H0 = (double)CriteriaStruct(Y)[0] / N;
                            Console.WriteLine("Cipher text {4}; Bigrams = {3}; CriteriaStruct; N = {0}; L = {1}; H0 = {2}", N, L, H0, item, j);
                        }
                    }
                }
            }
            Console.ReadLine();
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~||
        static string[] ToBreakText (string text, int choose) {

            int N = 0;
            int L = 0;

            switch (choose) {
                default:
                    return null;
                case 0:
                    N = 10_000;
                    L = 10;
                    break;
                case 1:
                    N = 10_000;
                    L = 100;
                    break;
                case 2:
                    N = 10_000;
                    L = 1_000;
                    break;
                case 3:
                    N = 1_000;
                    L = 10_000;
                    break;
            }

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
            int[] KeyLength = null;
            Random rnd = new Random();

            switch (bit) {
                default:
                    return null;
                case false:
                    KeyLength = new int[] { 1, 5, 10 };
                    for (int i = 0; i < N; i++) {
                        int r = KeyLength[rnd.Next(KeyLength.Length)];
                        char[] Key = new char[r];
                        for (int j = 0; j < r; j++)
                            Key[j] = alphabet[rnd.Next(alphabet.Length)];
                        for (int j = 0; j < L; j++)
                            Y[i] += alphabet[(Array.IndexOf(alphabet, X[i][j]) + Array.IndexOf(alphabet, Key[j % r])) % alphabet.Length];
                    }
                    return Y;
                case true:
                    KeyLength = new int[] { 100, 150, 300 };
                    for (int i = 0; i < N; i++) {
                        int r = KeyLength[rnd.Next(KeyLength.Length)];
                        string[] Key = new string[r];
                        for (int j = 0; j < r; j++)
                            Key[j] = bigrams[rnd.Next(bigrams.Length)];
                        for (int j = 0; j < L; j+=2)
                            Y[i] += bigrams[(Array.IndexOf(bigrams, X[i].Substring(j,2)) + Array.IndexOf(bigrams, Key[j % r])) % bigrams.Length];
                    }
                    return Y;
            }
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
        static string[] ToUniform (string[] X, bool bit) {

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
        static string[] ToFormula (string[] X, bool bit) {

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
        static int[] Criteria2_0 (string[] Y, bool bit) {

            int N = Y.Length;
            int L = Y[0].Length;
            int[] H = new int[2];
            int most_freq = 0;

            //Console.WriteLine("Criteria 2.0 is operating...");
            List<string> Afrq = null;

            switch (bit) {

                case false:
                    most_freq = 5;
                    Afrq = new List<string>(most_freq);
                    for (int i = 0; i < most_freq; i++)
                        Afrq.Add(List_char_freq.ElementAt(alphabet.Length-i-1).Key.ToString());
                    break;

                case true:
                    most_freq = 20;
                    Afrq = new List<string>(most_freq);
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
        static int[] Criteria2_1(string[] Y, bool bit) {
            
            int N = Y.Length;
            int L = Y[0].Length;
            int[] H = new int[2];
            int most_freq = 0;
            int k_f = 0;

            //Console.WriteLine("Criteria 2.1 is operating...");
            List<string> Afrq = null;

            switch (bit) {

                case false:
                    most_freq = 5;
                    k_f = 3;
                    Afrq = new List<string>(most_freq);
                    for (int i = 0; i < most_freq; i++)
                        Afrq.Add(List_char_freq.ElementAt(alphabet.Length - i - 1).Key.ToString());
                    break;

                case true:
                    most_freq = 20;
                    k_f = 15;
                    Afrq = new List<string>(most_freq);
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
        static int[] Criteria2_2(string[] Y, bool bit) {

            int N = Y.Length;
            int L = Y[0].Length;
            int[] H = new int[2];
            int most_freq = 0;

            //Console.WriteLine("Criteria 2.2 is operating...");

            switch (bit) {
                default:
                    break;

                case false:
                    most_freq = 5;
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
                    most_freq = 20;
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
        static int[] Criteria2_3(string[] Y, bool bit) {

            int N = Y.Length;
            int L = Y[0].Length;
            int[] H = new int[2];
            int most_freq = 0;

            //Console.WriteLine("Criteria 2.3 is operating...");

            List<string> Afrq = new List<string>(most_freq); 
            double Kf = 0;

            switch (bit) {
                default:
                    return null;

                case false:
                    most_freq = 5;
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
                    most_freq = 20;
                    for (int j = 0; j < most_freq; j++)
                        Afrq.Add(List_bigrams_freq.ElementAt(bigrams.Length - j - 1).Key.ToString());

                    Kf = 0;
                    foreach (var item in Afrq) 
                        Kf += Dictionary_bigrams_freq[item];
                    Kf = 2 * Kf / text.Length;
                    for (int i = 0; i < N; i++) {

                        Dictionary<string, double> Prob = CalculateBigramsProb(Y[i]);

                        double Ff = 0;
                        foreach (var item in Afrq) 
                            Ff += Prob[item];
                        if (Ff < Kf)
                            H[1]++;
                        else
                            H[0]++;
                    }
                    return H;
            }
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~||
        static int[] Criteria4 (string[] Y, bool bit) {

            int N = Y.Length;
            int L = Y[0].Length;
            int[] H = new int[2];
            double k_i = 0;

            switch (bit) {
                default:
                    return null;

                case false:
                    k_i = 0.25;
                    for (int i = 0; i < N; i++) {
                        var Prob = CalculateCharProb(Y[i]);
                        double I_local = 0;
                        foreach (var item in Prob) {
                            double value = item.Value*L;
                            I_local+=value*(value-1);
                        }
                        I_local /= L*(L-1);
                        if (Math.Abs(I_char - I_local) > k_i) 
                            H[1]++;
                        else 
                            H[0]++;
                    }
                    return H;
                case true:
                    k_i = 0.25;
                    for (int i = 0; i < N; i++) {
                        var Prob = CalculateBigramsProb(Y[i]);
                        double I_local = 0;
                        foreach (var item in Prob) {
                            double value = item.Value * L;
                            I_local += value * (value - 1);
                        }
                        I_local = 4*I_local/ (L * (L - 1));
                        if (Math.Abs(I_bigrams - I_local) > k_i)
                            H[1]++;
                        else
                            H[0]++;
                    }
                    return H;
            }
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~||
        static int[] Criteria5 (string[] Y, bool bit) {

            int N = Y.Length;
            int L = Y[0].Length;
            int[] H = new int[2];
            int[] rare = null;
            int empty = 0;
            int k_empty = 0;

            switch (bit) {
                default:
                    return null;
                case false:
                    k_empty = 3;
                    rare = new int[] {2,3,5};
                    foreach (var j in rare) {

                        Dictionary<char, int> Bprh = new Dictionary<char, int>(j);
                        for (int i = 0; i < j; i++)
                            if (Bprh.ContainsKey(List_char_freq.ElementAt(j).Key) == false) 
                                Bprh.Add(List_char_freq.ElementAt(j).Key, 0);
                        for (int i = 0; i < N; i++) {

                            for (int k = 0; k < L; k++) 
                                if (Bprh.ContainsKey(Y[i][k]))
                                    Bprh[Y[i][k]]++;
                            empty = 0;
                            foreach (var item in Bprh) 
                                if(item.Value == 0) 
                                    empty++;                                                                                                
                            if (empty <= k_empty)
                                H[1]++;
                            else 
                                H[0]++;
                        }
                    }
                    return H;

                case true:
                    k_empty = 10;
                    rare = new int[] {50, 100, 200};
                    foreach (var j in rare) {

                        Dictionary<string, int> Bprh = new Dictionary<string, int>(j);
                        for (int i = 0; i < j; i++)
                            if(Bprh.ContainsKey(List_bigrams_freq.ElementAt(j).Key.ToString()) == false) 
                                Bprh.Add(List_bigrams_freq.ElementAt(j).Key.ToString(),0);
                        for (int i = 0; i < N; i++) {

                            for (int k = 0; k < L; k+=2)
                                if (Bprh.ContainsKey(Y[i].Substring(k,2)))
                                    Bprh[Y[i].Substring(k, 2)]++;
                            empty = 0;
                            foreach (var item in Bprh)
                                if (item.Value == 0)
                                    empty++;
                            if (empty <= k_empty)
                                H[1]++;
                            else
                                H[0]++;
                        }
                    }
                    return H;
            }
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~||
        static int[] CriteriaStruct(string[] Y) {
            int N = Y.Length;
            int L = Y[0].Length;
            int[] H = new int[2];
            double k = 0.3;
            for (int i = 0; i < N; i++) {
                var Arch = new SharpLZW.LZWEncoder();
                if (Math.Abs((double)Arch.EncodeToByteList(Arch.Encode(Y[i])).Count() / ASCIIEncoding.ASCII.GetByteCount(Y[i]) - compress) <= k)
                    H[0]++;
                else 
                    H[1]++;
            }
            return H;
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
        static string[] ToDistort (string[] X, int alg, bool bit) {

            switch (alg) {
                default:
                    return null;
                case 0:
                    return ToVizhener(X, bit);
                case 1:
                    return ToAffine(X, bit);
                case 2:
                    return ToUniform(X, bit);
                case 3:
                    return ToFormula(X, bit);
            }
        }

    }
}
