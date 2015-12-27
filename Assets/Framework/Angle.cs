using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Framework
{
    public class Angle : Tuple<float, float>
    {
        public static readonly int DegreesPerCircle = 360;

        public override float First
        {
            get
            {
                return base.First;
            }

            set
            {
                while( value > 360)
                {
                    value -= 360;
                }
                base.First = value;
            }
        }

        public override float Second
        {
            get
            {
                return base.Second;
            }

            set
            {
                while (value > 360)
                {
                    value -= 360;
                }
                base.Second = value;
            }
        }

        public int TargetVertice { get; set; }
        public int OpeningVertice { get; set; }
        public int ClosingVertice { get; set; }

        public MeshFilter MeshFilter { get; set; }


        Angle(float first, float second) : base(first, second)
        {

        }
     
        public static Angle New(float first, float second)
        {
            var tuple = new Angle(first, second);
            return tuple;
        }

        public float GetSize()
        {
            float begin = First;
            float end = Second;

            while (begin > end)
            {
                begin = begin - DegreesPerCircle;
            }

            return end - begin;
        }

    }
}
