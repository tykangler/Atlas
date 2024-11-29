namespace Atlas.Console.Services;

using System;
using System.Text.Json;

public static class OutputService
{
    public static async Task WriteObjectToConsoleOrFile<T>(T obj, string? outputFile) where T : notnull
    {
        if (outputFile == null)
        {
            Console.WriteLine(SerializationService.Serialize(obj));
        }
        else
        {
            await FileUtilities.WriteToFile(obj, outputFile);
        }
    }
}