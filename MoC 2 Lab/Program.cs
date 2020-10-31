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

            StreamReader origin = new StreamReader(@"C:\Users\PRIDE\source\repos\MoC 2 Lab\text.txt");

            string text = origin.ReadToEnd();
            //string curLine = "";
            //string text = "";
            //while ((curLine = cp.ReadLine()) != null) text += curLine;
            origin.Close();

            text = text.Replace("ґ", "г");
            text = text.ToLower();

            Regex rgx = new Regex("[^а-яі]");
            text = rgx.Replace(text, "");
            //Console.WriteLine(text);

            StreamWriter form = new StreamWriter(@"C:\Users\PRIDE\source\repos\MoC 2 Lab\text1.txt");

            form.Write(text);
            form.Close();
        }
    }
}
