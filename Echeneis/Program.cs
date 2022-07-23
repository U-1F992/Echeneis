using Echeneis;

using (var orca = new OrcaWrapper())
{
    Console.CancelKeyPress += (object? sender, ConsoleCancelEventArgs eventArgs) =>
    {
        orca.Dispose();
    };
    
    Console.WriteLine(@"
    1. Connect to SerialPort.
    2. Load script and compile.
    3. (Recommended) Disable KeyboradController. Setting > 「マクロ終了後にキーボードモードに移行」
    4. Press Enter to continue...");
    Console.ReadLine();

    orca.Connected = true;
    orca.Compiled = true;

    do
    {
        orca.Ternimate();
        orca.Connect();
        orca.Compile();
        await orca.Run();

    } while (!new CriteriaImpl().Verify());

    Console.WriteLine("Press Enter to exit...");
    Console.ReadLine();
}
