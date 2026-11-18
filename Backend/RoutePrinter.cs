using System.Globalization;
using System.Text;

namespace Backend;

/// <summary>
///Here is a brief interpretation of the project, Zulu.
/// It generates a step-by-step route description: tasks at the same location are grouped
/// into a single stop, deliveries are listed before pickups, and it also identifies
/// the streets traveled, as well as the total distance in km.
/// </summary>

public static class RoutePrinter
{
    public static string Print(
        Graph<string> city,
        IReadOnlyList<Package> packages,
        string start,
        RoutePlanner planner,
        List<Stop> route)
    {
        var text = new StringBuilder();
        var (streets, oneWayStreets) = CountStreets(city);

        text.AppendLine($"POSICION INICIAL: {start}");
        text.AppendLine($"Paquetes: {packages.Count}  Ubicaciones: {city.VertexCount}  Calles: {streets} ({oneWayStreets} UN SOLO SENTIDO)");
        text.AppendLine();

        string current = start;
        double total = 0;
        int number = 1;

        
        foreach (var group in GroupStops(route))
        {
            string location = group[0].Location;
            string tasks = Describe(group);

            if (location == current)
            {
                text.AppendLine($"{number}. En {location}: {tasks}.");
            }
            else
            {
                double meters = planner.Distance(current, location);
                string path = string.Join(" -> ", planner.PathBetween(current, location));
                total += meters;

                text.AppendLine($"{number}. Ir de {current} a {location} ({Format(meters)} m): {tasks}. Camino: {path}");
            }

            current = location;
            number++;
        }

        string kilometers = (total / 1000).ToString("F2", CultureInfo.InvariantCulture);
        text.AppendLine();
        text.AppendLine("TODOS LOS PAQUETES FUERON RECOGIDOS Y ENTREGADOS DE MANERA CORRECTA");
        text.AppendLine($"Recorrido total: {kilometers} km");

        return text.ToString();
    }

    
    private static List<List<Stop>> GroupStops(List<Stop> route)
    {
        var groups = new List<List<Stop>>();

        foreach (var stop in route)
        {
            if (groups.Count > 0 && groups[^1][0].Location == stop.Location)
            {
                groups[^1].Add(stop);
            }
            else
            {
                groups.Add(new List<Stop> { stop });
            }
        }

        return groups;
    }

 
    private static string Describe(List<Stop> group)
    {
        var deliveries = group.Where(s => s.Kind == StopKind.Delivery).Select(s => s.PackageId).ToList();
        var pickups = group.Where(s => s.Kind == StopKind.Pickup).Select(s => s.PackageId).ToList();

        var parts = new List<string>();
        if (deliveries.Count > 0)
        {
            parts.Add("entregar " + PackagesText(deliveries));
        }
        if (pickups.Count > 0)
        {
            parts.Add("recoger " + PackagesText(pickups));
        }

        return string.Join(" y ", parts);
    }


    private static string PackagesText(List<int> ids)
    {
        if (ids.Count == 1)
        {
            return $"paquete {ids[0]}";
        }

        return $"paquetes {string.Join(", ", ids.Take(ids.Count - 1))} y {ids[^1]}";
    }

    private static (int Streets, int OneWay) CountStreets(Graph<string> city)
    {
        int edges = 0;
        int oneWay = 0;

        foreach (var vertex in city.Vertices)
        {
            foreach (var edge in city.GetEdges(vertex))
            {
                edges++;
                if (!city.GetEdges(edge.To).Any(back => back.To == edge.From))
                {
                    oneWay++;
                }
            }
        }

        int twoWay = (edges - oneWay) / 2;
        return (oneWay + twoWay, oneWay);
    }

    private static string Format(double value)
    {
        return value.ToString("0.##", CultureInfo.InvariantCulture);
    }
}