using UglyLang.Execution;

bool isTesting = false;

if (isTesting)
{
    Main.ParseExamples();
}
else
{
    // Prompt user for filename
    string defaultFilename = "source.txt";
    Console.WriteLine("Enter the name of the file (default '" + defaultFilename + "'): ");
    string filepath = Console.ReadLine() ?? "";
    if (filepath.Length == 0) filepath = defaultFilename;

    Main.ReadAndExecute(Main.GetBaseDirectory(), filepath);
}
