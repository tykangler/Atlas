namespace Atlas.Console.Services;

public static class FileUtilities
{
    public static TextWriter CreateFile(string path)
    {
        string? directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }
        return File.CreateText(path);
    }
}
