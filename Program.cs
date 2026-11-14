using Backend;

try
{
    var city = CityFile.Load("city_mini.txt");
    Console.WriteLine($"Ubicaciones: {city.VertexCount}");
    Console.WriteLine($"Aristas: {city.Vertices.Sum(v => city.GetEdges(v).Count)}");

    foreach (var start in new[] { "C", "D" })
    {
        foreach (var edge in city.GetEdges(start))
        {
            Console.WriteLine($"{edge.From} -> {edge.To} ({edge.Weight} m)");
        }
    }
}
catch (InvalidDataException ex)
{
    Console.WriteLine(ex.Message);
}