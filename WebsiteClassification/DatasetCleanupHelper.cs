using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsiMl.WebsiteClasification
{
    public class DatasetCleanupHelper
    {
        public static void CleanupDirectory(string directoryPath, string cleanDirectoryPath)
        {
            Logger.Log("Cleaning whole directory {0}", directoryPath);

            if (!System.IO.Directory.Exists(cleanDirectoryPath))
                System.IO.Directory.CreateDirectory(cleanDirectoryPath);
            Logger.Log("Path for the clean directory is {0}", cleanDirectoryPath);

            Stack<string> directories = new Stack<string>();
            directories.Push(directoryPath);
            var directoryName = new System.IO.DirectoryInfo(directoryPath).Name;            
            
            while (directories.Count > 0)
            {
                var dir = directories.Pop();
                var subdirs = System.IO.Directory.EnumerateDirectories(dir);
                foreach(var subdir in subdirs)
                {
                    Logger.Log("Subdirectory {0}", subdir);
                    directories.Push(subdir);
                }
                
                var files = System.IO.Directory.EnumerateFiles(dir).ToList<string>();
                // Poor coding
                if (files.Count == 0)
                {
                    Logger.Log("No files in here");
                    continue;
                }

                var path = files[0].Substring(files[0].IndexOf(directoryName) + directoryName.Length);

                Logger.Log("Cleaning directory {0}", dir);
                Logger.Log("Path for files from this directory is {0}", path);

                var cleanSubdir = new System.IO.FileInfo(cleanDirectoryPath + path).DirectoryName;
                if (!System.IO.Directory.Exists(cleanSubdir))
                    System.IO.Directory.CreateDirectory(cleanSubdir);

                foreach(var file in files)
                {
                    
                    CleanupFile(file, cleanDirectoryPath + path);
                }
            }
        }

        public static void CleanupFile(string datasetPath, string cleanDatasetPath)
        {
            var uniqueLines = new Dictionary<string, List<string>>();

            using (var writer = new System.IO.StreamWriter(cleanDatasetPath))
            {
                foreach(var line in System.IO.File.ReadLines(datasetPath))
                {
                    bool isUnique = true;

                    var lineData = line.Split('\t');
                    var url = lineData[0];

                    var baseUrl = ExtractBaseURL(url);
                    var getParams = ExtractGetParams(url);

                    // If there's already a similar url (based on base url)
                    if (uniqueLines.ContainsKey(baseUrl))
                    {
                        // Make decision based on the number of same HTTP GET parameters
                        var commonParams = uniqueLines[baseUrl].Intersect<string>(getParams);

                        if (commonParams.Count<string>() != uniqueLines[baseUrl].Count)
                        {
                            isUnique = true;
                        }
                        else
                        {
                            isUnique = false;
                        }
                    }

                    if (isUnique)
                    {
                        uniqueLines[url] = new List<string>();
                        writer.WriteLine(line);
                    }
                }
            }
        }

        private static string ExtractBaseURL(string url)
        {
            int questionMark = url.IndexOf('?');
            if (questionMark == -1)
            {
                return url;
            }
            return url.Substring(0, questionMark);
        }

        private static List<string> ExtractGetParams(string url)
        {
            var paramsList = new List<string>();
            int questionMark = url.IndexOf('?');
            var data = url.Substring(questionMark + 1);
           
            string[] keyValues = data.Split('&');
            
            for(int i = 0; i < keyValues.Length; i++)
            {
                paramsList.Add(keyValues[i].Split('=')[0]);
            }
            return paramsList;
        }
    }
}
