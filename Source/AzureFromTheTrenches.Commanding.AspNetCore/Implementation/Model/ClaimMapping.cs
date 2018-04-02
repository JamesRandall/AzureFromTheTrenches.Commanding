using System.Reflection;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Implementation.Model
{
    internal class ClaimMapping
    {
        public string FromClaimType { get; set; }

        public PropertyInfo ToPropertyInfo { get; set; }        
    }
}
