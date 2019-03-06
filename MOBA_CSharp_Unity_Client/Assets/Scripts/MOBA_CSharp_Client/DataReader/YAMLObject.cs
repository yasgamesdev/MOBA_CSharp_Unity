using System.Collections.Generic;

public class YAMLObject
{
    Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();

    public T GetData<T>(string key)
    {
        if (keyValuePairs.ContainsKey(key))
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