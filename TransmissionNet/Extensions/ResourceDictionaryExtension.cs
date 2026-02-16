namespace TransmissionNet.Extensions;

public static class ResourceDictionaryExtension
{
    public static T[] GetValuesByKeys<T>(this ResourceDictionary resources, string[] keys,
                                         int start, int count, T[] destination)
    {
        T[] values = new T[count];

        for (int i = start; i < count; i++)
        {
            if (resources.TryGetValue(keys[i], out object value))
                destination[i] = (T)value;
        }

        return values;
    }
}