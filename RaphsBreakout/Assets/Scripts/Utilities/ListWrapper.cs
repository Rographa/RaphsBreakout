using System;
using System.Collections.Generic;

namespace Utilities
{
    [Serializable]
    public class ListWrapper
    {
        public List<int> list;

        public ListWrapper(int capacity)
        {
            list = new(capacity);
        }
    }
}