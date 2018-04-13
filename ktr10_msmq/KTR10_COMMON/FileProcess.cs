
using System;
using System.IO;
using System.Text;
namespace KTR10_COMMON
{
    public static class FileProcess
    {
        private static string filepath = "C:\\WEBATMLOGS\\IscepMatik\\" + "KTR_10.Analysis.txt";
        private static object locker = new Object();

        public static void WriteToFile(string text)
        {
            lock (locker)
            {
                using (FileStream file = new FileStream(filepath, FileMode.Append, FileAccess.Write, FileShare.Read))
                using (StreamWriter writer = new StreamWriter(file, Encoding.Unicode))
                {
                    writer.WriteLine(text.ToString());
                }
            }

        }
    }
}
