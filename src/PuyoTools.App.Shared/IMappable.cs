using System;
using System.Collections.Generic;
using System.Text;

namespace PuyoTools.App
{
    interface IMappable<T>
    {
        void MapTo(T obj);
    }
}
