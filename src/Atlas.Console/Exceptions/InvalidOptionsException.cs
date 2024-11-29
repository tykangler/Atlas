using System;

namespace Atlas.Console.Exceptions;

public class PageContentException(string message) : InvalidOperationException(message);
