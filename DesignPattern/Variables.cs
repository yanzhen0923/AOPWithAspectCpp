namespace DesignPattern
{
    class Variables
    {
        public static string cutTheTree = "cutTheTree.exe";
        public static string res = "res.xml";
        public static string funcions = "functions.tmp";
        public static string nouse = "nouse.tmp";
        public static string aims = "aims.tmp";
        public static string coverage = "coverage.ah";
        public static string mycoverage = "mycoverage.ah";
        public static string clear = "clear.bat";
        public static string appeartimes = "appeartimes.tmp";
        public static string invokedconditon = "invokecondition.tmp";
        public static string beinvokedconditon = "beinvokedcondition.tmp";
        public static string strongre = "strongre.tmp";

        public static string ifndef(string filename)
        {
            return "#ifndef __" + filename + "_ah__\r\n"
                + "#define __" + filename + "_ah__\r\n\r\n";
        }

        public static string start1 = "#include <iostream>\r\nusing namespace std;\r\naspect ";
        public static string aspectName = "default";
        public static string start2 = "{\r\n    pointcut mons() = ";

        public static string tab = "    ";
        public static string adviceExecution = "\r\n    advice execution (";
        public static string mons = "mons()";
        public static string counterName = "counter";
        public static string counterDeclaraion = "    int " + counterName + ";\r\n    ";
        public static string before = ") : before() {\r\n    ";
        public static string after = ") : after() {\r\n    ";
        public static string around = ") : around() {\r\n    ";
        public static string tjp = "tjp->proceed();\r\n";
        public static string tryCode = "    try{\r\n        ";
        public static string catchCode = "    }\r\n    catch(...){\r\n        throw;\r\n    }\r\n";
        public static string bodyEnd = tab + "}";
        public static string codeEnd = "\r\n};\r\n\r\n";
        public static string endif = "#endif";

    }
}
