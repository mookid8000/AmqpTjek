using System;
using System.Threading.Tasks;
using Rebus.Handlers;

namespace AmqpTjek
{
    public class StringHandler : IHandleMessages<string>
    {
        public async Task Handle(string message)
        {
            Console.WriteLine($"Got string: {message}");
        }
    }
}