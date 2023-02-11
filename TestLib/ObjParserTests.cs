using FileParserLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;

namespace TestLib
{
    [TestClass]
    public class ObjParserTests
    {
        [TestMethod]
        public void ShouldParseObj()
        {
            string objPath = "./../../../../../Assets/sphere.obj";
            var parser = new ObjParser();

            var obj = parser.Parse(objPath);

            RecreateObj(obj.Vertices, obj.HasTextures);

            Assert.IsNotNull(obj);
        }

        private void RecreateObj(float[] vertices, bool hasTextures)
        {
            var v = new List<(float, float, float)>();
            var vn = new List<(float, float, float)>();
            var vt = new List<(float, float)>();
            var faces = new List<((int, int, int), (int, int, int), (int, int, int))>();

            int step = hasTextures ? 8 : 6;

            for (int j = 0; j < vertices.Length; j += 3 * step)
            {
                for (int i = j; i < j + 3 * step; i += step)
                {
                    v.Add((vertices[i], vertices[i + 1], vertices[i + 2]));
                    vn.Add((vertices[i + 3], vertices[i + 4], vertices[i + 5]));
                    if (hasTextures)
                        vt.Add((vertices[i + 6], vertices[i + 7]));
                }
                faces.Add(((v.Count - 2, vt.Count - 2, vn.Count - 2), (v.Count - 1, vt.Count - 1, vn.Count - 1), (v.Count, vt.Count, vn.Count)));
            }

            var filePath = Path.GetTempFileName();

            using var file = new StreamWriter(filePath);

            file.WriteLine("mtllib sphere.mtl");
            file.WriteLine("o Sphere");

            foreach (var x in v)
                file.WriteLine(string.Format("v {0:0.000000} {1:0.000000} {2:0.000000}", x.Item1, x.Item2, x.Item3));

            foreach (var x in vn)
                file.WriteLine(string.Format("vn {0:0.000000} {1:0.000000} {2:0.000000}", x.Item1, x.Item2, x.Item3));

            foreach (var x in vt)
                file.WriteLine(string.Format("vt {0:0.000000} {1:0.000000}", x.Item1, x.Item2));

            foreach (var face in faces)
                if(hasTextures)
                    file.WriteLine(string.Format("f {0}/{1}/{2} {3}/{4}/{5} {6}/{7}/{8}",
                        face.Item1.Item1, face.Item1.Item2, face.Item1.Item3, face.Item2.Item1, face.Item2.Item2, face.Item2.Item3, face.Item3.Item1, face.Item3.Item2, face.Item3.Item3));
                else
                    file.WriteLine(string.Format("f {0}//{1} {2}//{3} {4}//{5}",
                        face.Item1.Item1, face.Item1.Item3, face.Item2.Item1, face.Item2.Item3, face.Item3.Item1, face.Item3.Item3));

        }
    }
}