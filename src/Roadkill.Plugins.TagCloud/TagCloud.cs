using System.Text;
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
            get { return "Inserts the Wiki tag cloud. Usage: {TagCloud}"; }
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

	        StringBuilder sb = new StringBuilder();
            sb.AppendLine("<ul id=\"tagcloud\" class=\"rounded10 clear\">");
            foreach (TagViewModel tag in _pageService.AllTags()) {
                sb.AppendFormat("<li class=\"{0}\"><a href=\"/pages/tag/{1}\">{1}</a></li>", tag.ClassName, tag.Name);
            }
            sb.AppendLine("</ul>");
			return _regex.Replace(html, sb.ToString());
        }
    }
}
