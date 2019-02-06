using System;
using System.Collections.Generic;
using System.Text;

namespace BigInt.Core
{
    public interface IBigIntParser
    {
        BigInt Parse(string str);
    }
}
