using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;
using Framework.Extensions;

namespace Assets.Framework
{
    /// <summary>
    /// Can be used to handle informations abaout free mesh directions around a vertice
    /// </summary>
    public class RadialSpace
    {
        public RadialSpace(float begin, float end)
        {
            Begin = begin;
            End = end;
        }
        public RadialSpace(RadialSpace previousSpace, RadialSpace nextSpace)
        {
            PreviousRegion = previousSpace;
            NextRegion = nextSpace;
        }

        public static readonly int DegreesPerCircle = 360;
        public static float Tolerance = FreeSpace2D.Tolerance;
        private float _begin;
        private float _end;
        private RadialSpace _nextRegion;
        private RadialSpace _previousRegion;

        public float Begin
        {
            get { return _begin; }
            private set { _begin = FormatDegrees(value); }
        }

        public float End
        {
            get { return _end; }
            private set { _end = FormatDegrees(value); }
        }

        //public void SetAfter(RadialSpace region)
        //{
        //    End = region.End;
        //    OpeningVertice = region.ClosingVertice;
        //}

        //public void SetInFrontOf(RadialSpace region)
        //{
        //    End = region.Begin;
        //    ClosingVertice = region.OpeningVertice;
        //}

        public Vertice CenterVertice { get; set; }
        public Vertice OpeningVertice { get; set; }
        public Vertice ClosingVertice { get; set; }

        public RadialSpace PreviousRegion
        {
            get { return _previousRegion; }
            set
            {

                if (value == null)
                {
                    _previousRegion = null;
                    return; // return falls der neue wert null ist
                }

                if (value != this) // falls value nicht this ist, sollen Begin bzw. End von this gesetzt werden
                {
                    Begin = value.End;
                }

                value._nextRegion = this;
                OpeningVertice = value.ClosingVertice;
                _previousRegion = value;
            }
        }

        public RadialSpace NextRegion
        {
            get { return _nextRegion; }
            set
            {
                if (value == null) 
                {
                    _nextRegion = null;
                    return; // return falls der neue wert null ist
                }

                if (value != this) // falls value nicht this ist, sollen Begin bzw. End von this gesetzt werden
                {
                    End = value.Begin;
                }

                value._previousRegion = this;
                ClosingVertice = value.OpeningVertice;
                _nextRegion = value;
            }
        }

        private float FormatDegrees(float value)
        {
            if (value > DegreesPerCircle)
            {
                value = value % DegreesPerCircle;
            }

            else if (value < 0)
            {
                while (value < 0)
                {
                    // e.g. make -50 degree to 310 degree
                    value += DegreesPerCircle;
                }
            }

            return value;
        }

        public float GetSize()
        {
            float begin = Begin;
            float end = End;

            while (begin > end)
            {
                begin = begin - DegreesPerCircle;
            }
            return end - begin;
        }

        public bool IsAlmostZero()
        {
            // Are valid if not almost equal
            return !Begin.IsAlmostEqual(second: End, tolerance: Tolerance);
        }

        public bool ContainsDirection(float value)
        {
            value = FormatDegrees(value);

            bool valueIsOnEdge = (value.IsAlmostEqual(Begin, Tolerance) ||
                      value.IsAlmostEqual(End, Tolerance));

            if (valueIsOnEdge)
            {
                return false;
            }

            if (Begin > End)
            {
                return (value > Begin || value < End);
            }

            else // Der zweite ist größer als der erste
            {
                return (value > Begin && value < End);
            }

        }

        public bool Equals(RadialSpace other)
        {
            return  Begin.IsAlmostEqual(other.Begin, Tolerance) && End.IsAlmostEqual(other.End, Tolerance); // Anfang und ende stimmen

        }

        public bool IsBlocked
        {

            get
            {

                if (OpeningVertice == null || ClosingVertice == null)
                {
                    return false;
                }

                if (GetSize() > 180) // If size is bigger tha 180 the space cannot be close
                {
                    return false;
                }

                return OpeningVertice.Triangles.Any((triangle) => triangle.Contains(ClosingVertice.Index));
            }
        }

        public bool IsCompleteWithin(RadialSpace other)
        {
            return other.ContainsDirection(Begin) && other.ContainsDirection(End);
        }

        public bool BeginsBefore(RadialSpace other)
        {
            return !other.ContainsDirection(Begin); // As soon as begin doesn't lie in space, it could start before other begin
        }

        public bool EndsAfter(RadialSpace other)
        {
            return !other.ContainsDirection(End);
        }

        public bool IntersectsWith(RadialSpace other)
        {
            return other.ContainsDirection(Begin) || other.ContainsDirection(End);
        }

        public bool BeginsWith(RadialSpace other)
        {
            return Begin.IsAlmostEqual(other.Begin, Tolerance);
        }

        public bool EndsWith(RadialSpace other)
        {
            return End.IsAlmostEqual(other.End, Tolerance);
        }

        public override string ToString()
        {
            return string.Format("From \"{0}\" to \"{1}\"", Begin, End);
        }


        public static RadialSpace MergeRadialSpaces(RadialSpace first, RadialSpace second)
        {
            var mergedSpace = new RadialSpace(first.PreviousRegion, second.NextRegion);
            return mergedSpace;
        }
    }
}
