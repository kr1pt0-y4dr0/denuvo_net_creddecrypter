using System;
using System.Security.Cryptography;
using System.Text;
using DenuvoNetCredDecrypter.Domain;
using DenuvoNetCredDecrypter.Services;
using Xunit;

namespace DenuvoNetCredDecrypter.Tests
{
    public sealed class HwidCalculatorTests
    {
        private readonly HwidCalculator _calculator = new HwidCalculator();

        [Fact]
        public void Uses_processor_id_when_present_and_ignores_clock_speed()
        {
            var info = new MachineInfo
            {
                ProcessorId = "PROC",
                ProcessorName = "Name",
                ProcessorManufacturer = "Maker",
                ProcessorMaxClockSpeed = "3600",
                BaseBoardManufacturer = "BB-Maker",
                BaseBoardName = "BB-Name",
                BaseBoardSerialNumber = "BB-Serial"
            };

            var result = _calculator.Compute(info);

            Assert.Equal("PROC", result.CpuId);
            Assert.Equal("BB-MakerBB-NameBB-Serial", result.BaseId);
            Assert.Equal("PROCBB-MakerBB-NameBB-Serial", result.Concatenated);
        }

        [Fact]
        public void Falls_back_to_name_plus_clock_speed()
        {
            var info = new MachineInfo
            {
                ProcessorId = string.Empty,
                ProcessorName = "Name",
                ProcessorManufacturer = "Maker",
                ProcessorMaxClockSpeed = "3600"
            };

            var result = _calculator.Compute(info);

            Assert.Equal("Name3600", result.CpuId);
        }

        [Fact]
        public void Falls_back_to_manufacturer_plus_clock_speed()
        {
            var info = new MachineInfo
            {
                ProcessorId = string.Empty,
                ProcessorName = string.Empty,
                ProcessorManufacturer = "Maker",
                ProcessorMaxClockSpeed = "3600"
            };

            var result = _calculator.Compute(info);

            Assert.Equal("Maker3600", result.CpuId);
        }

        [Fact]
        public void Falls_back_to_clock_speed_only_when_everything_is_missing()
        {
            var info = new MachineInfo
            {
                ProcessorMaxClockSpeed = "3600"
            };

            var result = _calculator.Compute(info);

            Assert.Equal("3600", result.CpuId);
        }

        [Fact]
        public void Produces_upper_case_md5_hex_of_the_concatenation()
        {
            var info = new MachineInfo
            {
                ProcessorId = "A",
                BaseBoardManufacturer = "B",
                BaseBoardName = "C",
                BaseBoardSerialNumber = "D"
            };

            var result = _calculator.Compute(info);

            // Independently computed with BitConverter to avoid mirroring the implementation.
            var expected = BitConverter
                .ToString(MD5.Create().ComputeHash(Encoding.ASCII.GetBytes("ABCD")))
                .Replace("-", string.Empty);

            Assert.Equal(expected, result.Hwid);
            Assert.Equal(result.Hwid, result.Hwid.ToUpperInvariant());
            Assert.Equal(32, result.Hwid.Length);
        }
    }
}
