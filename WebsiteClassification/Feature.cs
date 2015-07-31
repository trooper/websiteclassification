namespace PsiMl.WebsiteClasification
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Feature
    {
        public class Type
        {
            public const string RawText = "r";
            public const string MetaTag = "m";
        }

        public string Name { get; set; }

        public double Value { get; set; }
    }
}
