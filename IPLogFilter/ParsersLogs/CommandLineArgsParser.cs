using IPLogFilter.Models;

namespace IPLogFilter.ParsersLogs
{
    public class CommandLineArgsParser
    {
        public CommandLogOptions? ParseArgs(string[] args)
        {
            var options = new CommandLogOptions();
            for (int i = 0; i < args.Length; i += 2)
            {
                switch (args[i])
                {
                    case "--file-log":
                        options.FileLog = args[i + 1];
                        break;
                    case "--file-output":
                        options.FileOutput = args[i + 1];
                        break;
                    case "--address-start":
                        options.AddressStart = args[i + 1];
                        break;
                    case "--address-mask":
                        options.AddressMask = args[i + 1];
                        break;
                    case "--time-start":
                        options.TimeStart = DateTime.ParseExact(args[i + 1], "dd.MM.yyyy", null);
                        break;
                    case "--time-end":
                        options.TimeEnd = DateTime.ParseExact(args[i + 1], "dd.MM.yyyy", null);
                        break;
                }
            }

            return options;
        }
    }
}
