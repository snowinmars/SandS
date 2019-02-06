using System;
using System.Collections.Generic;
using System.Text;

namespace Common.SandS
{
    public enum ContentType
    {
        Json
    }

    public enum HttpVerbs
    {
        Get = 1,
        Post = 2,
        Put = 4,
        Delete = 8,
        Head = 16,
        Patch = 32,
        Options = 64,
    }
}
