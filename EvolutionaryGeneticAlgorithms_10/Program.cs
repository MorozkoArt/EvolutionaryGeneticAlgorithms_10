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
        static int populationSize = 20;
        static int populationadd = 50;
        static int generations = 100; 
        static double mutationRate = 0.05;
        static int tournamentSize = 10;
        static int maxend = 10;
        static Random random = new Random();
        public static List<List<int>> processingTimes = new List<List<int>>(); // Время обработки (15 заявок, 5 приборов)
        public static List<int> dueDates = new List<int>(); // Директивные сроки
        static List<int> penalty = new List<int>(); // Штрафы

        // Лучшая перестановка: 13, 7, 12, 9, 3, 1, 4, 14, 15, 5, 10, 6, 8, 11, 2
        // Значение целевой ф-ии: 1166
        static void Main(string[] args)
        {
            ReadFile();
            WriteConsole();
            int[] bestSchedule = Generation();
            int[] InSchedule = { 12, 6, 11, 13, 8, 0, 1, 2, 3, 4, 5, 7, 9, 10, 14 };
            int bestPenalty = CalculatePenalty(bestSchedule);
            int bestPenalty_e_learning = CalculatePenalty(InSchedule);
            for (int i = 0; i < bestSchedule.Length; i++)
            {
                bestSchedule[i] = bestSchedule[i] + 1;
            }
            for (int i = 0; i < InSchedule.Length; i++)
            {
                InSchedule[i] = InSchedule[i] + 1;
            }
            Console.WriteLine($"\n\nЛучшая перестановка: {string.Join(", ", bestSchedule)}");
            Console.WriteLine($"Значение целевой ф-ии: {bestPenalty}");
            Console.WriteLine("".PadRight(85, '-'));           
            Console.WriteLine($"Перестановка из файла с ответами: {string.Join(", ", InSchedule)}");
            Console.WriteLine($"Значение целевой функции для этой перестановки: {bestPenalty_e_learning}");
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
        static void WriteConsole()
        {
            Console.WriteLine($"Количество приборов: {processingTimes.Count}");
            Console.WriteLine($"Количество заявок: {processingTimes[0].Count}");
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
        static int[] Generation()
        {
            List<int[]> population = InitializePopulation().OrderBy(CalculatePenalty).ToList();
            int minZnach = CalculatePenalty(population[0]);
            int end_now = 0;
            for (int generation = 0; generation < generations; generation++)
            {
                while (population.Count < populationSize + populationadd)
                {
                    int[] parent1 = SelectParent(population);
                    int[] parent2 = SelectParent(population);
                    int[] child = Crossover(parent1, parent2);
                    Mutate(child);
                    population.Add(child);
                }
                population = Select(population);

                int minZnach_now = CalculatePenalty(population[0]);

                if (End(end_now, minZnach_now, minZnach) == 1)
                    break;
                else if (End(end_now, minZnach_now, minZnach) == 2)
                {
                    minZnach = minZnach_now;
                    end_now = 0;
                }
                else end_now++;
            }
            int[] bestSchedule = population.OrderBy(CalculatePenalty).First();
            return bestSchedule;
        }

        private static List<int[]> Select(List<int[]> population)
        {
            int[] bestIndividual = population.OrderBy(CalculatePenalty).First();

            var rankedPopulation = population.Except(new List<int[]> { bestIndividual }).OrderBy(CalculatePenalty).ToList();

            int populationCount = rankedPopulation.Count;
            List<double> rankFitness = new List<double>();
            for (int i = 0; i < populationCount; i++)
            {
                rankFitness.Add(i + 1); 
            }
            double totalFitness = rankFitness.Sum();
            List<double> probabilities = rankFitness.Select(x => x / totalFitness).ToList();
            List<int[]> selectedPopulation = new List<int[]>(populationSize - 1); 
            Random random = new Random();
            // рулетка
            for (int i = 0; i < populationSize - 1; i++)
            {
                double randomNumber = random.NextDouble();
                double cumulativeProbability = 0;
                int selectedIndex = -1;
                for (int j = 0; j < populationCount; j++)
                {
                    cumulativeProbability += probabilities[j];
                    if (randomNumber <= cumulativeProbability)
                    {
                        selectedIndex = j;
                        break;
                    }
                }
                selectedPopulation.Add(rankedPopulation[selectedIndex]);
            }
            selectedPopulation.Add(bestIndividual);

            return selectedPopulation;
        }

        static int End(int end_now, int minZnach_now, int minZnach)
        {
            if (minZnach_now >= minZnach)
            {
                end_now++;
                if (end_now == maxend) return 1;
            }
            else return 2;
            return 3;

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
            int type_of_Select = random.Next(2);
            if (type_of_Select == 0) return SelectParent_random(population);
            else return SelectParent_tournament(population);
        }
        static int[] SelectParent_random(List<int[]> population)
        {
            return population[random.Next(population.Count)];
        }
        
        static int[] SelectParent_tournament(List<int[]> population)
        {
            if (tournamentSize > population.Count)
            {
                tournamentSize = population.Count;
            }
            List<int[]> tournamentParticipants = new List<int[]>();

            // Случайный выбор участников турнира
            for (int i = 0; i < tournamentSize; i++)
            {
                int randomIndex = random.Next(population.Count);
                tournamentParticipants.Add(population[randomIndex]);
            }

            int[] bestParent = tournamentParticipants[0];
            double bestFitness = CalculatePenalty(bestParent); 

            foreach (var participant in tournamentParticipants)
            {
                double fitness = CalculatePenalty(participant);
                if (fitness < bestFitness) 
                {
                    bestFitness = fitness;
                    bestParent = participant;
                }
            }
            return bestParent;
        }
        static int[] Crossover(int[] parent1, int[] parent2)
        {
            int type_of_Crossover = random.Next(2);
            if (type_of_Crossover == 0) return Crossover_onePoint(parent1, parent2);
            else return Crossover_twoPoint(parent1, parent2);
        }
        static int[] Crossover_onePoint(int[] parent1, int[] parent2)
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
        static int[] Crossover_twoPoint(int[] parent1, int[] parent2)
        {
            int length = parent1.Length;
            int[] child = new int[length];
            bool[] taken = new bool[length];
            for (int i = 0; i<child.Length; i++)
            {
                child[i] = -1;
            }
            int crossoverPoint1 = random.Next(1, length - 1);
            int crossoverPoint2 = random.Next(1, length - 1);
            if (crossoverPoint1 > crossoverPoint2)
            {
                (crossoverPoint1, crossoverPoint2) = (crossoverPoint2, crossoverPoint1);
            }
            for (int i = crossoverPoint1; i < crossoverPoint2; i++)
            {
                child[i] = parent1[i];
                taken[parent1[i]] = true;
            }
            int index = 0;
            for (int i = 0; i < length; i++)
            {
                if (!taken[parent2[i]])
                {
                    while (index < length && child[index] != -1) 
                    {
                        index++;
                    }
                    if (index < length)
                    {
                        child[index++] = parent2[i];
                    }
                }
            }
            return child;
        }

        static void Mutate(int[] schedule)
        {
            int type_of_mutation = random.Next(2);
            if (type_of_mutation == 0) Mutate_Point(schedule);
            else Mutate_Inversion(schedule);

        }
        static void Mutate_Point(int[] schedule)  
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
        static void Mutate_Inversion(int[] schedule)
        {
            for (int i = 0; i < schedule.Length; i++)
            {
                if (random.NextDouble() < mutationRate)
                {
                    int startIndex = random.Next(schedule.Length);
                    int endIndex = random.Next(schedule.Length);
                    if (startIndex > endIndex)
                    {
                        (startIndex, endIndex) = (endIndex, startIndex);
                    }
                    Array.Reverse(schedule, startIndex, endIndex - startIndex + 1);
                }
            }
        }
        
        static int CalculatePenalty(int[] schedule)
        {
            int totalPenalty = 0;
            int jobCompletionTime;
            List<List<X_Y>> pen = X_Y.Generation_List(processingTimes);
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
                totalPenalty += penalty[schedule[i]] * Math.Max(0, jobCompletionTime - dueDates[schedule[i]]);
                
            }
            return totalPenalty;
        }
    }
}