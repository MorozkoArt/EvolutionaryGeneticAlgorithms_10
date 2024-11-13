using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    internal class Program
    {
        static int populationSize = 500;
        static int generations = 1000;
        static double mutationRate = 0.7;
        static Random random = new Random();

        public static List<List<int>> processingTimes = new List<List<int>>(); // Время обработки (15 заявок, 5 приборов)
        public static List<int> dueDates = new List<int>(); // Директивные сроки
        static List<int> penalty = new List<int>();


        static void Main(string[] args)
        {
            //Чтение из файла:
            ReadFile();
            // Вывод входных данных
            Write();
            // Генерация начальной популяции
            List<int[]> population = InitializePopulation();
            for (int generation = 0; generation < generations; generation++)
            {
                List<int[]> newPopulation = new List<int[]>();
                while (newPopulation.Count < populationSize)
                {
                    int[] parent1 = SelectParent(population);
                    int[] parent2 = SelectParent(population);
                    int[] child = Crossover(parent1, parent2);
                    Mutate(child);
                    newPopulation.Add(child);
                }
                population = newPopulation;
            }

            int[] bestSchedule = population.OrderBy(CalculatePenalty).First();
            int[] ttt = { 12, 6, 11, 13, 8, 0, 1, 2, 3, 4, 5, 7, 9, 10, 14 };
            int bestPenalty = CalculatePenalty(bestSchedule);
            for (int i = 0; i < bestSchedule.Length; i++)
            {
                bestSchedule[i] = bestSchedule[i] + 1;
            }
            Console.WriteLine("\n\nBest Schedule: " + string.Join(", ", bestSchedule));
            Console.WriteLine("Total Penalty: " + bestPenalty);
            Console.WriteLine("hjfghjg: " + CalculatePenalty(ttt));
            Console.ReadKey();
        }
        static void ReadFile()
        {
            int n = 0;
            int m = 0;
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
                else if (s == "TIME_W")
                {
                    int count = 0;
                    while (count != n)
                    {
                        count++;
                        s = f.ReadLine();
                        List<int> time_str = s.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
                        processingTimes.Add(time_str);
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
                            dueDates = s.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
                        }
                        else if (i == penalties_ind)
                        {
                            penalty = s.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
                        }
                    }
                }

            }
            f.Close();

        }
        static void Write()
        {
            Console.WriteLine($"Колдичество приборов: {processingTimes[0].Count}");
            Console.WriteLine($"Колдичество заявок: {processingTimes.Count}");
            Console.WriteLine("Матрица времен выполнения заявок на приборах: ");
            for (int i = 0; i < processingTimes.Count; i++)
            {
                for (int j = 0; j < processingTimes[0].Count; j++)
                {
                    Console.Write(String.Format("{0,-4}", processingTimes[i][j]));
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            Console.Write("Директивные сроки: ");
            for (int p = 0; p < dueDates.Count; p++)
            {
                Console.Write(dueDates[p] + " ");
            }
            Console.WriteLine("\n");
            Console.Write("Коэффиценты штрафа: ");
            for (int k = 0; k < penalty.Count; k++)
            {
                Console.Write(penalty[k] + " ");
            }
        }

        static List<int[]> InitializePopulation()
        {
            List<int[]> population = new List<int[]>();
            for (int i = 0; i < populationSize; i++)
            {
                int[] schedule = Enumerable.Range(0, processingTimes[0].Count).ToArray();
                Shuffle(schedule);
                population.Add(schedule);
            }
            return population;
        }

        static void Shuffle(int[] array)
        {
            for (int i = array.Length - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (array[i], array[j]) = (array[j], array[i]);
            }
        }

        static int[] SelectParent(List<int[]> population)
        {
            return population[random.Next(population.Count)];
        }

        static int[] Crossover(int[] parent1, int[] parent2)
        {
            int length = parent1.Length;
            int[] child = new int[length];
            bool[] taken = new bool[length];

            int crossoverPoint = random.Next(1, length - 1);
            for (int i = 0; i < crossoverPoint; i++)
            {
                child[i] = parent1[i];
                taken[parent1[i]] = true;
            }

            int index = crossoverPoint;
            for (int i = 0; i < length; i++)
            {
                if (!taken[parent2[i]])
                {
                    child[index++] = parent2[i];
                }
            }

            return child;
        }

        static void Mutate(int[] schedule)
        {
            for (int i = 0; i < schedule.Length; i++)
            {
                if (random.NextDouble() < mutationRate)
                {
                    int j = random.Next(schedule.Length);
                    (schedule[i], schedule[j]) = (schedule[j], schedule[i]);
                }
            }
        }
        static int CalculatePenalty(int[] schedule)
        {
            int totalPenalty = 0;
            int jobCompletionTime;
            List<List<ClassOO>> pen = ClassOO.Generation_List(processingTimes);
            for (int i = 0; i < schedule.Length; i++)
            {
                for (int machine = 0; machine < processingTimes.Count; machine++)
                {
                    if (machine == 0)
                    {
                        if (i != 0) pen[machine][i].x = pen[machine][i - 1].y;
                        pen[machine][i].y = pen[machine][i].x + processingTimes[machine][schedule[i]];
                    }
                    else
                    {
                        if (i == 0)
                        {
                            pen[machine][i].x = pen[machine - 1][i].y;
                            pen[machine][i].y = pen[machine][i].x + processingTimes[machine][schedule[i]];
                        }
                        else if (pen[machine - 1][i].y > pen[machine][i - 1].y)
                        {
                            pen[machine][i].x = pen[machine - 1][i].y;
                            pen[machine][i].y = pen[machine][i].x + processingTimes[machine][schedule[i]];
                        }
                        else
                        {
                            pen[machine][i].x = pen[machine][i - 1].y;
                            pen[machine][i].y = pen[machine][i].x + processingTimes[machine][schedule[i]];
                        }
                    }
                }
                jobCompletionTime = pen[processingTimes.Count - 1][i].y;
                // Проверяем, если текущее время превышает срок выполнения
                if (jobCompletionTime > dueDates[schedule[i]])
                {
                    totalPenalty += penalty[schedule[i]] * Math.Max(0, jobCompletionTime - dueDates[schedule[i]]);
                }
            }
            return totalPenalty;
        }

    }

}