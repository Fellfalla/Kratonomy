using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assets.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Framework.Extensions.Tests
{
    [TestClass()]
    public class Vector3ExtensionsTests
    {
        [TestMethod()]
        public void ClockwiseAngleToTest()
        {
            var normalVector = Vector3.forward;


            var vec1 = new Vector3(-1,0,0);
            var vec2 = new Vector3(0,1,0);
            var vec3 = new Vector3(1,1,0);
            var vec4 = new Vector3(0,0,1);
            var vec5 = new Vector3(1,0,0);
            var vec6 = new Vector3(-1,-1,0);

            Assert.AreEqual(270f,vec1.PlanarAngle(vec2, normalVector));
            Assert.AreEqual(225f,vec1.PlanarAngle(vec3, normalVector));
            Assert.AreEqual(0,vec1.PlanarAngle(vec4, normalVector));
            Assert.AreEqual(180f,vec1.PlanarAngle(vec5, normalVector));

            Assert.AreEqual(45f, vec1.PlanarAngle(vec6, normalVector));
            Assert.AreEqual(315f,vec6.PlanarAngle(vec1, normalVector));

        }
    }
}