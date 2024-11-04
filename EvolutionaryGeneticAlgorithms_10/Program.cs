using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EvolutionaryGeneticAlgorithms_10
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int n = 0;
            int m = 0;
            List<List<int>> Time_w = new List<List<int>>();
            List<int> legislative_deadlines = new List<int>();
            List<int> penalty_rates = new List<int>();
            string s = " ";
            string path = @"Z15_5_7.DAT";
            StreamReader f = new StreamReader(path);
            while (!f.EndOfStream)
            {
                s = f.ReadLine();
                if (s == "PARAM")
                {
                    s = f.ReadLine();
                    s.Replace("  ", " ");
                    List<int> param = s.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
                    n = param[0]; m = param[1];  
                }
                else if(s == "TIME_W")
                {
                    int count = 0;
                    while(count != n)
                    {
                        count++;
                        s = f.ReadLine();
                        List<int> time_str = s.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
                        Time_w.Add(time_str);
                    }
                }
                else if (s == "TIME_ST")
                {
                    int derectiv_ind = 1;
                    int penalties_ind = 3;
                    int max_ind = 4;
                    for (int i = 0; i < max_ind; i++)
                    {
                        s = f.ReadLine();                     
                        if (i == derectiv_ind)
                        {
                            legislative_deadlines = s.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
                        }
                        else if (i == penalties_ind)
                        {
                            penalty_rates = s.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
                        }
                    }
                }
                
            }
            f.Close();
            Console.WriteLine($"Колдичество приборов {n}");
            Console.WriteLine($"Колдичество заявок {m}");
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    Console.Write(Time_w[i][j] + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            for (int p = 0; p < legislative_deadlines.Count; p++)
            {
                Console.Write(legislative_deadlines[p] + " ");
            }
            Console.WriteLine("\n");
            for (int k = 0; k< penalty_rates.Count; k++)
            {
                Console.Write(penalty_rates[k] + " ");
            }
            Console.ReadKey();
        }
    }
}
