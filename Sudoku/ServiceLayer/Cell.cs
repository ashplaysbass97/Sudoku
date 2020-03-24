using System;
using System.Drawing;

namespace Sudoku.ServiceLayer
{
    public class Cell : IEquatable<Cell>
    {
        public Point Coordinates { get; set; }
        public Point Region { get; set; }
        public int? Value { get; set; }
        public int? Solution { get; set; }
        public bool Editable { get; set; }

        public bool Equals(Cell other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Coordinates.Equals(other.Coordinates) && Region.Equals(other.Region) && Value == other.Value && Solution == other.Solution && Editable == other.Editable;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Cell) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Coordinates, Region, Value, Solution, Editable);
        }
    }
}