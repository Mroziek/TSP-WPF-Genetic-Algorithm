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

        static CancellationTokenSource tokenSource = new CancellationTokenSource(); //token needed to enable stop option
        CancellationToken ct = tokenSource.Token;

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StartButton.IsEnabled = false; //block button to block more than 1 genetic algorithm tasks at the same moment

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

                tokenSource = new CancellationTokenSource(); //token needed to enable stop option
                ct = tokenSource.Token;

                Task.Factory.StartNew(() => //main loop
                {
                    for (int i = 0; i <= numberOfLoops; i++)
                    {
                        if (ct.IsCancellationRequested) break; //if button stop is clicked - abort task
                        population.CrossoverPopulation(crossoverChance);
                        population.Mutation(mutationChance);
                        population.TournamentSelection(10000);
                        if (i % 50 == 0 && i < 2000) //GUI update is more often during first loops
                        {
                            DrawLines(population.bestPathInPopulation, CityList, mapScale);
                        }
                        else if (i % 250 == 0)
                        {
                            DrawLines(population.bestPathInPopulation, CityList, mapScale);
                        }
                        UpdateScore(population.lengthofBestPath);
                        UpdateLoopCount(i); //Best score and loop counter is refreshed in every loop repeatation
                    }

                    this.Dispatcher.Invoke(() =>
                    {
                        StartButton.IsEnabled = true; //enable start button after algorithm is finished or cancelled
                    });
                }, tokenSource.Token);
            }
            catch (Exception ex)
            {
                if (filePath == "") MessageBox.Show("File has not been selected");
                else MessageBox.Show("File not compatibile"); //in case of wrong file type
            }

        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            tokenSource.Cancel(); //set token to cancel
            MessageBox.Show("Algorithm is stopped"); //show message to user
            StartButton.IsEnabled = true; //enable startbutton (it's locked at the start of genetic algorithm)
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
            Nullable<bool> result = openFileDlg.ShowDialog(); //open file dialog window 

            if (result == true) //if file selected
            {
                FileNameLabel.Content = System.IO.Path.GetFileName(openFileDlg.FileName); //show only file name - not whole path
                filePath = openFileDlg.FileName;
            }
        }

        void DrawCities(List<City> cities, int scale) //draw cities on canvas 
        {
            foreach (City city in cities) //every city on list of cities (XY coords)
            {
                this.Dispatcher.Invoke(() =>
                {
                    var square = new Rectangle()
                    {
                        Stroke = Brushes.DarkRed,
                        Fill = Brushes.DarkRed,
                        Width = 4, //size of square
                        Height = 4,
                    };
                    Canvas.SetLeft(square, city.X / scale);
                    Canvas.SetTop(square, city.Y / scale);
                    Map.Children.Add(square); //add city to canvas in GUI
                });
            }
        }

        void DrawLines(Tour path, List<City> cities, int scale)  //draw lines between cities
        {
            this.Dispatcher.Invoke(() =>
            {
                Map.Children.Clear(); //clear map to delete old routes
                DrawCities(cities, scale); //draw cities (it could be optimize to not draw cities every time - but works fine so its not necessary)
                for (int i = 0; i < cities.Count - 1; i++) //for every city in path
                {
                    Line lin = new Line();
                    lin.Stroke = Brushes.White;
                    lin.StrokeThickness = 2;
                    lin.X1 = cities[path.PathCities[i]].X / scale + 2; //X coordination - city 1
                    lin.X2 = cities[path.PathCities[i + 1]].X / scale + 2; //X coordination - city 2
                    lin.Y1 = cities[path.PathCities[i]].Y / scale + 2; //Y coordination - city 1
                    lin.Y2 = cities[path.PathCities[i + 1]].Y / scale + 2; //Y coordination - city 2
                    Map.Children.Add(lin);
                }
                //connection between last city and first city (back to hometown)
                Line lin2 = new Line();
                lin2.Stroke = Brushes.White;
                lin2.StrokeThickness = 2;
                lin2.X1 = cities[path.PathCities[0]].X / scale + 2;
                lin2.X2 = cities[path.PathCities[cities.Count - 1]].X / scale + 2;
                lin2.Y1 = cities[path.PathCities[0]].Y / scale + 2;
                lin2.Y2 = cities[path.PathCities[cities.Count - 1]].Y / scale + 2;
                Map.Children.Add(lin2);
            });
        }

        int CalculateMapScale(List<City> cities)
        {
            int max = int.MinValue;

            foreach (City city in cities) //find biggest coord value
            {
                if (city.X > max) max = city.X;
                if (city.Y > max) max = city.Y;
            }
            return (max / 800) + 1; //canvas size
        }

    }
}
