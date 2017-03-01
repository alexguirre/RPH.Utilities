namespace RPH.Utilities.AI
{
    // System
    using System;
    using System.Collections.Generic;

    public class Blackboard
    {
        private readonly Dictionary<string, object> globalMemory = new Dictionary<string, object>();
        private readonly Dictionary<Guid, Dictionary<string, object>> treeMemory = new Dictionary<Guid, Dictionary<string, object>>();

        protected Dictionary<string, object> GetTreeMemory(Guid treeScope)
        {
            if (!treeMemory.ContainsKey(treeScope))
            {
                treeMemory.Add(treeScope, new Dictionary<string, object>()
                {
                    { "nodeMemory", new Dictionary<Guid, Dictionary<string, object>>()  },
                    { "openNodes", new List<Guid>() }
                });
            }

            return treeMemory[treeScope];
        }

        protected Dictionary<string, object> GetNodeMemory(Dictionary<string, object> treeMemory, Guid nodeScope)
        {
            Dictionary<Guid, Dictionary<string, object>> memory = (Dictionary<Guid, Dictionary<string, object>>)treeMemory["nodeMemory"];

            if (!memory.ContainsKey(nodeScope))
            {
                memory.Add(nodeScope, new Dictionary<string, object>());
            }

            return memory[nodeScope];
        }

        protected Dictionary<string, object> GetMemory(Guid? treeScope = null, Guid? nodeScope = null)
        {
            if (nodeScope.HasValue && !treeScope.HasValue)
            {
                throw new InvalidOperationException("Can't pass null to 'treeScope' if 'nodeScope' isn't null.");
            }

            Dictionary<string, object> memory = globalMemory;

            if (treeScope.HasValue)
            {
                memory = GetTreeMemory(treeScope.Value);

                if (nodeScope.HasValue)
                {
                    memory = GetNodeMemory(memory, nodeScope.Value);
                }
            }

            return memory;
        }


        public object Get(string key, Guid? treeScope = null, Guid? nodeScope = null, object defaultValue = null)
        {
            Dictionary<string, object> memory = GetMemory(treeScope, nodeScope);
            if (!memory.ContainsKey(key))
            {
                memory.Add(key, defaultValue);
                return defaultValue;
            }

            return memory[key];
        }
        
        public T Get<T>(string key, Guid? treeScope = null, Guid? nodeScope = null, T defaultValue = default(T))
        {
            return (T)Get(key, treeScope, nodeScope, (object)defaultValue);
        }

        public void Set(string key, object value, Guid? treeScope = null, Guid? nodeScope = null)
        {
            Dictionary<string, object> memory = GetMemory(treeScope, nodeScope);
            if (!memory.ContainsKey(key))
            {
                memory.Add(key, value);
            }
            else
            {
                memory[key] = value;
            }
        }

        public void Set<T>(string key, T value, Guid? treeScope = null, Guid? nodeScope = null)
        {
            Set(key, (object)value, treeScope, nodeScope);
        }
    }
}
