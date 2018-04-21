using System;
using System.Reflection;

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
            string outputDirectory = args[1];

            Assembly assembly = Assembly.LoadFile(inputAssemblyFile);
            
        }
    }
}
