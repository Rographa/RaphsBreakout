using System;

namespace Utilities
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class GetComponentAttribute : Attribute
    {
        public bool SearchChildren;

        public GetComponentAttribute(bool searchChildren = true)
        {
            SearchChildren = searchChildren;
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class GetComponentsAttribute : GetComponentAttribute
    {
    }
}