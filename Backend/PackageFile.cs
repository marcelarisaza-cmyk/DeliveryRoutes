using System.Globalization;

namespace Backend;

/// <summary>
/// I'll leave a brief interpretation of the project here, Zulu.
/// Loads the tasks file location,action,package and validates it against the city map
/// and groups the Pickup and Delivery lines of each package into Package objects.
/// </summary>
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
                errors.Add($"Línea {lineNumber}: FORMATO INVALIDO, SE ESPERABA 'UBICACIÓN,ACCIÓN,PAQUETE' Y SE ENCONTRÓ '{line}'.");
                continue;
            }

            string location = parts[0];
            string action = parts[1];

            bool isPickup = action.Equals("Pickup", StringComparison.OrdinalIgnoreCase);
            bool isDelivery = action.Equals("Delivery", StringComparison.OrdinalIgnoreCase);
            if (!isPickup && !isDelivery)
            {
                errors.Add($"Línea {lineNumber}: la acción '{action}' DEBE SER 'Pickup' O 'Delivery'.");
                continue;
            }

            if (!int.TryParse(parts[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out int id) || id <= 0)
            {
                errors.Add($"Línea {lineNumber}: el número de paquete '{parts[2]}' NO ES UN ENTERO MAYOR QUE 0.");
                continue;
            }

            if (!city.ContainsVertex(location))
            {
                errors.Add($"Línea {lineNumber}: la ubicación '{location}' NO EXISTE EN EL MAPA.");
                continue;
            }

            if (isPickup)
            {
                if (!pickups.TryAdd(id, location))
                {
                    errors.Add($"Línea {lineNumber}: el paquete {id}  YA TIENE UNA RECOGIDA OSEA (Pickup).");
                }
            }
            else
            {
                if (!deliveries.TryAdd(id, location))
                {
                    errors.Add($"Línea {lineNumber}: el paquete {id} YA TIENE UNA ENTREGA OSEA (Delivery).");
                }
            }
        }

        foreach (int id in pickups.Keys.Union(deliveries.Keys).OrderBy(x => x))
        {
            if (!pickups.ContainsKey(id))
            {
                errors.Add($"El paquete {id} NO TIENE RECOGIDA OSEA (Pickup).");
            }
            else if (!deliveries.ContainsKey(id))
            {
                errors.Add($"El paquete {id} NO TIENE ENTREGA OSEA (Delivery).");
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