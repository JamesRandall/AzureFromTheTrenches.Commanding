using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace AzureFromTheTrenches.Commanding.Tests.Performance.Console.Model.Mediatr
{
    class SimpleMediatrRequestHandler : IRequestHandler<SimpleMediatrRequest, SimpleResult>
    {
        public SimpleMediatrRequestHandler()
        {
            
        }

        public Task<SimpleResult> Handle(SimpleMediatrRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult<SimpleResult>(null);
        }
    }
}
