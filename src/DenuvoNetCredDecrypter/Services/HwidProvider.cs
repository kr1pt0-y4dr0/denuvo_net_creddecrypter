using DenuvoNetCredDecrypter.Abstractions;
using DenuvoNetCredDecrypter.Domain;

namespace DenuvoNetCredDecrypter.Services
{
    /// <summary>
    /// Combines machine information with the HWID algorithm.
    /// </summary>
    public sealed class HwidProvider : IHwidProvider
    {
        private readonly IMachineInfoProvider _machineInfoProvider;
        private readonly HwidCalculator _calculator;

        public HwidProvider(IMachineInfoProvider machineInfoProvider, HwidCalculator calculator)
        {
            _machineInfoProvider = machineInfoProvider;
            _calculator = calculator;
        }

        public HwidComputation GetCurrent()
        {
            return _calculator.Compute(_machineInfoProvider.Read());
        }
    }
}
