﻿namespace Rabbiter.Core.Pipelines.Builders
{
    using System;
    using System.Threading.Tasks;

    public delegate Task<TOut> StepHandler<TIn, TOut>(TIn data, Func<TIn, Task<TOut>> next);
}
