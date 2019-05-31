using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP_Csharp_WPF
{
    class Population
    {
        public Tour[] PopulationArray;
        int numberOfPaths;
        public int numberOfCities;
        public static Random r = new Random();

        public Tour bestPathInPopulation;
        public int lengthofBestPath = int.MaxValue;

        public Population(int numberOfPaths, int numberOfCities) //constructor of population - it creates population with random paths
        {
            PopulationArray = new Tour[numberOfPaths];
            for (int i = 0; i < numberOfPaths; ++i)
            {
                PopulationArray[i] = new Tour(numberOfCities);
                PopulationArray[i].GenerateRandomPath();
            }

            this.numberOfPaths = numberOfPaths;
            this.numberOfCities = numberOfCities;
            bestPathInPopulation = new Tour(numberOfCities);
        }


        public void PrintPopulation() //print whole population to console - for debugging purposes
        {
            foreach (Tour path in PopulationArray)
            {
                path.PrintPath();
                Console.WriteLine();
            }
            Console.WriteLine();
        }


        public void CrossoverPopulation(int crossoverChance) //crossover - user specify crossover chance - paths are crossovered or just copied to next generation
        {
            Tour[] newPopulationArray = new Tour[numberOfPaths];
            for (int i = 0; i < numberOfPaths; ++i) //new empty generation
            {
                newPopulationArray[i] = new Tour(numberOfCities);
            }

            for (int k = 0; k < PopulationArray.Length; k++) //loop all paths
            {
                int individual1 = k;
                int individual2 = GlobalRandom.r.Next(0, numberOfPaths);

                if (GlobalRandom.r.Next(0, 100) < crossoverChance && !PopulationArray[individual1].PathCities.SequenceEqual(PopulationArray[individual2].PathCities))
                {

                    Array.Copy(CrossoverPaths(PopulationArray[individual1].PathCities, PopulationArray[individual2].PathCities), newPopulationArray[k].PathCities, newPopulationArray[k].PathCities.Length);
                }

                else
                {
                    Array.Copy(PopulationArray[k].PathCities, newPopulationArray[k].PathCities, PopulationArray[k].PathCities.Length);
                }
            }

            Array.Copy(newPopulationArray, PopulationArray, newPopulationArray.Length); //copy new generation to main population array 
        }

        int[] CrossoverPaths(int[] path1, int[] path2) //crossover 2 paths
        {
            int pos1 = GlobalRandom.r.Next(0, path1.Length);
            int pos2 = GlobalRandom.r.Next(0, path1.Length);

            if (pos1 > pos2)
            {
                int foo1 = pos2;
                pos2 = pos1;
                pos1 = foo1;
            }

            int[] tab1 = new int[pos2 - pos1];
            int[] newPath = new int[path1.Length];

            for (int p = pos1, x = 0; p < pos2; p++, x++)
            {
                tab1[x] = path1[p];
            }

            for (int p = 0; p < path1.Length; p++)
            {
                newPath[p] = -1;
            }

            for (int p = pos1; p < pos2; p++)
            {
                newPath[p] = path1[p];
            }

            int foo = 0;
            for (int p = 0; p < path1.Length;)
            {
                if (newPath[p] == -1)
                {
                    if (!tab1.Contains(path2[foo]))
                    {
                        newPath[p] = path2[foo];
                        p++;
                    }
                    foo++;
                }
                else p++;
            }
            return newPath;
        }


        public void Mutation(int mutationChance) //mutate whole population
        {
            for (int k = 0; k < PopulationArray.Length; k++)
            {
                if (GlobalRandom.r.Next(0, 100) < mutationChance) //user specify mutation chance - path mutates or dont change
                {
                    PopulationArray[k].MutatePath();
                }
            }
        }


        public void TournamentSelection(int startPrinting) //get the best paths in population. Application make tournaments between 3 random paths - the best of them is in new population.
        {
            Tour[] newPopulationArray = new Tour[numberOfPaths];
            for (int i = 0; i < numberOfPaths; ++i)
            {
                newPopulationArray[i] = new Tour(numberOfCities);
            }

            int[] fitnessArray = new int[numberOfPaths];

            for (int k = 0; k < numberOfPaths; k++)
            {
                fitnessArray[k] = PopulationArray[k].CalculateFitness();
                if (fitnessArray[k] < lengthofBestPath)
                {
                    if (startPrinting > 5000)
                    {
                        PopulationArray[k].PrintPath();
                    }
                    Array.Copy(PopulationArray[k].PathCities, bestPathInPopulation.PathCities, PopulationArray[k].PathCities.Length);
                    lengthofBestPath = fitnessArray[k];
                    Console.WriteLine();
                }
            }

            for (int k = 0; k < numberOfPaths; k++)
            {
                int tournamentWinner;

                int individual1 = GlobalRandom.r.Next(0, numberOfPaths);
                int individual2 = GlobalRandom.r.Next(0, numberOfPaths);
                int individual3 = GlobalRandom.r.Next(0, numberOfPaths);

                int sumDistancePath1 = fitnessArray[individual1];
                int sumDistancePath2 = fitnessArray[individual2];
                int sumDistancePath3 = fitnessArray[individual3];

                if (sumDistancePath1 <= sumDistancePath2 && sumDistancePath1 <= sumDistancePath3) tournamentWinner = individual1;
                else if (sumDistancePath2 <= sumDistancePath1 && sumDistancePath2 <= sumDistancePath3) tournamentWinner = individual2;
                else tournamentWinner = individual3;

                Array.Copy(PopulationArray[tournamentWinner].PathCities, newPopulationArray[k].PathCities, PopulationArray[k].PathCities.Length);
            }

            Array.Copy(newPopulationArray, PopulationArray, newPopulationArray.Length);
        }



    }
}
