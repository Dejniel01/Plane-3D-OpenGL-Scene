using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileParserLib.Structure
{
    public class ParserResponseData
    {
        public Vector3[] Vertices { get; set; } = Array.Empty<Vector3>();
        public Vector3[] Normals { get; set; } = Array.Empty<Vector3>();
        public Vector2[] Textures { get; set; } = Array.Empty<Vector2>();
        public (uint, uint, uint)[] Faces { get; set; } = Array.Empty<(uint, uint, uint)>();
    }
}
