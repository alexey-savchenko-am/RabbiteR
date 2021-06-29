namespace Rabbiter.Core.Pipelines.Builders
{
    public class PipelineBuilder<T>
    : BaseBuilder<T, T, PipelineBuilder<T>>
    { }

    public class PipelineBuilder<TIn, TOut>
        : BaseBuilder<TIn, TOut, PipelineBuilder<TIn, TOut>>
    { }

}
