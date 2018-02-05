using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace EasyDownloader.Utilities
{
    public class HrefFinder
    {
        public Dictionary<string, string> FindHrefs(string page)
        {
            Dictionary<string, string> listOfStrings = new Dictionary<string, string>();

            Regex regex = new Regex("href\\s*=\\s*(?:\"(?<1>[^\"]*)\"|(?<1>\\S+))", RegexOptions.IgnoreCase);
            Match match;

            int i = 1;
            for (match = regex.Match(page); match.Success; match = match.NextMatch())
            {
                foreach (Group group in match.Groups)
                {
                    if (group.Value.Contains("watch?") && !group.Value.Contains("href")
                         && !listOfStrings.ContainsKey(group.Value) && !group.Value.Contains("list"))
                    {
                        listOfStrings.Add(group.Value, i.ToString());
                        i++;
                    }
                }
            }

            return listOfStrings;
        }
    }
}
