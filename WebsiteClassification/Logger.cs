namespace PsiMl.WebsiteClasification
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Logger
    {
        public static DateTime Start = DateTime.Now;
        public static void Log(string message)
        {
            Console.WriteLine("{0,12:0.000} {1}", (DateTime.Now - Start).TotalSeconds,  message);
        }

        public static void Log(string text, params object[] args)
        {
            Log(string.Format(text, args));
        }

        public static void Initialize()
        {
            Log("Logger initialized");
        }
    }
}
