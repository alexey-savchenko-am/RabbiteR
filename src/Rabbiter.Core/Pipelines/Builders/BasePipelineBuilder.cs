namespace Rabbiter.Core.Pipelines.Builders
{
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public abstract class BaseBuilder<TIn, TOut, TBuilder>
        : IDisposable
        where TBuilder : BaseBuilder<TIn, TOut, TBuilder>, new()
    {

        private readonly LinkedList<StepHandler<TIn, TOut>> _stepHandlers
            = new LinkedList<StepHandler<TIn, TOut>>();

        private static TBuilder _builderInstance;
        private IServiceScope _scope;
        private StepHandler<TIn, TOut> _finalStep;

        public static TBuilder StartWith<TPipelineStep>(IServiceScopeFactory serviceScopeFactory)
            where TPipelineStep : IPipelineStep<TIn, TOut>
        {
            _builderInstance = new TBuilder();
            _builderInstance._scope = serviceScopeFactory.CreateScope();

            StepHandler<TIn, TOut> handler = async (TIn data, Func<TIn, Task<TOut>> next) =>
            {
                var step = _builderInstance._scope.ServiceProvider.GetRequiredService<TPipelineStep>();
                return await step.InvokeAsync(data, next);
            };

            return _builderInstance.AddStep(handler);
        }

        public TBuilder AddStep<TPipelineStep>()
            where TPipelineStep : IPipelineStep<TIn, TOut>
        {
            StepHandler<TIn, TOut> handler = async (TIn data, Func<TIn, Task<TOut>> next) =>
            {
                var step = _builderInstance._scope.ServiceProvider.GetRequiredService<TPipelineStep>();
                return await step.InvokeAsync(data, next);

            };

            return AddStep(handler);
        }

        

        public Func<TIn, Task<TOut>> EndWithAndBuild<TPipelineStep>()
            where TPipelineStep : IPipelineStep<TIn, TOut>
        {

            _finalStep = async (TIn data, Func<TIn, Task<TOut>> next) =>
            {
                var step = _builderInstance._scope.ServiceProvider.GetRequiredService<TPipelineStep>();

                return await step.InvokeAsync(data, null);
            };

            return BuildChain(_stepHandlers.First);

        }


        private TBuilder AddStep(StepHandler<TIn, TOut> step)
        {
            _stepHandlers.AddLast(step);
            return _builderInstance;
        }


        private Func<TIn, Task<TOut>> BuildChain(LinkedListNode<StepHandler<TIn, TOut>> node)
        {
            if (node == null)
                return async input => await _finalStep.Invoke(input, null);

            return async input =>
                await node.Value
                    .Invoke(input, BuildChain(node.Next))
                    .ConfigureAwait(false);
        }

        public void Dispose()
        {
            if(_scope != null)
                _scope.Dispose();
        }
    }
}
