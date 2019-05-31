# TSP-WPF

Travelling salesman problem* - solved with genetic algorithm in C# - including animated Map (WPF)

*The travelling salesman problem (TSP) asks the following question: "Given a list of cities and the distances between each pair of cities, what is the shortest possible route that visits each city and returns to the origin city?" It is an NP-hard problem in combinatorial optimization, important in operations research and theoretical computer science.

I created this genetic algorithm earlier, but it was just console application - it was impossible to visualize in mind generated solutions.
Now, with GUI you can see exacly all locations on map, and every step of genetic algorithm's genius. It's very satysfying to watch when algorithm start from clearly random route and keeps improving during time.

Generation 500 vs Generation 50000
<br>
<img src="https://user-images.githubusercontent.com/47602711/58696522-0abdf680-8398-11e9-8dbb-1d144c15ce62.png" height="500">

Genetic Algorithm it's not the best approach for this problem, but results are acceptable - if it's not finds the optimum, it's very close to it. Sometimes it's better to restart a simulation and repeat it few times to get the best solution.

Thanks to GUI created in WPF, user can easily choose source file with problem (app accepts standarized tsp files with integer coordinates - example files are in repository), tune parameters to get the best result, and start/stop algorithm when needed. The best part of this approach is map refreshed every specified interval (at the begging every 50 generations, later 250). You can clearly see how the algorithm works.
<br><br><br>
Screenshoots:
![screen2](https://user-images.githubusercontent.com/47602711/58694919-6edebb80-8394-11e9-8776-b43bfe4f3e57.png)
<br>
![screen1](https://user-images.githubusercontent.com/47602711/58694920-6edebb80-8394-11e9-88c2-25e949c99e91.png)


Genetic algorithm is optimized for speed (ex. distances matrix is created at the start, so distance between cities don't have to be calculated everytime, just once). GUI slows it a little bit, but in most cases you get acceptable result in 5-60seconds.<br>
It uses:<br>
-Ordered CrossOver (OX)<br>
-Inversion Mutation (some research show, that it gives faster result compare to other mutations)<br>
-Tournament Selection (3 paths(tours) in tournament)</p>
