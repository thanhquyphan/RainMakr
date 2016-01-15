using System;
using Microsoft.SPOT;

namespace RainMakr.Core.Web
{
    using System.Collections;

    public interface IEndPointProvider
    {
        void Initialize();
        ArrayList AvailableEndPoints();
    }
}