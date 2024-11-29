using System;

namespace Atlas.Console.Exceptions;

public class InvalidOptionsException(string message) : InvalidOperationException(message);
