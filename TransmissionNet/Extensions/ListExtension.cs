namespace TransmissionNet.Extensions;

public static class ListExtension
{
    public static int FindIndex<T>(this IList<T> list, Predicate<T> predicate)
    {
        int index = 0;
        foreach (T t in list)
        {
            if (predicate(t))
                return index;
            index++;
        }
        return -1;
    }
}