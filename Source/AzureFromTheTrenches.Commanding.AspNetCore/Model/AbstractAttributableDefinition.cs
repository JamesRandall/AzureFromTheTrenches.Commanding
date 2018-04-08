using System.Collections.Generic;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Model
{
    public abstract class AbstractAttributableDefinition
    {
        public List<AttributeDefinition> Attributes { get; set; } = new List<AttributeDefinition>();
    }
}
