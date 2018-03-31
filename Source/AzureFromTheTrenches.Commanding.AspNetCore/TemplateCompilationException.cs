using System;
using System.Collections.Generic;
using System.Text;

namespace AzureFromTheTrenches.Commanding.AspNetCore
{
    class TemplateCompilationException : Exception
    {
        public TemplateCompilationException(string message) : base(message)
        {
        }
    }
}
