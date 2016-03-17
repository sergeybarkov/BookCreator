using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aspose.Words;
using Aspose.Words.Saving;
using System.Text.RegularExpressions;
using System.IO;

namespace BookCreator
{
    class Word2Html
    {
        public static string Convert(String input, String output)
        {           

            Document doc = new Document(input);

            var options = new HtmlSaveOptions();

            options.ImagesFolder = Path.GetDirectoryName(output) + "/img";
            options.ImagesFolderAlias = "img";

            Directory.CreateDirectory(options.ImagesFolder);

            doc.Save(output, options);

            String htmlDocument = File.ReadAllText(output);

            var m = Regex.Match(htmlDocument, "<body>(.+?)</body>");

            String result = m.Groups[1].Value;

            result = Regex.Replace(m.Groups[1].Value, @"font-family:(.+?);", String.Empty);
            result = Regex.Replace(result, @"font-size:[0-9]+pt;", String.Empty);
            result = Regex.Replace(result, @"font-size:[0-9]+pt""", @"""");

            result = Regex.Replace(result, @"margin:[0-9]+pt;", String.Empty);
            result = Regex.Replace(result, @"margin:[0-9]+pt""", @"""");

            result = Regex.Replace(result, @"text-indent:[0-9\.]+pt;", String.Empty);
            result = Regex.Replace(result, @"text-indent:[0-9\.]+pt""", @"""");

            result = Regex.Replace(result, @"<span style="" "">(.+?)</span>", "$1"); 


          //  var aaa = Regex.Match(result, @"<span((.+?)>(&#xa0;?)?)</span>");
          //  result = Regex.Replace(result, @"<p(.+?)>(&#xa0;?)</p>", String.Empty);


            var paragraphs = Regex.Match(result, @"<p(.+?)</p>");

            var p1 = paragraphs.Groups[0].Value;
            var p1_final = String.Format("<p class='cdml_ch'>{0}</p>", Regex.Replace(p1, "<.*?>", String.Empty));

            paragraphs = paragraphs.NextMatch();

            var p2 = paragraphs.Groups[0].Value;
            var p2_final = String.Format("<p class='cdml_pr'>{0}</p>", Regex.Replace(p2, "<.*?>", String.Empty));

            try
            {

                result = result.Replace(p1, p1_final);
                result = result.Replace(p2, p2_final);

            }
            catch
            {
                ;
            }

            result = Regex.Replace(result, @"<span([^>/][^>]*)></(.+?)>", String.Empty);
            
            //result = Regex.Replace(result, @"<([^>/][^>]*)> </(.+?)>", String.Empty);
            result = Regex.Replace(result, @"<([^>/][^>]*)>&#xa0;</(.+?)>", String.Empty);
            //result = Regex.Replace(result, @"<p([^>/][^>]*)>&#xa0;</p>", String.Empty);

            //result = Regex.Replace(result, @"[ ]{2,}", String.Empty); //убийство лишних пробелов
            //result = Regex.Replace(result, @"(&#xa0;){2,}", String.Empty); //убийство лишних пробелов


            //result = Regex.Replace(result, "@</span><span(.+?)>(.+)</span>", "$2" + "</span>");

            
            


            result = result.Replace("</p>", "</p>\n    ");
            result = result.Replace("</div>", "</div>\n    ");
            result = result.Replace("</ul>", "</ul>\n    ");           

            File.WriteAllText(output, result);

            return result;
        }
    }
}
