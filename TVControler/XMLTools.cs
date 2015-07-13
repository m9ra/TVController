using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

namespace TVControler
{
    static class XMLTools
    {
        public static void CreateFromTemplate(string templatPath, string outputPath, IDictionary<string, string> pars)
        {
            string data;
            using (var reader = new StreamReader(templatPath))
            {
                data = reader.ReadToEnd();
                reader.Close();
            }

            foreach (var par in pars)
            {
                data = data.Replace("{" + par.Key + "}", par.Value);
            }

            using (var writer = new StreamWriter(outputPath))
            {
                writer.Write(data);
                writer.Close();
            }
        }
    }
}
