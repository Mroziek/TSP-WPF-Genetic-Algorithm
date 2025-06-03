using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP_Csharp_WPF
{
    static class FileReader
    {
        public static int[,] CreateDistanceMatrix(List<City> cities) //method that creates matrix with distances - good for optimalization, faster calculating during checking path length
        {
            int size = cities.Count;
            int[,] distancesArray = new int[size, size];

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    distancesArray[i, j] = CalculateDistance(cities[i], cities[j]);
                }
            }
            return distancesArray;
        }

        public static int CalculateDistance(City cityA, City cityB) //calculates distances beetwen cities to create distance matrix
        {
            int dx = cityA.X - cityB.X;
            int dy = cityA.Y - cityB.Y;
            double distanceSquared = dx * dx + dy * dy; //takes coordinations of 2 points (2D - X,Y)
            return (int)Math.Round(Math.Sqrt(distanceSquared));
        }

        public static List<City> ReadFile(string filePath) //read file
        {
            List<City> CityList = new List<City>();

            string[] lines = File.ReadAllLines(filePath);

            bool NODE_COORD_TYPE = false; //in next line after that phrase are coordinations (this is what is needed)

            foreach (string line in lines)
            {
                if (NODE_COORD_TYPE && line != "EOF")
                {
                    string[] row = line.Trim().Split(' ');
                    City city = new City(int.Parse(row[1]), int.Parse(row[2])); // The first value is just the city ID (starting from 1), so it is ignored.
                    CityList.Add(city);
                }

                if (line == "NODE_COORD_SECTION")
                {
                    NODE_COORD_TYPE = true;
                }
            }
            return CityList;
        }
    }
}
