using IPLogFilter.Commands.Contracts;
using IPLogFilter.Models;
using System.Net;
using System.Net.Sockets;

namespace IPLogFilter.Commands
{
    public class FilterLogsCommand : ILogCommand
    {
        private readonly CommandLogOptions _options;

        public FilterLogsCommand(CommandLogOptions options)
        {
            _options = options;
        }

        public void Execute()
        {
            if (_options.LogRecords == null || !_options.LogRecords.Any())
            {
                throw new ArgumentNullException("Отсутствуют логи!");
            }
            if (!string.IsNullOrEmpty(_options.AddressStart))
            {
                IPAddress.TryParse(_options.AddressStart, out IPAddress startAdress);
                if (!string.IsNullOrEmpty(_options.AddressMask))
                {
                    IPAddress.TryParse(_options.AddressMask, out IPAddress addressMask);
                    _options.LogRecords = _options.LogRecords
                        .Where(log => IsIpAddressInRangeWithMask(log.IPAddress, startAdress, addressMask)).ToList();
                }
                else
                {
                    _options.LogRecords = _options.LogRecords
                        .Where(log => IsIpAddressInRange(log.IPAddress, startAdress)).ToList();
                }
            }

            _options.LogRecords = _options.LogRecords
                .Where(log => log.TimeVisit >= _options.TimeStart && log.TimeVisit <= _options.TimeEnd).ToList();
        }

        private bool IsIpAddressInRangeWithMask(IPAddress ipAddress, IPAddress startAddress, IPAddress addressMask)
        {
            var startAddressBytes = startAddress.GetAddressBytes();
            Array.Reverse(startAddressBytes);
            var addressMaskBytes = addressMask.GetAddressBytes();
            Array.Reverse(addressMaskBytes);
            var ipAddressBytes = ipAddress.GetAddressBytes();
            Array.Reverse(ipAddressBytes);

            if (startAddress.AddressFamily == AddressFamily.InterNetwork)
            {
                var ipAddressToInt = BitConverter.ToUInt32(ipAddressBytes);

                return BitConverter.ToUInt32(startAddressBytes, 0) <= ipAddressToInt && ipAddressToInt <= BitConverter.ToUInt32(addressMaskBytes, 0);
            }

            return false;
        }

        private bool IsIpAddressInRange(IPAddress ipAddress, IPAddress startAddress)
        {
            var startAddressBytes = startAddress.GetAddressBytes();
            Array.Reverse(startAddressBytes);
            var ipAddressBytes = ipAddress.GetAddressBytes();
            Array.Reverse(ipAddressBytes);

            if (startAddress.AddressFamily == AddressFamily.InterNetwork)
            {
                var ipAddressToInt = BitConverter.ToUInt32(ipAddressBytes);

                return BitConverter.ToUInt32(startAddressBytes, 0) <= ipAddressToInt;
            }

            return false;
        }
    }
}
