namespace PsiMl.WebsiteClasification
{
    public class WebPage
    {
        public string Url { get; set; }

        public string Content { get; set; }

        public override string ToString()
        {
            return this.Url;
        }
    }
}
