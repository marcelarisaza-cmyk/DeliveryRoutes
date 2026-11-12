namespace Backend;

public class Graph<T> where T : notnull
{
    private readonly Dictionary<T, List<Edge<T>>> _adjacency = new();

    public bool Directed { get; }

    public Graph(bool directed = false)
    {
        Directed = directed;
    }

    public int VertexCount => _adjacency.Count;

    public IEnumerable<T> Vertices => _adjacency.Keys;

    public bool ContainsVertex(T vertex) => _adjacency.ContainsKey(vertex);

    public void AddVertex(T vertex)
    {
        if (!_adjacency.ContainsKey(vertex))
        {
            _adjacency[vertex] = new List<Edge<T>>();
        }
    }

    public void AddEdge(T from, T to, double weight)
    {
        AddVertex(from);
        AddVertex(to);
        _adjacency[from].Add(new Edge<T>(from, to, weight));

        if (!Directed)
        {
            _adjacency[to].Add(new Edge<T>(to, from, weight));
        }
    }

  
    public IReadOnlyList<Edge<T>> GetEdges(T vertex) => _adjacency[vertex];
}