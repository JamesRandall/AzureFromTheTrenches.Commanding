using System;
using System.Collections.Generic;
using System.Text;

namespace HttpCommanding.Model.Results
{
    public class UpdateResult
    {
        public string ValidationMessage { get; set; }

        public bool DidUpdate { get; set; }
    }
}
