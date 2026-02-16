namespace TransmissionNet.Extensions;

public enum SortMode
{
    Ascending,
    Descending
}

public static class LinqExtension
{
    public static IEnumerable<TSource> OrderByMode<TSource, TKey>(this IEnumerable<TSource> source, SortMode sortMode, Func<TSource, TKey> keySelector)
    {
        switch (sortMode)
        {
            case SortMode.Ascending:
                return source.OrderBy(keySelector);
            case SortMode.Descending:
            default:
                return source.OrderByDescending(keySelector);
        }
    }
}