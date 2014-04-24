using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Roadkill.Core.Database;
using Roadkill.Core.Mvc.ViewModels;
using Roadkill.Core.Plugins;
using Roadkill.Core.Services;
using System.Text.RegularExpressions;

namespace Roadkill.Plugins.TagCloud
{
    public class TagCloud : TextPlugin
    {
        private PageService _pageService;
        private static Regex _regex = new Regex(@"(?<!{)(?:\{TagCloud\})(?!})", RegexOptions.Compiled);

        public override string Id
        {
            get { return "TagCloud"; }
        }

        public override string Name
        {
            get { return "TagCloud"; }
        }

        public override string Description
        {
            get { return "Enables a tag cloud based on the added categories. Usage: {TagCloud}"; }
        }

        public override string Version
        {
            get { return "1.0.0"; }
        }

        public TagCloud(PageService pageService)
        {
            _pageService = pageService;
        }

        public override string AfterParse(string html)
        {
            if (!html.Contains("{TagCloud}")) return html;

            // strip the {TagCloud} creole from the output
            html = _regex.Replace(html, "");
            
            // insert TagCloud html in place
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<ul id=\"tagcloud\" class=\"rounded10 clear\">");
            foreach (TagViewModel tag in _pageService.AllTags()) {
                sb.AppendFormat("<li class=\"{0}\"><a href=\"/pages/tag/{1}\">{1}</a></li>", tag.ClassName, tag.Name);
            }
            sb.AppendLine("</ul>");
            return string.Format("{0}{1}", html, sb);
        }
    }
}
