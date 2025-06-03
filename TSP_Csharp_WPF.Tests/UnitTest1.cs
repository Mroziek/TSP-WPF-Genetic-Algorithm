namespace TSP_Csharp_WPF.Tests;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public void CalculateDistance_ReturnsCorrectValue()
    {
        var cityA = new City(0, 0);
        var cityB = new City(3, 4);

        int distance = FileReader.CalculateDistance(cityA, cityB);

        Assert.AreEqual(5, distance);
    }
}
