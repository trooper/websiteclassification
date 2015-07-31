using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PsiMl.WebsiteClasification
{
    public class Blacklist
    {
        private string path;
        private HashSet<string> list;

        public Blacklist(string path)
        {
            this.path = path;
            foreach(var line in File.ReadAllLines(path))
            {
                list.Add(line);
            }
        }

        public bool Contains(Feature feature)
        {
            return this.list.Contains(feature.Name);
        }

        public bool Contains(string featureName)
        {
            return this.list.Contains(featureName);
        }

        public void Add(Feature feature)
        {
            list.Add(feature.Name);
        }

        public void Add(string featureName)
        {
            list.Add(featureName);
        }

        public void Save()
        {
            File.WriteAllLines(this.path, this.list);
        }

        /*private Dictionary<Target, List<string>> list;

        public Blacklist()
        {
            list = new Dictionary<Target,List<string>>();
            foreach(Target t in Enum.GetValues(typeof(Target)))
            {
                list[t] = new List<string>();
            }
        }

        public void LoadFromDirectory(string path)
        {
            foreach(string file in Directory.EnumerateFiles(path))
            {
                
            }
        }

        public void Add(Target target, Feature feature)
        {
            
        }

        public void Contains(Feature feature)
        {

        }

        public void Contains(Feature feature, Target target)
        {

        }*/
    }
}
