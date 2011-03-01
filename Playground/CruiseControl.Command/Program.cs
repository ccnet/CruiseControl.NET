namespace CruiseControl.Command
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using CruiseControl.Common;

    class Program
    {
        private static readonly char[] shortCuts = new[] { '?', '.', '~', '-', '+' };

        static void Main(string[] args)
        {
            var connection = new ServerConnection(args[0]);
            var exit = false;
            WriteToConsole(ConsoleColor.White, "CruiseControl.NET: Interactive Console");
            WriteToConsole(ConsoleColor.White, new string('=', 40));
            WriteToConsole(ConsoleColor.Yellow, "Retrieving server name...");
            var fullUrn = connection.RetrieveServerName();
            WriteToConsole(ConsoleColor.Yellow, "...server name is '{0}'", fullUrn);
            var shortUrn = fullUrn.Substring(10);
            while (!exit)
            {
                Console.Write(shortUrn + ">");
                var command = ReadFromConsole();
                switch (command.Name)
                {
                    case "exit":
                        exit = true;
                        break;

                    case "ping":
                    case "~":
                        RunPingCommand(connection);
                        break;

                    case "query":
                    case "?":
                        RunQueryCommand(connection, fullUrn);
                        break;

                    case "invoke":
                    case ".":
                        RunInvokeCommand(connection, fullUrn, command.Arguments);
                        break;

                    case "up":
                    case "-":
                        if (shortUrn.Contains(":"))
                        {
                            shortUrn = shortUrn.Substring(0, shortUrn.LastIndexOf(':'));
                            fullUrn = fullUrn.Substring(0, fullUrn.LastIndexOf(':'));
                        }

                        break;

                    case "down":
                    case "+":
                        if (command.Arguments.Length > 0)
                        {
                            fullUrn += ":" + command.Arguments[0];
                            shortUrn += ":" + command.Arguments[0];
                        }

                        break;
                }
            }
        }

        private static Command ReadFromConsole()
        {
            var input = Console.ReadLine() ?? string.Empty;
            if ((input.Length > 1) && (shortCuts.Contains(input[0])))
            {
                input = input.Substring(0, 1) + " " + input.Substring(1);
            }

            var parts = input.Split(' ');
            var command = new Command();
            if (parts.Length > 0)
            {
                command.Name = parts[0].ToLowerInvariant();
                command.Arguments = parts.Skip(1).ToArray();
            }

            return command;
        }

        private static void WriteToConsole(ConsoleColor colour, string message, params object[] args)
        {
            var oldColour = Console.ForegroundColor;
            try
            {
                Console.ForegroundColor = colour;
                if (args.Length == 0)
                {
                    Console.WriteLine(message);
                }
                else
                {
                    Console.WriteLine(message, args);
                }
            }
            finally
            {
                Console.ForegroundColor = oldColour;
            }
        }

        private static void RunPingCommand(ServerConnection connection)
        {
            WriteToConsole(ConsoleColor.Yellow, "Sending ping...");
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var result = connection.Ping();
            stopwatch.Stop();
            if (result)
            {
                WriteToConsole(ConsoleColor.Yellow, "...ping received in {0}ms", stopwatch.ElapsedMilliseconds);
            }
            else
            {
                WriteToConsole(ConsoleColor.Red, "...ping failed!", stopwatch.ElapsedMilliseconds);
            }
        }

        private static void RunQueryCommand(ServerConnection connection, string fullUrn)
        {
            WriteToConsole(ConsoleColor.Yellow, "Querying '{0}'...", fullUrn);
            try
            {
                var result = connection.Query(
                    fullUrn,
                    new QueryArguments { DataToInclude = DataDefinitions.Both });
                WriteToConsole(ConsoleColor.Yellow, "...{0} action(s) retrieved", result.Length);
                foreach (var action in result)
                {
                    WriteToConsole(ConsoleColor.White, "\t{0}", action);
                }
            }
            catch (Exception error)
            {
                WriteToConsole(ConsoleColor.Red, "...failed: " + error.Message);
            }
        }

        private static void RunInvokeCommand(ServerConnection connection, string fullUrn, IList<string> arguments)
        {
            WriteToConsole(ConsoleColor.Yellow, "Invoking '{1}' on '{0}'...", fullUrn, arguments[0]);
            try
            {
                var message = "<Blank xmlns=\"urn:cruisecontrol:common\" />";
                var result = connection.Invoke(fullUrn, arguments[0], message);
                WriteToConsole(ConsoleColor.Yellow, "...completed");
                WriteToConsole(ConsoleColor.Cyan, result);
            }
            catch (Exception error)
            {
                WriteToConsole(ConsoleColor.Red, "...failed: " + error.Message);
            }
        }
    }
}
