using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOBA_CSharp_Server.Library.DataReader
{
    public class YAMLObject
    {
        Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();

        public T GetData<T>(string key)
        {
            if(keyValuePairs.ContainsKey(key))
            {
                return (T)keyValuePairs[key];
            }
            else
            {
                return default(T);
            }
        }

        public void AddData(string key, object value)
        {
            keyValuePairs.Add(key, value);
        }
    }
}
