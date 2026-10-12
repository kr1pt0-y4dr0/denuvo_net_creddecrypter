using DenuvoNetCredDecrypter.Abstractions;
using DenuvoNetCredDecrypter.Domain;

namespace DenuvoNetCredDecrypter.Tests.Fakes
{
    internal sealed class FakeHwidProvider : IHwidProvider
    {
        private readonly HwidComputation _computation;

        public FakeHwidProvider(HwidComputation computation)
        {
            _computation = computation;
        }

        public HwidComputation GetCurrent() => _computation;
    }
}
