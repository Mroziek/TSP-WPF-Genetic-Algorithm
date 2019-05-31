using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Threading;

namespace TSP_Csharp_WPF
{
    public partial class MainWindow : Window
    {
        string filePath = "";
        static CancellationTokenSource tokenSource2 = new CancellationTokenSource();
        CancellationToken ct = tokenSource2.Token;

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StartButton.IsEnabled = false;

                List<City> CityList = FileReader.ReadFile(filePath); //Generate List with Cities (XY coords)
                Distances.distancesArray = FileReader.CreateDistanceMatrix(CityList); //Create Distances Matrix between cities (Faster calculations during main loop)

                int mapScale = CalculateMapScale(CityList); //adjust map scale to proper show cities on GUI
                DrawCities(CityList, mapScale); //Draw cities on map

                int individualsInGeneration = (int)SliderIndividuals.Value;
                int mutationChance = (int)SliderMutation.Value;
                int crossoverChance = (int)SliderCrossover.Value;
                int numberOfLoops = 100000; //number of generations
                int numberOfCities = CityList.Count;
                Population population = new Population(individualsInGeneration, numberOfCities); //first random population

                tokenSource2 = new CancellationTokenSource();
                ct = tokenSource2.Token;

                Task.Factory.StartNew(async () => //main loop
                {
                    for (int i = 0; i <= numberOfLoops; i++)
                    {
                        if (ct.IsCancellationRequested) return;
                        population.CrossoverPopulation(crossoverChance);
                        population.Mutation(mutationChance);
                        population.TournamentSelection(10000);
                        if (i % 50 == 0 && i < 2000) //GUI update is more often during first loops
                        {
                            DrawCities(CityList, mapScale);
                            DrawLines(population.bestPathInPopulation, CityList, mapScale);
                            await Task.Delay(1);
                        }
                        else if (i % 250 == 0)
                        {
                            DrawCities(CityList, mapScale);
                            DrawLines(population.bestPathInPopulation, CityList, mapScale);
                            await Task.Delay(1);
                        }
                        UpdateScore(population.lengthofBestPath);
                        UpdateLoopCount(i); //Best score and loop counter is refreshed in every loop repeatation
                    }
                }, tokenSource2.Token);
            }
            catch (Exception ex)
            {
                if (filePath == "") MessageBox.Show("File has not been selected");
                else MessageBox.Show("File not compatibile");
            }

            StartButton.IsEnabled = true;
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            tokenSource2.Cancel();
            MessageBox.Show("Algorithm is stopped");
            StartButton.IsEnabled = true;
        }

        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            DataFileDialogWindow();
        }


        public MainWindow()
        {
            InitializeComponent();
        }

        void UpdateScore(int Score)
        {
            this.Dispatcher.Invoke(() =>
            {
                ScoreLabel.Content = "BEST SCORE: " + Score;
            });
        }

        void UpdateLoopCount(int i)
        {
            this.Dispatcher.Invoke(() =>
            {
                LoopCountLabel.Content = "Loop: " + i;
            });
        }

        void DataFileDialogWindow()
        {
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();
            openFileDlg.Filter = "TSP files (*tsp.txt)|*tsp.txt";
            Nullable<bool> result = openFileDlg.ShowDialog();

            if (result == true)
            {
                FileNameLabel.Content = System.IO.Path.GetFileName(openFileDlg.FileName);
                filePath = openFileDlg.FileName;
            }
        }

        void DrawCities(List<City> cities, int scale)
        {
            foreach (City city in cities)
            {
                this.Dispatcher.Invoke(() =>
                {
                    var square = new Rectangle()
                    {
                        Stroke = Brushes.DarkRed,
                        Fill = Brushes.DarkRed,
                        Width = 4,
                        Height = 4,
                    };
                    Canvas.SetLeft(square, city.X / scale);
                    Canvas.SetTop(square, city.Y / scale);
                    Map.Children.Add(square);
                });
            }
        }

        void DrawLines(Tour sciezka, List<City> cities, int scale)
        {
            this.Dispatcher.Invoke(() =>
            {
                Map.Children.Clear();
                DrawCities(cities, scale);
                for (int i = 0; i < cities.Count - 1; i++)
                {
                    Line lin = new Line();
                    lin.Stroke = Brushes.White;
                    lin.StrokeThickness = 2;
                    lin.X1 = cities[sciezka.PathCities[i]].X / scale + 2;
                    lin.X2 = cities[sciezka.PathCities[i + 1]].X / scale + 2;
                    lin.Y1 = cities[sciezka.PathCities[i]].Y / scale + 2;
                    lin.Y2 = cities[sciezka.PathCities[i + 1]].Y / scale + 2;
                    Map.Children.Add(lin);
                }
                //connection between last city and first city (back to hometown)
                Line lin2 = new Line();
                lin2.Stroke = Brushes.White;
                lin2.StrokeThickness = 2;
                lin2.X1 = cities[sciezka.PathCities[0]].X / scale + 2;
                lin2.X2 = cities[sciezka.PathCities[cities.Count - 1]].X / scale + 2;
                lin2.Y1 = cities[sciezka.PathCities[0]].Y / scale + 2;
                lin2.Y2 = cities[sciezka.PathCities[cities.Count - 1]].Y / scale + 2;
                Map.Children.Add(lin2);
            });
        }

        int CalculateMapScale(List<City> cities)
        {
            int max = int.MinValue;

            foreach (City city in cities)
            {
                if (city.X > max) max = city.X;
                if (city.Y > max) max = city.Y;
            }
            return (max / 800) + 1; //canvas size
        }

    }
}
