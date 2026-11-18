namespace Backend;

public static class ShortestPaths
{
    
    public static (Dictionary<string, double> Distance, Dictionary<string, string> Previous) Dijkstra(
        Graph<string> graph, string source)
    {
        
        var distance = new Dictionary<string, double>();
        var previous = new Dictionary<string, string>();
        var queue = new PriorityQueue<string, double>();

        distance[source] = 0;
        queue.Enqueue(source, 0);

        while (queue.TryDequeue(out string? current, out double currentDistance))
        {
            if (currentDistance > distance[current])
            {
                continue;
            }

            foreach (var edge in graph.GetEdges(current))
            {
                double candidate = currentDistance + edge.Weight;

                if (!distance.TryGetValue(edge.To, out double known) || candidate < known)
                {
                    distance[edge.To] = candidate;
                    previous[edge.To] = current;
                    queue.Enqueue(edge.To, candidate);
                }
            }
        }

        return (distance, previous);
    }

    
    public static List<string> BuildPath(Dictionary<string, string> previous, string source, string target)
    {
        var path = new List<string> { target };
        string current = target;

        while (current != source)
        {
            if (!previous.TryGetValue(current, out string? before))
            {
                return new List<string>();
            }

            current = before;
            path.Add(current);
        }

        path.Reverse();
        return path;
    }
    
    public static List<string> CheckReachability(Graph<string> city, IReadOnlyList<Package> packages, string start)
    {
        var problems = new List<string>();

        if (!city.ContainsVertex(start))
        {
            problems.Add($"La posición inicial '{start}' no existe en el mapa.");
            return problems;
        }

        var points = new HashSet<string>();
        foreach (var package in packages)
        {
            points.Add(package.Pickup);
            points.Add(package.Delivery);
        }

        var ordered = points.OrderBy(p => p, StringComparer.Ordinal).ToList();

        var fromStart = Dijkstra(city, start).Distance;
        foreach (var point in ordered)
        {
            if (!fromStart.ContainsKey(point))
            {
                problems.Add($"El punto {point} no es alcanzable desde la posición inicial {start}.");
            }
        }

        foreach (var point in ordered)
        {
            var reachable = Dijkstra(city, point).Distance;
            var missing = ordered.Where(other => other != point && !reachable.ContainsKey(other)).ToList();

            if (missing.Count > 0)
            {
                problems.Add($"Desde el punto {point} no se puede llegar a: {string.Join(", ", missing)}.");
            }
        }

        return problems;
    }
}