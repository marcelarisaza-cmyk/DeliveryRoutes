using Backend;

var graph = new Graph<string>(directed: true);

graph.AddEdge("A", "B", 100);
graph.AddEdge("B", "C", 50);
graph.AddEdge("C", "B", 50);


foreach (var vertex in graph.Vertices)
{
    foreach (var edge in graph.GetEdges(vertex))
    {
        Console.WriteLine($"{edge.From} -> {edge.To} ({edge.Weight} m)");
    }
}