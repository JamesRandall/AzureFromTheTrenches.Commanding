using System;
using System.Collections.Generic;
using System.Text;

namespace AccidentalFish.Commanding.Implementation
{
    internal interface ICommandAuditPipeline
    {
        void RegisterAuditor<TAuditorImpl>() where TAuditorImpl : ICommandAuditor;
    }
}
