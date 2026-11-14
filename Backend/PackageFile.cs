using System.Globalization;

namespace Backend;

public static class PackageFile
{
    public static List<Package> Load(string path, Graph<string> city)
    {
        var lines = new SimpleTextFile(path).ReadLines();
        var errors = new List<string>();
        var pickups = new Dictionary<int, string>();
        var deliveries = new Dictionary<int, string>();

        for (int i = 0; i < lines.Length; i++)
        {
            int lineNumber = i + 1;
            string line = lines[i].Trim();

            if (line.Length == 0 || line.StartsWith('#'))
            {
                continue;
            }

            string[] parts = line.Split(',').Select(p => p.Trim()).ToArray();
            if (parts.Length != 3 || parts.Any(p => p.Length == 0))
            {
                errors.Add($"Línea {lineNumber}: formato inválido, se esperaba 'ubicación,acción,paquete' y se encontró '{line}'.");
                continue;
            }

            string location = parts[0];
            string action = parts[1];

            bool isPickup = action.Equals("Pickup", StringComparison.OrdinalIgnoreCase);
            bool isDelivery = action.Equals("Delivery", StringComparison.OrdinalIgnoreCase);
            if (!isPickup && !isDelivery)
            {
                errors.Add($"Línea {lineNumber}: la acción '{action}' debe ser Pickup o Delivery.");
                continue;
            }

            if (!int.TryParse(parts[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out int id) || id <= 0)
            {
                errors.Add($"Línea {lineNumber}: el número de paquete '{parts[2]}' no es un entero mayor que 0.");
                continue;
            }

            if (!city.ContainsVertex(location))
            {
                errors.Add($"Línea {lineNumber}: la ubicación '{location}' no existe en el mapa.");
                continue;
            }

            if (isPickup)
            {
                if (!pickups.TryAdd(id, location))
                {
                    errors.Add($"Línea {lineNumber}: el paquete {id} ya tiene una recogida (Pickup).");
                }
            }
            else
            {
                if (!deliveries.TryAdd(id, location))
                {
                    errors.Add($"Línea {lineNumber}: el paquete {id} ya tiene una entrega (Delivery).");
                }
            }
        }

        foreach (int id in pickups.Keys.Union(deliveries.Keys).OrderBy(x => x))
        {
            if (!pickups.ContainsKey(id))
            {
                errors.Add($"El paquete {id} no tiene recogida (Pickup).");
            }
            else if (!deliveries.ContainsKey(id))
            {
                errors.Add($"El paquete {id} no tiene entrega (Delivery).");
            }
        }

        if (errors.Count > 0)
        {
            throw new InvalidDataException(string.Join(Environment.NewLine, errors));
        }

        return pickups.Keys
            .OrderBy(id => id)
            .Select(id => new Package(id, pickups[id], deliveries[id]))
            .ToList();
    }
}