using System.Collections;
using System.Text;

namespace Sudoku.ServiceLayer
{
    internal class Candidate : IEnumerable
    {
        private readonly int _gridSize;
        private readonly bool[] _values;

        public int Count { get; private set; }

        public Candidate(int gridSize, bool initialValue)
        {
            _gridSize = gridSize;
            _values = new bool[gridSize];
            Count = 0;

            for (int i = 1; i <= gridSize; i++)
            {
                this[i] = initialValue;
            }
        }

        public bool this[int key]
        {
            // Allows candidates to be referenced by their actual value
            get => _values[key - 1];

            // Automatically tracks the number of candidates
            set
            {
                Count += _values[key - 1] == value ? 0 : value ? 1 : -1;
                _values[key - 1] = value;
            }
        }

        public void SetAll(bool value)
        {
            for (int i = 1; i <= _gridSize; i++)
            {
                this[i] = value;
            }
        }

        public override string ToString()
        {
            StringBuilder values = new StringBuilder();
            foreach (int candidate in this)
            {
                values.Append(candidate);
            }
            return values.ToString();
        }

        public IEnumerator GetEnumerator()
        {
            return new CandidateEnumerator(this);
        }

        private class CandidateEnumerator : IEnumerator
        {
            private int _position;
            private readonly Candidate _candidate;

            public CandidateEnumerator(Candidate candidate)
            {
                _candidate = candidate;
                _position = 0;
            }

            // Only iterates over valid candidates
            public bool MoveNext()
            {
                ++_position;
                return _position <= _candidate._gridSize && (_candidate[_position] || MoveNext());
            }

            public void Reset()
            {
                _position = 0;
            }

            public object Current => _position;
        }
    }
}
