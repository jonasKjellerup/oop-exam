using System.Collections.Generic;

namespace oop_exam.Util
{
    public class IdDistributor
    {
        private readonly Dictionary<string, uint> _counters = new();

        public uint NextId<T>()
        {
            var typeName = typeof(T).FullName!;
            if (_counters.TryGetValue(typeName, out var id) is false)
                id = 0;
            return _counters[typeName] = id + 1;
        }

        public void Notify<T>(uint id)
        {
            var typeName = typeof(T).FullName!;
            if (_counters.TryGetValue(typeName, out var storedId) && storedId >= id)
                return;
            _counters[typeName] = id;
        }
    }
}