using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Rebus.Bus;

namespace AmqpTjek
{
    class PeriodicPublisher : IDisposable
    {
        readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public PeriodicPublisher(IContainer container)
        {
            var bus = container.Resolve<IBus>();

            Task.Run(() => Run(bus, _cancellationTokenSource.Token));
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
        }

        static async Task Run(IBus bus, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    await bus.SendLocal($"Hej med dig, klokken er {DateTime.Now}");

                    await Task.Delay(TimeSpan.FromSeconds(1), token);
                }
                catch (OperationCanceledException)
                {
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }
                }
            }
        }
    }
}