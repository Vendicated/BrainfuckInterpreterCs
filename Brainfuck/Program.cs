using System;
using System.Linq;

namespace Brainfuck
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            string rest = string.Join(' ', args.Skip(1));
            switch (args[0])
            {
                case "encode" or "e":
                    Console.Write(BrainfuckInterpreter.Brainfuck.From(rest));
                    break;
                case "decode" or "d":
                    new BrainfuckInterpreter.Brainfuck(rest).Evaluate();
                    break;
                default:
                    string name = AppDomain.CurrentDomain.FriendlyName;
                    Console.WriteLine("Brainfuck Interpreter");
                    Console.WriteLine("Usage:");
                    Console.WriteLine($"\t{name} [e]ncode [ASCII]");
                    Console.WriteLine($"\t{name} [d]ecode [BRAINFUCK]");
                    break;
            }
        }
    }
}