using System;
using System.Diagnostics;
using System.IO;

namespace LogGen
{
    class Program
    {
        private const string FileName = "log.txt";

        private static readonly string[] Types = {"A", "B", "C", "D"};
        private static readonly string[] Systems = {"CORE", "DAL", "UI", "TESTS"};
        private static readonly string[] Messages = {"NullReferenceException",
                                              "InvalidOperationException",
                                              "SqlException",
                                              "UserSighed",
                                              "UserLogoff",
                                              "OperationStarted",
                                              "OperationCanceled",
                                              "OperationComplete" };

        static void Main(string[] args)
        {
            var random = new Random(Environment.TickCount);
            using (var str = File.CreateText(FileName))
            {
                for (int i = 0; i < 100000; i++)
                {
                    str.WriteLine($"{DateTime.Now.AddSeconds(i).ToString("dd.MM.yy hh:mm:ss.mss").Substring(0,21)} {Types[random.Next(Types.Length)]} {Systems[random.Next(Systems.Length)]} {Messages[random.Next(Messages.Length)]}");
                }
                str.Flush();
                str.Close();
            }
        }

        //static void Main(string[] args)
        //{
        //    Stopwatch stopWatch = new Stopwatch();
        //    stopWatch.Start();
        //    using (var str = File.OpenText(FileName))
        //    {
        //        while (!str.EndOfStream)
        //        {
        //            string s = str.ReadLine();
        //        }
        //    }
        //    stopWatch.Stop();
        //    // Get the elapsed time as a TimeSpan value.
        //    TimeSpan ts = stopWatch.Elapsed;

        //    // Format and display the TimeSpan value.
        //    string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
        //        ts.Hours, ts.Minutes, ts.Seconds,
        //        ts.Milliseconds / 10);
        //    Console.WriteLine("RunTime " + elapsedTime);
        //    Console.ReadLine();
        //}
    }
}
