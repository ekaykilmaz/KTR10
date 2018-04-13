using System;
using System.Linq;
using System.Diagnostics;
using System.IO;

namespace KTR10_CIM
{
    public class Utility
    {
        public const string TraceDebugStr = @"""{0}"" - Write Debug Problem - ex detail :  {1}";

        #region Debug && Log
        public static void DebugMe(string _log, string level)
        {
            DebugMeSub(_log, level);
        }

        public static void DebugMe(string _log)
        {
            DebugMeSub(_log, "DEBUG");
        }

        private static void DebugMeSub(string _log, string level)
        {
            String path = "C:\\WEBATMLOGS\\IscepMatik\\" + "KTR10_" + DateTime.Now.ToString("yyMMdd") + ".log";

            DateTime now = DateTime.Now;
            String header = "";
            header += now.ToString("yyMMdd-HHmmss.fff");
            header += " ";
            header += level;
            header += " ";

            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine(header + _log);
                writer.Close();
            }
        }

        #endregion

        #region O B E B  | |  Greatest Common Divisor
        public static int GCD(int[] numbers)
        {
            return numbers.Aggregate(GCD);
        }

        public static int GCD(int a, int b)
        {
            return b == 0 ? a : GCD(b, a % b);
        }

        #endregion
        
    }

}
