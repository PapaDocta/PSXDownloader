using System;

namespace PSXDLL
{
    public static class Logger
    {
        public static void LogError(Exception ex, string message)
        {
            Console.WriteLine($"[ERROR] {message}: {ex.Message}");
        }
    }
}
