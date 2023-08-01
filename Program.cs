// MAIN
using UglyLang.Source;

//Console.WriteLine("Enter the name of the file: ");
//string filepath = Console.ReadLine() ?? "";
//Console.WriteLine();
string filepath = "source.txt";

string fullFilePath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
fullFilePath = Directory.GetParent(Directory.GetParent(Directory.GetParent(fullFilePath).FullName).FullName).FullName;
fullFilePath += "\\" + filepath;

if (File.Exists(fullFilePath))
{
    string program = File.ReadAllText(fullFilePath);

    Parser p = new();
    p.Parse(program);
    if (p.Error != null)
    {
        Console.WriteLine("An error occured whilst parsing:");
        Console.WriteLine(p.Error.ToString());
    }
    else
    {
        Context ctx = new(filepath);
        Signal sig = p.AST.Evaluate(ctx);
        Console.WriteLine(string.Format("Program terminated with signal {0} ({1})", sig.ToString(), (int) sig));
        if (ctx.Error != null)
        {
            Console.WriteLine(ctx.GetErrorString());
        }
    }
}
else
{
    Console.WriteLine(string.Format("Unable to locate the source file - {0}", fullFilePath));
}
