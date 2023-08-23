using UglyLang.Source;

namespace UglyLang.Execution
{
    public class Main
    {
        /// <summary>
        /// Return the absolute path to the base directory of the application.
        /// </summary>
        public static string GetBaseDirectory()
        {
            string baseDir = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
            baseDir = Directory.GetParent(Directory.GetParent(Directory.GetParent(baseDir).FullName).FullName).FullName;
            return baseDir;
        }

        /// <summary>
        /// Given a file path, attempt to read, and parse the file. Return success.
        /// </summary>
        public static bool ReadAndParse(string baseDir, string relPath)
        {
            string path = Path.Join(baseDir, relPath);

            Console.WriteLine("Opening " + path + " ...");

            if (File.Exists(path))
            {
                string program = File.ReadAllText(path);

                var watch = System.Diagnostics.Stopwatch.StartNew();

                ParseOptions options = new(baseDir);
                Parser p = new(options);
                p.ParseSource(relPath, program);
                watch.Stop();

                if (p.IsError())
                {
                    Console.WriteLine("An error occured whilst parsing:");
                    Console.WriteLine(p.GetErrorString());
                    return false;
                }
                else
                {
                    Console.WriteLine(string.Format("File successfully parsed in {0} ms", watch.ElapsedMilliseconds));
                    return true;
                }
            }
            else
            {
                Console.WriteLine("Unable to locate the source file");
                return false;
            }
        }

        /// <summary>
        /// Given a file path, attempt to read, parse then execute the file. Return success.
        /// </summary>
        public static bool ReadAndExecute(string baseDir, string relPath)
        {
            string path = Path.Join(baseDir, relPath);

            Console.WriteLine("Opening " + path + " ...");

            if (File.Exists(path))
            {
                Console.WriteLine();
                string program = File.ReadAllText(path);

                var watch = System.Diagnostics.Stopwatch.StartNew();

                ParseOptions options = new(baseDir);
                Parser p = new(options);
                p.ParseSource(relPath, program);
                watch.Stop();
                long parsedTime = watch.ElapsedMilliseconds;

                if (p.IsError())
                {
                    Console.WriteLine("An error occured whilst parsing:");
                    Console.WriteLine(p.GetErrorString());
                    return false;
                }
                else
                {
                    watch.Reset();
                    watch.Start();

                    Context ctx = new(options, relPath);
                    ctx.InitialiseGlobals();

                    Signal sig = p.GetAST().Evaluate(ctx);
                    watch.Stop();
                    long executionTime = watch.ElapsedMilliseconds;
                    Console.WriteLine(string.Format("Program terminated with signal {0} ({1}) after {2} ms ({3} ms parsing, {4} ms execution)", (int)sig, sig.ToString(), parsedTime + executionTime, parsedTime, executionTime));
                    if (ctx.Error == null)
                    {
                        return true;
                    }
                    else
                    {
                        Console.WriteLine(ctx.GetErrorString());
                        return false;
                    }
                }
            }
            else
            {
                Console.WriteLine("Unable to locate the source file");
                return false;
            }
        }

        public static double ParseExamples()
        {
            string baseDir = Path.Join(GetBaseDirectory(), "Examples");
            string[] filePaths = Directory.GetFiles(baseDir, "*.txt", SearchOption.AllDirectories);
            int counter = 0;
            foreach (string filePath in filePaths)
            {
                if (ReadAndParse(baseDir, filePath.Replace(baseDir, "")))
                {
                    counter++;
                    Console.WriteLine();
                }
            }

            Console.WriteLine(Environment.NewLine + "~> Ran " + filePaths.Length + " tests. Pass rate: " + counter + "/" + filePaths.Length);
            return counter / filePaths.Length;
        }
    }
}
