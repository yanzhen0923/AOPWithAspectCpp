using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace DesignPattern
{
    class Command
    {
        public static void Execute(string executor,string args)
        {
            Process process = new Process();
            process.StartInfo.FileName = executor;
            process.StartInfo.Arguments = "/C " + args;
            process.Start();
            process.WaitForExit();
        }

        public static void Execute(string executor)
        {
            Process process = new Process();
            process.StartInfo.FileName = executor;
            process.Start();
            process.WaitForExit();
        }
    }
}
