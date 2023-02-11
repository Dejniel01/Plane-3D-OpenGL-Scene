using FileParserLib.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileParserLib.Interfaces
{
    internal interface IParser
    {
        (float[] Vertices, bool HasTextures) Parse(string fileName);
    }
}
