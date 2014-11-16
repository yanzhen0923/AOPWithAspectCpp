using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DesignPattern
{
    class FuncNameAndApprTm
    {
        public string FunctionName { get; set; }
        public string AppearTimes { get; set; }

        public FuncNameAndApprTm(string name, string value)
        {
            FunctionName = name;
            AppearTimes = value;
        }
    }

    class StrongRely
    {
        public string TargFunc { get; set; }
        public string DepFunc { get; set; }
        public string Times { get; set; }
        public string Perc { get; set; }
        public StrongRely(string tar, string dep, string tim, string per)
        {
            TargFunc = tar;
            DepFunc = dep;
            Times = tim;
            Perc = per;
        }
    }
}
