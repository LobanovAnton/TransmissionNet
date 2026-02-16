using System.Numerics;

namespace TransmissionNet.Utils
{
    public class MedianFilter<T> where T : unmanaged, IBinaryInteger<T>
    {
        private readonly int _filterWindowSize;
        private readonly Queue<T> _values;
        private readonly int _count;
        private readonly int _index;
        private T[] _sortedValues = [];

        public MedianFilter(int filterWindowSize, int percentageValuesForAveraging)
        {
            _filterWindowSize = (int)Math.Round(filterWindowSize / 2.0) * 2;
            _count = (int)Math.Round(_filterWindowSize * percentageValuesForAveraging / 100.0);
            _index = (_filterWindowSize - _count) >> 1;
            _values = new Queue<T>(_filterWindowSize);
        }

        public void AddValue(T value)
        {
            if (_values.Count == _filterWindowSize)
                _values.Dequeue();

            _values.Enqueue(value);
            _sortedValues = _index + _count > _values.Count ?
                _values.Order().ToArray() :
                _values.Order().Skip(_index).Take(_count).ToArray();
        }

        public T GetValue()
        {
            T result = default;

            foreach (T value in _sortedValues)
                result += value;

            return result / T.CreateTruncating(_sortedValues.Length);
        }
    }
}
