namespace Backend;

public class RoutePlanner
{
    private readonly IReadOnlyList<Package> _packages;
    private readonly string _start;
    private readonly Dictionary<string, Dictionary<string, double>> _distance = new();
    private readonly Dictionary<string, Dictionary<string, string>> _previous = new();

    public RoutePlanner(Graph<string> city, IReadOnlyList<Package> packages, string start)
    {
        _packages = packages;
        _start = start;

        
        var points = new HashSet<string> { start };
        foreach (var package in packages)
        {
            points.Add(package.Pickup);
            points.Add(package.Delivery);
        }

        foreach (var point in points)
        {
            var (distance, previous) = ShortestPaths.Dijkstra(city, point);
            _distance[point] = distance;
            _previous[point] = previous;
        }
    }

   
    public double Distance(string from, string to)
    {
        return _distance[from].TryGetValue(to, out double value) ? value : double.PositiveInfinity;
    }


    public List<Stop> Plan() => Relocate(BuildByInsertion(_packages));
    public List<Stop> NearestNeighbor()
    {
        var route = new List<Stop>();
        var picked = new HashSet<int>();
        var delivered = new HashSet<int>();
        string current = _start;

        while (delivered.Count < _packages.Count)
        {
            Stop? best = null;
            double bestDistance = double.PositiveInfinity;

            foreach (var package in _packages)
            {
                Stop candidate;
                if (!picked.Contains(package.Id))
                {
                    candidate = new Stop(package.Pickup, StopKind.Pickup, package.Id);
                }
                else if (!delivered.Contains(package.Id))
                {
                    candidate = new Stop(package.Delivery, StopKind.Delivery, package.Id);
                }
                else
                {
                    continue;
                }

                double distance = Distance(current, candidate.Location);
                if (distance < bestDistance)
                {
                    best = candidate;
                    bestDistance = distance;
                }
            }

            if (best is null)
            {
                throw new InvalidOperationException($"NO SE PUEDE LLEGAR A NINGUNA PARADA PENDIENTE DESDE {current}.");
            }

            
            route.Add(best.Value);
            if (best.Value.Kind == StopKind.Pickup)
            {
                picked.Add(best.Value.PackageId);
            }
            else
            {
                delivered.Add(best.Value.PackageId);
            }

            current = best.Value.Location;
        }

        return route;
    }

    public double TotalDistance(List<Stop> route)
    {
        double total = 0;
        string current = _start;

        foreach (var stop in route)
        {
            total += Distance(current, stop.Location);
            current = stop.Location;
        }

        return total;
    }

   
    public bool IsValid(List<Stop> route)
    {
        if (route.Count != _packages.Count * 2)
        {
            return false;
        }

        var picked = new HashSet<int>();
        var delivered = new HashSet<int>();

        foreach (var stop in route)
        {
            var package = _packages.FirstOrDefault(p => p.Id == stop.PackageId);
            if (package.Id == 0)
            {
                return false; 
            }

            if (stop.Kind == StopKind.Pickup)
            {
                if (stop.Location != package.Pickup || !picked.Add(stop.PackageId))
                {
                    return false;
                }
            }
            else
            {
                if (stop.Location != package.Delivery
                    || !picked.Contains(stop.PackageId)
                    || !delivered.Add(stop.PackageId))
                {
                    return false;
                }
            }
        }

        return picked.Count == _packages.Count && delivered.Count == _packages.Count;
    }

    
    public List<string> PathBetween(string from, string to)
    {
        return ShortestPaths.BuildPath(_previous[from], from, to);
    }

    public List<Stop> BuildByInsertion(IEnumerable<Package> order)
    {
        var route = new List<Stop>();

        foreach (var package in order)
        {
            var (pickupIndex, deliveryIndex) = BestInsertion(route, package);
            route = Insert(route, package, pickupIndex, deliveryIndex);
        }

        return route;
    }

    public List<Stop> Relocate(List<Stop> route)
    {
        bool improved = true;

        while (improved)
        {
            improved = false;

            foreach (var package in _packages)
            {
                double current = TotalDistance(route);

                var without = route.Where(s => s.PackageId != package.Id).ToList();
                var (pickupIndex, deliveryIndex) = BestInsertion(without, package);
                var candidate = Insert(without, package, pickupIndex, deliveryIndex);

                if (TotalDistance(candidate) < current - 1e-9)
                {
                    route = candidate;
                    improved = true;
                }
            }
        }

        return route;
    }

   
    private (int PickupIndex, int DeliveryIndex) BestInsertion(List<Stop> route, Package package)
    {
        int count = route.Count;
        double bestCost = double.PositiveInfinity;
        int bestI = 0;
        int bestJ = 0;

        for (int i = 0; i <= count; i++)
        {
            string before = i == 0 ? _start : route[i - 1].Location;
            string? after = i < count ? route[i].Location : null;

            for (int j = i; j <= count; j++)
            {
                double cost;

                if (i == j)
                {
                    cost = Distance(before, package.Pickup) + Distance(package.Pickup, package.Delivery);
                    if (after is not null)
                    {
                        cost += Distance(package.Delivery, after) - Distance(before, after);
                    }
                }
                else
                {
                    cost = Distance(before, package.Pickup)
                         + Distance(package.Pickup, after!) - Distance(before, after!);

                    string beforeDelivery = route[j - 1].Location;
                    string? afterDelivery = j < count ? route[j].Location : null;

                    cost += Distance(beforeDelivery, package.Delivery);
                    if (afterDelivery is not null)
                    {
                        cost += Distance(package.Delivery, afterDelivery) - Distance(beforeDelivery, afterDelivery);
                    }
                }

                
                if (cost < bestCost)
                {
                    bestCost = cost;
                    bestI = i;
                    bestJ = j;
                }
            }
        }

        return (bestI, bestJ);
    }

 
    private static List<Stop> Insert(List<Stop> route, Package package, int pickupIndex, int deliveryIndex)
    {
        var result = new List<Stop>(route);
        result.Insert(deliveryIndex, new Stop(package.Delivery, StopKind.Delivery, package.Id));
        result.Insert(pickupIndex, new Stop(package.Pickup, StopKind.Pickup, package.Id));
        return result;
    }
}