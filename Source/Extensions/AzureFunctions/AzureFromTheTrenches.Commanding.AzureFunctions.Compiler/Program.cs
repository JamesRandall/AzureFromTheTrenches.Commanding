using System;
using System.IO;
using System.Reflection;
using AzureFromTheTrenches.Commanding.AzureFunctions.Compiler.Implementation;

namespace AzureFromTheTrenches.Commanding.AzureFunctions.Compiler
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                throw new ArgumentException("Must specify at least <ASSMEBLY_FILE> and <OUTPUT_DIR>");
            }

            string inputAssemblyFile = args[0];
            string outputFunctionDirectory = args[1];

            Assembly assembly = Assembly.LoadFile(inputAssemblyFile);
            string outputBinaryDirectory = Path.GetDirectoryName(assembly.Location);

            FunctionCompiler compiler = new FunctionCompiler(assembly, outputBinaryDirectory, outputFunctionDirectory);
            compiler.Compile().Wait();
        }
    }
}
