using System;

namespace AzureFromTheTrenches.Commanding.Abstractions
{
    /// <summary>
    /// Used to mark command properties as a security risk and for extensions to behave appropriately.
    /// For example UserId, if normally obtained from a claim, would be dangerous to serialize through a REST
    /// API and the Asp.Net extensions will interpret this and ensure that it is excluded from binding
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class SecurityPropertyAttribute : Attribute
    {
    }
}
