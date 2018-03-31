using System;

namespace AzureFromTheTrenches.Commanding.AspNetCore
{
    class TemplateCompilationException : Exception
    {
        public TemplateCompilationException(string message) : base(message)
        {
        }
    }
}
