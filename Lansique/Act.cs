using System;
using System.Collections.Generic;
using HtmlAgilityPack;

namespace Thismaker.Lansica
{
    /// <summary>
    /// This class loads, by default, acts found within KenyaLaw.org, however, it has been
    /// modularised to be loadable from any source for easy management of law related stuff
    /// </summary>
    public class Act
    {
        public string Path { get; set; }
        public string ID { get; set; }
        public DateTime CommencmentDate { get; set; }
        public DateTime AssentDate { get; set; }
        public string Url { get; set; }
        public string ActTitle { get; set; }
        public string ActNumber { get; set; }
        public List<Section> Sections { get; set; } = new List<Section>();
        public Preamble Preamble { get; set; }
        public string Html { get; set; }
        
        /// <summary>
        /// The Act ID is the same as the id in KenyaLaw.Org
        /// </summary>
        /// <param name="ID"></param>
        public Act(string ID, string Path)
        {
            this.ID = ID;
            this.Path = Path;
            var treat = ID.Replace(" ", "%20");
            Url = $"http://www.kenyalaw.org:8181/exist/kenyalex/actiview.xql?actid={treat}";

            var web = new HtmlWeb();

            var doc = web.Load(Url);
            Html = doc.Text;

            Make();

        }

        private void Make()
        {
            var list = Retrieve(Html);

        }

        private List<object> Retrieve(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            List<object> result = new List<object>();

            var nodes = doc.DocumentNode.SelectNodes("//div");

            foreach (var node in nodes)
            {
                var val = node.Attributes["class"]?.Value;
                if (val == null) continue;

                if (val == "content")
                {
                    var section = new Section();

                    var secHTxt = node.InnerHtml;

                    var secDoc = new HtmlDocument();
                    secDoc.LoadHtml(secHTxt);

                    var subSecs = new List<object>();
                    var secNodes = secDoc.DocumentNode.SelectNodes("//d");
                    foreach (var secNode in secNodes)
                    {
                        if (secNode.Attributes["class"]?.Value == "subsection")
                        {
                            var secText = secNode.InnerHtml;
                        }
                    }

                    var h = Retrieve(node.InnerHtml);
                }
            }

            return result;
        }
    }
}
