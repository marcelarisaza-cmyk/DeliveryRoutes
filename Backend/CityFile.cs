using System.Globalization;

namespace Backend;

public static class CityFile
{
    public static Graph<string> Load(string path)
    {
        var lines = new SimpleTextFile(path).ReadLines();
        var city = new Graph<string>(directed: true);
        var errors = new List<string>();
        var streets = new HashSet<(string, string)>();

        for (int i = 0; i < lines.Length; i++)
        {
            int lineNumber = i + 1;
            string line = lines[i].Trim();

            if (line.Length == 0 || line.StartsWith('#'))
            {
                continue;
            }

            string[] parts = line.Split(',').Select(p => p.Trim()).ToArray();
            if (parts.Length != 4 || parts.Any(p => p.Length == 0))
            {
                errors.Add($"Línea {lineNumber}: formato inválido, se esperaba 'origen,destino,distancia,sentido' y se encontró '{line}'.");
                continue;
            }

            string from = parts[0];
            string to = parts[1];

            if (!double.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out double distance)
                || !double.IsFinite(distance)
                || distance <= 0)
            {
                errors.Add($"Línea {lineNumber}: la distancia '{parts[2]}' no es un número positivo.");
                continue;
            }

            bool twoWay;
            if (parts[3].Equals("TwoWay", StringComparison.OrdinalIgnoreCase))
            {
                twoWay = true;
            }
            else if (parts[3].Equals("OneWay", StringComparison.OrdinalIgnoreCase))
            {
                twoWay = false;
            }
            else
            {
                errors.Add($"Línea {lineNumber}: el sentido '{parts[3]}' debe ser TwoWay u OneWay.");
                continue;
            }

            var key = string.CompareOrdinal(from, to) <= 0 ? (from, to) : (to, from);
            if (!streets.Add(key))
            {
                errors.Add($"Línea {lineNumber}: la calle {from}-{to} está repetida (A,B y B,A son la misma calle).");
                continue;
            }

            city.AddEdge(from, to, distance);
            if (twoWay)
            {
                city.AddEdge(to, from, distance);
            }
        }

        if (errors.Count > 0)
        {
            throw new InvalidDataException(string.Join(Environment.NewLine, errors));
        }

        return city;
    }
}