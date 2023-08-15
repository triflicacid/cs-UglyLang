// MAIN
using UglyLang.Source;

// Prompt user for filename
string defaultFilename = "source.txt";
Console.WriteLine("Enter the name of the file (default '" + defaultFilename + "'): ");
string filepath = Console.ReadLine() ?? "";
if (filepath.Length == 0) filepath = defaultFilename;

// Construct full filepath to current directory
string fullFilePath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
fullFilePath = Directory.GetParent(Directory.GetParent(Directory.GetParent(fullFilePath).FullName).FullName).FullName;
fullFilePath += "\\" + filepath;
Console.WriteLine("Opening " + fullFilePath + " ...");

if (File.Exists(fullFilePath))
{
    Console.WriteLine();
    string program = File.ReadAllText(fullFilePath);

    var watch = System.Diagnostics.Stopwatch.StartNew();

    Parser p = new();
    p.Parse(program);
    watch.Stop();
    long parsedTime = watch.ElapsedMilliseconds;

    if (p.Error != null)
    {
        Console.WriteLine("An error occured whilst parsing:");
        Console.WriteLine(p.GetErrorString());
    }
    else
    {
        watch.Reset();

        Context ctx = new(filepath);
        ctx.AddSource(filepath, program);
        ctx.InitialiseGlobals();
        Signal sig = p.AST.Evaluate(ctx);
        watch.Stop();
        long executionTime = watch.ElapsedMilliseconds;
        Console.WriteLine(string.Format("Program terminated with signal {0} ({1}) after {2} ms ({3} ms parsing, {4} ms execution)", (int) sig, sig.ToString(), parsedTime + executionTime, parsedTime, executionTime));
        if (ctx.Error != null)
        {
            Console.WriteLine(ctx.GetErrorString());
        }
    }
}
else
{
    Console.WriteLine("Unable to locate the source file");
}
