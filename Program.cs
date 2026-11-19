using Backend;

/// <summary>
//Here is a brief interpretation of the Zulu project.
// The program's entry point reads the arguments (or prompts for them via the console),
// validates the files,
// plans the route, and outputs the results.

if (args.Length != 0 && args.Length != 3)
{
    Console.WriteLine("Uso: dotnet run -- city.txt packages.txt D");
    return;
}

try
{
    string cityPath = args.Length == 3 ? args[0] : Ask( "Ruta del archivo  MAPA (CITY.TXT): ");
    string packagesPath = args.Length == 3 ? args[1] : Ask("Ruta del archivo PAQUETE (PACKAGES.TXT): ");
    string start = (args.Length == 3 ? args[2] : Ask("Posición INICIO (D): ")).Trim();

    var city = CityFile.Load(cityPath);
    var packages = PackageFile.Load(packagesPath, city);

    var problems = ShortestPaths.CheckReachability(city, packages, start);
    if (problems.Count > 0)
    {
        throw new InvalidDataException(string.Join(Environment.NewLine, problems));
    }

    var planner = new RoutePlanner(city, packages, start);
    var route = planner.Plan();
    Console.Write(RoutePrinter.Print(city, packages, start, planner, route));
}
catch (InvalidDataException ex)
{
    Console.WriteLine(ex.Message);
    Environment.ExitCode = 1;
}
catch (FileNotFoundException ex)
{
    Console.WriteLine($"NO SE ENCONTRO ARCHIVO {ex.FileName}'.");
    Environment.ExitCode = 1;
}
catch (DirectoryNotFoundException ex)
{
    Console.WriteLine($"NO SE ENCONTRO LA CARPETA DEL ARCHIVO: {ex.Message}");
    Environment.ExitCode = 1;
}

catch (IOException ex)
{
    Console.WriteLine($"NO SE PUEDE LEER EL ARCHIVO: {ex.Message}");
    Environment.ExitCode = 1;

}

    static string Ask(string question)
{
    string? answer;
    do
    {
        Console.Write(question);
        answer = Console.ReadLine();
    }
    while (string.IsNullOrWhiteSpace(answer));

    return answer.Trim();
}