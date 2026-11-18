namespace Backend;

/// <summary>
/// I'll leave a brief interpretation of the project here, Zulu.
/// Small helper to read and write the lines of a plain text file. 
/// </summary>
/// 
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