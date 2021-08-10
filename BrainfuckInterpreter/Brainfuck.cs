using System;
using System.Text;

namespace BrainfuckInterpreter
{
    public class Brainfuck
    {
        public static string From(string ascii)
        {
            var sb = new StringBuilder();
            int current = 0;
            foreach (char c in ascii)
            {
                char sign = c > current ? '+' : '-';
                int diff = Math.Abs(c - current);
                switch (diff)
                {
                    case 0:
                        sb.Append('.');
                        break;
                    case < 10:
                        sb.Append(new string(sign, diff)).Append('.');
                        break;
                    default:
                    {
                        int closestSqn = (int) Math.Round(Math.Sqrt(diff));
                        int rest = (int) (diff - (Math.Pow(closestSqn, 2)));

                        sb
                            .Append('>')
                            .Append(new string('+', closestSqn))
                            .Append("[<")
                            .Append(new string(sign, closestSqn))
                            .Append(">-]<")
                            .Append(new string(rest > 0 ? sign : sign == '-' ? '+' : '-', Math.Abs(rest)))
                            .Append('.');
                        break;
                    }
                }

                current = c;
            }

            return sb.ToString();
        }

        public static bool ValidateBrainfuck(string bf, out string reason)
        {
            int depth = 0;
            int lastOpen = 0;
            int lastClose = 0;
            for (int i = 0; i < bf.Length; i++)
            {
                switch (bf[i])
                {
                    case '[':
                        lastOpen = i;
                        depth++;
                        break;
                    case ']':
                        lastClose = i;
                        depth--;
                        break;
                }

                if (depth < 0) break;
            }

            bool isValid = depth == 0;
            if (!isValid)
            {
                bool isUnclosed = depth > 0;
                int position = isUnclosed ? lastOpen : lastClose;
                char c = isUnclosed ? '[' : ']';
                int startIdx = Math.Max(0, position - 5);
                int endIdx = Math.Min(11, bf.Length - startIdx);
                reason = $"Unmatched {c} at position {position}: {bf.Substring(startIdx, endIdx)}";
            }
            else reason = null;

            return isValid;
        }

        private readonly string _bf;
        private readonly int[] _buffer;
        private int _ptr;
        private int _cursor;

        public Brainfuck(string bf)
        {
            if (!ValidateBrainfuck(bf, out var reason)) throw new BrainfuckException(reason);
            _bf = bf;
            _buffer = new int[30000];
            _buffer.Initialize();
        }

        private delegate void PutChar(char c);

        private void Evaluate(PutChar putChar)
        {
            for (_cursor = 0, _ptr = 0; _cursor < _bf.Length; _cursor++)
            {
                switch (_bf[_cursor])
                {
                    case '>':
                        if (_ptr == _buffer.Length - 1)
                            _ptr = 0;
                        else
                            _ptr++;
                        break;
                    case '<':
                        if (_ptr == 0)
                            _ptr = _buffer.Length - 1;
                        else
                            _ptr--;
                        break;
                    case '+':
                        _buffer[_ptr]++;
                        break;
                    case '-':
                        _buffer[_ptr]--;
                        break;
                    case '.':
                        putChar((char) _buffer[_ptr]);
                        break;
                    case ',':
                        _buffer[_ptr] = Console.Read();
                        break;
                    case '[':
                    {
                        if (_buffer[_ptr] == 0)
                        {
                            int depth = 1;
                            do
                            {
                                switch (_bf[++_cursor])
                                {
                                    case '[':
                                        depth++;
                                        break;
                                    case ']':
                                        depth--;
                                        break;
                                }
                            } while (depth != 0);
                        }

                        break;
                    }
                    case ']':
                    {
                        int depth = 0;
                        do
                        {
                            switch (_bf[_cursor--])
                            {
                                case '[':
                                    depth--;
                                    break;
                                case ']':
                                    depth++;
                                    break;
                            }
                        } while (depth != 0);

                        break;
                    }
                }
            }
        }

        public void Evaluate()
        {
            Console.OutputEncoding = Encoding.ASCII;
            Evaluate(Console.Write);
        }

        public void Evaluate(out string outBuffer)
        {
            var sb = new StringBuilder();
            Evaluate(c => sb.Append(c));
            outBuffer = sb.ToString();
        }
    }
}
