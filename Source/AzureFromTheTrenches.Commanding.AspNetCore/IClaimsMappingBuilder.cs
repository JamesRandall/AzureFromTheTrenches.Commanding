using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace AzureFromTheTrenches.Commanding.AspNetCore
{
    public interface IClaimsMappingBuilder
    {
        /// <summary>
        /// Will map a claim of the given type to any property with the name propertyName.
        /// This is useful if you take a consistent approach to naming for example
        /// </summary>
        /// <param name="claimType"></param>
        /// <param name="propertyName"></param>
        /// <param name="nameComparisonType">Type of string comparison to use, default to invariant culture ignore case</param>
        /// <returns></returns>
        IClaimsMappingBuilder MapClaimToPropertyName(string claimType, string propertyName, StringComparison nameComparisonType = StringComparison.InvariantCultureIgnoreCase);

        IClaimsMappingBuilder MapClaimToCommandProperty<TCommand>(string claimType, Expression<Func<TCommand, object>> getProperty);
    }
}
