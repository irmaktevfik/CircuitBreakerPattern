﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitBreaker
{
    public enum CircuitState
    {
        Open,
        Closed,
        HalfOpen
    }
}
