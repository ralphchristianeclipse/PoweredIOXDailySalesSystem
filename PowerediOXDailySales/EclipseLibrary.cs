using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PowerediOXDailySales
{
    public class MultipleDictionary : Dictionary<string, MultipleDictionary>
    {

        public object Value;
        public T GetValue<T>()
        {
            return (T)Value;
        }
        Dictionary<string, MultipleDictionary> _holder = new Dictionary<string, MultipleDictionary>();

        new public MultipleDictionary this[string key]
        {
            get
            {
                if (!_holder.ContainsKey(key))
                {
                    _holder.Add(key, new MultipleDictionary());
                    this.Add(key, _holder[key]);
                }
                return _holder[key];
            }
        }

        public bool HasKeys()
        {
            return this.Values.Count > 0;
        }

        public override string ToString()
        {
            return (Value ?? string.Empty).ToString();
        }
        public IEnumerable<MultipleDictionary> GetAllMultipleDictionary(MultipleDictionary keys,bool showFirstKey = false)
        {
            if(showFirstKey)
                yield return keys;
            foreach(var key in keys)
            {
                yield return key.Value;
                if (key.Value.HasKeys())
                    foreach (var keyChild in GetAllMultipleDictionary(key.Value))
                        yield return keyChild;
            }
        }
	}
}