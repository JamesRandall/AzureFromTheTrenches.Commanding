using System;

namespace AzureFromTheTrenches.Commanding.AspNetCore.Abstractions
{
    /// <summary>
    /// Command properties that contain data normally obtained from claims 
    /// Marking a command property with this attribute will prevent the model binder from attempting to
    /// deser
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class SecurityPropertyAttribute : Attribute
    {
    }
}
