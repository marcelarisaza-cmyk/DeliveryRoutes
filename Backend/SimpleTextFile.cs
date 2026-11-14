namespace Backend;

public class SimpleTextFile
{
    private readonly string? _path;

    public SimpleTextFile(string path)
    {
        _path = path;
    }

    public void WriteLines(string[] lines)
    {
        File.WriteAllLines(_path!, lines);
    }

    public string[] ReadLines()
    {
        return File.ReadAllLines(_path!);
    }
}