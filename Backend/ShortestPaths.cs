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
}