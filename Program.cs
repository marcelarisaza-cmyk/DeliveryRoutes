/*using Backend;

try
{
    var city = CityFile.Load("city.txt");
    Console.WriteLine($"Ubicaciones: {city.VertexCount}");
    Console.WriteLine($"Aristas: {city.Vertices.Sum(v => city.GetEdges(v).Count)}");

    foreach (var start in new[] { "AQ", "BS" })
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
}*/

/*using Backend;

try
{
    var city = CityFile.Load("city_mini.txt");
    var packages = PackageFile.Load("packages_mini.txt", city);

    Console.WriteLine($"Paquetes: {packages.Count}");
    foreach (var package in packages)
    {
        Console.WriteLine($"Paquete {package.Id}: recoger en {package.Pickup}, entregar en {package.Delivery}");
    }
}
catch (InvalidDataException ex)
{
    Console.WriteLine(ex.Message);
}*/
/*
using Backend;

try
{
    var city = CityFile.Load("city.txt");
    var packages = PackageFile.Load("packages.txt", city);

    Console.WriteLine($"Ubicaciones: {city.VertexCount}");
    Console.WriteLine($"Paquetes: {packages.Count}");

    foreach (var package in packages.Take(5))
    {
        Console.WriteLine($"Paquete {package.Id}: recoger en {package.Pickup}, entregar en {package.Delivery}");
    }

    var last = packages[^1];
    Console.WriteLine($"Paquete {last.Id}: recoger en {last.Pickup}, entregar en {last.Delivery}");
}
catch (InvalidDataException ex)
{
    Console.WriteLine(ex.Message);
}*/

/*using Backend;

var mini = CityFile.Load("city_mini.txt");


var (distance, previous) = ShortestPaths.Dijkstra(mini, "A");
foreach (var vertex in distance.Keys.OrderBy(v => v))
{
    Console.WriteLine($"A -> {vertex}: {distance[vertex]} m");
}
Console.WriteLine("Camino A a G: " + string.Join(" -> ", ShortestPaths.BuildPath(previous, "A", "G")));

var (distanceD, previousD) = ShortestPaths.Dijkstra(mini, "D");
Console.WriteLine($"D -> A: {distanceD["A"]} m");
Console.WriteLine("Camino D a A: " + string.Join(" -> ", ShortestPaths.BuildPath(previousD, "D", "A")));

var city = CityFile.Load("city.txt");
Console.WriteLine($"AQ -> BS: {ShortestPaths.Dijkstra(city, "AQ").Distance["BS"]} m");
Console.WriteLine($"BS -> AQ: {ShortestPaths.Dijkstra(city, "BS").Distance["AQ"]} m");*/

/*using Backend;

var city = CityFile.Load("city_mini.txt");
var packages = PackageFile.Load("packages_mini.txt", city);
var planner = new RoutePlanner(city, packages, "A");

var route = planner.NearestNeighbor();
int number = 1;
foreach (var stop in route)
{
    string action = stop.Kind == StopKind.Pickup ? "Recoger" : "Entregar";
    Console.WriteLine($"{number++}. {action} paquete {stop.PackageId} en {stop.Location}");
}

Console.WriteLine($"Válida: {planner.IsValid(route)}");
Console.WriteLine($"Total: {planner.TotalDistance(route)} m");
Console.WriteLine("Camino D a A: " + string.Join(" -> ", planner.PathBetween("D", "A")));*/

/*using Backend;

var city = CityFile.Load("city_mini.txt");
var packages = PackageFile.Load("packages_mini.txt", city);
var planner = new RoutePlanner(city, packages, "A");

var route = planner.Plan();
int number = 1;
foreach (var stop in route)
{
    string action = stop.Kind == StopKind.Pickup ? "Recoger" : "Entregar";
    Console.WriteLine($"{number++}. {action} paquete {stop.PackageId} en {stop.Location}");
}

Console.WriteLine($"Válida: {planner.IsValid(route)}");
Console.WriteLine($"Total: {planner.TotalDistance(route)} m");*/

using Backend;

var city = CityFile.Load("city.txt");
var packages = PackageFile.Load("packages.txt", city);
var planner = new RoutePlanner(city, packages, "D");

var inserted = planner.BuildByInsertion(packages);
Console.WriteLine($"Solo inserción: {planner.TotalDistance(inserted) / 1000:F2} km");

var relocated = planner.Relocate(inserted);
Console.WriteLine($"Con reubicación: {planner.TotalDistance(relocated) / 1000:F2} km");
Console.WriteLine($"Paradas: {relocated.Count}");
Console.WriteLine($"Válida: {planner.IsValid(relocated)}");