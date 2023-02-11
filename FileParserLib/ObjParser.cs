using FileParserLib.Interfaces;
using FileParserLib.Structure;
using Assimp;
using OpenTK.Mathematics;
using System.Globalization;
using System.Numerics;

namespace FileParserLib
{
    public class ObjParser : IParser
    {
        private readonly List<(float, float, float)> vertices = new List<(float, float, float)>();
        private readonly List<(float, float, float)> normals = new List<(float, float, float)>();
        private readonly List<(float, float)> textures = new List<(float, float)>();

        public (float[] Vertices, bool HasTextures) Parse(string fileName)
        {
            var ret = new List<float>();

            if (!File.Exists(fileName))
                return (null, false);

            foreach (var line in File.ReadLines(fileName))
            {
                if(string.IsNullOrWhiteSpace(line)) continue;

                switch (line.Substring(0, 2))
                {
                    case "v ":
                        var pointSplit = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        vertices.Add((
                            float.Parse(pointSplit[1], CultureInfo.InvariantCulture),
                            float.Parse(pointSplit[2], CultureInfo.InvariantCulture),
                            float.Parse(pointSplit[3], CultureInfo.InvariantCulture)
                            ));
                        break;

                    case "vn":
                        var normalSplit = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        normals.Add((
                            float.Parse(normalSplit[1], CultureInfo.InvariantCulture),
                            float.Parse(normalSplit[2], CultureInfo.InvariantCulture),
                            float.Parse(normalSplit[3], CultureInfo.InvariantCulture)
                            ));
                        break;

                    case "vt":
                        var textureSplit = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        textures.Add((
                            float.Parse(textureSplit[1], CultureInfo.InvariantCulture),
                            float.Parse(textureSplit[2], CultureInfo.InvariantCulture)
                            ));
                        break;

                    case "f ":
                        var faceSplit = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        if(faceSplit.Length == 4) // triangle
                        {
                            foreach (var vertex in faceSplit.Skip(1))
                                AddVertex(vertex, ret);
                        }
                        else if(faceSplit.Length == 5) // quadruple
                        {
                            AddVertex(faceSplit[1], ret);
                            AddVertex(faceSplit[2], ret);
                            AddVertex(faceSplit[3], ret);
                            AddVertex(faceSplit[1], ret);
                            AddVertex(faceSplit[3], ret);
                            AddVertex(faceSplit[4], ret);
                        }
                        break;

                    default:
                        break;
                }
            }

            return (ret.ToArray(), textures.Any());
        }

        private void AddVertex(string vertex, List<float> ret)
        {
            var vertexSplit = vertex.Split('/');
            int v = int.Parse(vertexSplit[0]);
            if (!int.TryParse(vertexSplit[1], out int vt))
                vt = -1;
            int vn = int.Parse(vertexSplit[2]);

            ret.Add(vertices[v - 1].Item1);
            ret.Add(vertices[v - 1].Item2);
            ret.Add(vertices[v - 1].Item3);
            ret.Add(normals[vn - 1].Item1);
            ret.Add(normals[vn - 1].Item2);
            ret.Add(normals[vn - 1].Item3);
            if (vt > 0)
            {
                ret.Add(textures[vt - 1].Item1);
                ret.Add(textures[vt - 1].Item2);
            }
        }
    }
}