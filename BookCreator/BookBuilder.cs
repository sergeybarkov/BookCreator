using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookCreator;
using System.Data.Entity;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Text.RegularExpressions;

namespace BookCreator
{
    public class BookBuilder
    {

        Dictionary<string, string> content = new Dictionary<string, string>();

        private book_creatorEntities context;
        private string outputdir;
        private string inputdir;
        private bool wordmode;

        public BookBuilder(string outputdir, string inputdir)
        {
            this.context = new book_creatorEntities();
            this.outputdir = outputdir;
            this.inputdir = inputdir;
            this.wordmode = !String.IsNullOrEmpty(inputdir);
        }

        private void prepareDir()
        {
            Helper.DirectoryCopy("Content/base", outputdir, true);
        }

        private Dictionary<string,string> initGlossary(bool wordmode)
        {
            var index = new StringBuilder();

            index.Append("<p class='cdml_ch'>Глоссарий</p>");

            var lastLetter = new char();

            var result = new Dictionary<string, string>();
            
            var morpher = new ru_morpher_api.WebService();

            List<bc_gloss> glossary = new List<bc_gloss>();

            if (!wordmode)
            {
                glossary = (from c in context.bc_gloss select c).ToList();
            }
            else
            {
                var glosstxt = File.ReadAllText(inputdir + "/gloss.txt");

                var glossarray = glosstxt.Split('@');

                foreach (var g in glossarray)
                {
                    if (!String.IsNullOrEmpty(g))
                    {
                        bc_gloss gl = new bc_gloss();
                        gl.text = g.Trim();
                        gl.text = gl.text.Replace("\n", "<br />");

                        var tmptmptmp = g.Split(':');
                        //var tmptmptmp = g.Split('-');                        

                        gl.name = tmptmptmp[0].Trim();

                        var newname = char.ToUpper(gl.name[0]) + gl.name.Substring(1);

                        gl.text = gl.text.Replace(gl.name, "<b>" + newname + "</b>");
                        gl.name = newname;

                        glossary.Add(gl);
                    }
                    
                }

                glossary = glossary.OrderBy(c => c.name).ToList();


            }


            var glossDir = String.Format("{0}/gloss" ,outputdir);

            Directory.CreateDirectory(glossDir);

            XmlSerializer serializer = new XmlSerializer(typeof(ru_morpher_api.GetXmlResult));

            var fileCount = 0;

            foreach (var c in glossary)
            {

                fileCount++;

                ru_morpher_api.GetXmlResult xmlresult = new ru_morpher_api.GetXmlResult();

                var cachefile = String.Format("morpher_cache/{0}.xml", c.name);

                if (File.Exists(cachefile))
                {
                    using (Stream stream = new FileStream(cachefile, FileMode.Open, FileAccess.Read))
                    {
                        xmlresult = (ru_morpher_api.GetXmlResult)serializer.Deserialize(stream);
                    }
                }
                else
                {
                    xmlresult = morpher.GetXml(c.name);
                    
                    using (Stream stream = new FileStream(cachefile, FileMode.Create, FileAccess.Write))
                    {
                        serializer.Serialize(stream, xmlresult);
                    }

             
                }

                //var filename = String.Format("{0}.html", Transliteration.Transliteration.Front(c.name));
                var filename = String.Format("{0}.html", fileCount);
                var filepath = String.Format("{0}/{1}", glossDir, filename);


                if (c.name[0] != lastLetter)
                {
                    index.Append(String.Format("<p>&nbsp;</p><p style='text-transform: upper-case;'><b>{0}</b></p>", c.name[0]));
                    lastLetter = c.name[0];
                }

                index.Append(String.Format("<p><a href='{0}'>{1}</a></p>", filename, c.name));

                File.WriteAllText(filepath, c.text);


                try { result.Add(xmlresult.множественное.И, filename); }
                catch { }
                try { result.Add(xmlresult.множественное.Р, filename); }
                catch { }
                try { result.Add(xmlresult.множественное.Д, filename); }
                catch { }
                try { result.Add(xmlresult.множественное.В, filename); }
                catch { }
                try { result.Add(xmlresult.множественное.Т, filename); }
                catch { }
                try { result.Add(xmlresult.множественное.П, filename); }
                catch { }
                try { result.Add(xmlresult.Р, filename); }
                catch { }
                try { result.Add(xmlresult.Д, filename); }
                catch { }
                try { result.Add(xmlresult.В, filename); }
                catch { }
                try { result.Add(xmlresult.Т, filename); }
                catch { }
                try { result.Add(xmlresult.П, filename); }
                catch { }
                try { result.Add(c.name, filename); }
                catch { }

            }

            var tmp = File.ReadAllText("Content/page_simple.html");
            tmp = tmp.Replace("{content}", index.ToString());

            File.WriteAllText(outputdir + "/gloss/index.html", tmp);

            return result;

        }

        public void WriteChapters()
        {
            Console.WriteLine();
            Console.WriteLine("Начал работу");

            Dictionary<string, string> glossary = initGlossary(wordmode);

            prepareDir();

            var glossaryLinkTemplate = "<a href='../gloss/{0}' class='fancybox'>{1}</a>{2}";

            var path = String.Format("{0}/content", outputdir);

            List<bc_chapters> items = new List<bc_chapters>();
            if (!wordmode)
            {
                items = (from c in context.bc_chapters orderby c.name select c).ToList();
            }
            else
            {
                var files = Directory.GetFiles(inputdir, "*.docx");

                foreach (var file in files)
                {
                    var name = Path.GetFileNameWithoutExtension(file);
                    var htmlfile = String.Format("{0}/{1}.html", path, name);

                    var ch = new bc_chapters();
                    ch.name = name;
                    ch.text = Word2Html.Convert(file, htmlfile);

                    items.Add(ch);
 
                }
            }

            var pageTemplate = File.ReadAllText("Content/page.html");

            //Directory.Delete(path, true);

            Directory.CreateDirectory(path);

            var lastChapterName = new List<string>();

            var i = 0;
            foreach (var c in items)
            {
                var filepath = String.Format("{0}/{1}.html", path, c.name);

                var lastPage = i == 0 ? "content" : items[i - 1].name;
                var nextPage = i == items.Count-1 ? "title" : items[i + 1].name;

                var builder = new StringBuilder(pageTemplate);
                builder.Replace("{content}", c.text);
                builder.Replace("{last_page}", lastPage);
                builder.Replace("{next_page}", nextPage);
                builder.Replace("<p>&nbsp;</p>", "");

                foreach (var g in glossary)
                {
                    if (!String.IsNullOrEmpty(g.Key))
                    {
                        builder.Replace(g.Key + "<", String.Format(glossaryLinkTemplate, g.Value, g.Key, "<"));
                        builder.Replace(g.Key + " ", String.Format(glossaryLinkTemplate, g.Value, g.Key, " "));
                        builder.Replace(">" + g.Key + "<", ">" + String.Format(glossaryLinkTemplate, g.Value, g.Key, "<"));
                        builder.Replace(g.Key + ".", String.Format(glossaryLinkTemplate, g.Value, g.Key, "."));
                        builder.Replace(g.Key + ",", String.Format(glossaryLinkTemplate, g.Value, g.Key, ","));
                        builder.Replace(g.Key + ";", String.Format(glossaryLinkTemplate, g.Value, g.Key, ";"));
                        builder.Replace(g.Key + ")", String.Format(glossaryLinkTemplate, g.Value, g.Key, ")"));

                        builder.Replace(">" + g.Key.ToLower() + "<", ">" + String.Format(glossaryLinkTemplate, g.Value, g.Key.ToLower(), "<"));

                    }
                }

                File.WriteAllText(filepath, builder.ToString());

                //Поиск название главы --------------------------------
                var pureText = Regex.Replace(c.text, "<.*?>", string.Empty);
                var lines = pureText.Split('\n');
                foreach (var l in lines)
                {
                    if (!lastChapterName.Contains(l.Trim()) && !String.IsNullOrWhiteSpace(l))
                    {
                        content.Add(c.name, l.Trim());
                        lastChapterName.Add(l.Trim());                        
                        break;
                    }
                }
                //------------------------------------------------------

                


                i++;
            }

            buildIndex();

        }

        private void buildIndex()
        {
            var pageTemplate = File.ReadAllText("Content/page.html");
            var page = new StringBuilder(pageTemplate);

            var index = new StringBuilder();

            var jsonindex = new StringBuilder();

            index.Append("<div style='text-align:center; font-weight: bold;'>СОДЕРЖАНИЕ</div>");

            foreach (var c in content)
            {
                var shortname = (from str in c.Value.Split('.') select str).FirstOrDefault();

                if (c.Key.EndsWith("00"))
                {
                    index.Append(String.Format("<p class='cdml_pr'><a href='{0}.html'>{1}</a></p>", c.Key, c.Value));

                    jsonindex.Append(String.Format("],[null,'{1}','content/{0}.html',null, '{2}' ", c.Key, shortname, c.Value));
                }
                else
                {
                    index.Append(String.Format("<p><a href='{0}.html'>{1}</a></p>", c.Key, c.Value));

                    jsonindex.Append(String.Format(", [null,'{1}','content/{0}.html',null,'{2}'],", c.Key, shortname, c.Value));
                }
            }

            jsonindex.Replace(",]", "]");
            jsonindex.Replace(",,", ",");

            char[] json = new char[jsonindex.Length-3];
            jsonindex.CopyTo(2, json, 0, json.Length);

            page.Replace("{content}", index.ToString());
            page.Replace("{last_link}", "title");
            page.Replace("{next_link}", "00_00");

            File.WriteAllText(outputdir + "/content/content.html", page.ToString());

            var tabs = File.ReadAllText(outputdir + "/tabs.html");
            tabs = tabs.Replace("{jsonindex}", new string(json));

            File.WriteAllText(outputdir + "/tabs.html", tabs);

            
        }
        
        


    }
}
