using System.Text.Json;

namespace Atlas.Console.Services;

public static class FileUtilities
{
    public static TextWriter CreateOrGetFile(string path)
    {
        string? directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }
        return File.CreateText(path);
    }

    public static async Task WriteToFile<T>(T obj, string fileName)
    {
        using var outStream = CreateOrGetFile(fileName);
        await outStream.WriteAsync(SerializationService.Serialize(obj));
    }
}
