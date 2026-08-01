namespace NumeralSystems.Net.Tool;

public static class Program
{
    public static int Main(string[] args) =>
        NumsysApplication.Run(
            args,
            Console.IsInputRedirected ? Console.In : null,
            Console.Out,
            Console.Error);
}
