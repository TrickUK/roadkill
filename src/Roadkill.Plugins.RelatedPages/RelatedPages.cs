#region

using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Roadkill.Core.Database;
using Roadkill.Core.Mvc.ViewModels;
using Roadkill.Core.Plugins;

#endregion

namespace Roadkill.Plugins.RelatedPages
{
	public class RelatedPages : TextPlugin
	{
		private IRepository _repository;
		private static Regex _regex = new Regex(@"(?<!{)(?:\{Related\})(?!})", RegexOptions.Compiled);

		public override string Id
		{
			get { return "RelatedPages"; }
		}

		public override string Name
		{
			get { return "RelatedPages"; }
		}

		public override string Description
		{
			get { return "Inserts a list of links to pages with the same tag(s). Usage: {Related}"; }
		}

		public override string Version
		{
			get { return "1.0.0"; }
		}

		public RelatedPages(IRepository repository)
		{
			_repository = repository;
		}

		public override string AfterParse(string html)
		{
			if (!html.Contains("{Related}")) return html;
			RouteData routeData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(HttpContext.Current));
			if (routeData == null) return html;

			int pageId;
			Page currentPage = null;
			if (int.TryParse(routeData.Values["id"].ToString(), out pageId)) currentPage = _repository.GetPageById(pageId);
			if (currentPage == null) currentPage = _repository.FindPagesContainingTag("homepage").FirstOrDefault();
			if (currentPage == null) return html;

			UrlHelper helper = new UrlHelper(HttpContext.Current.Request.RequestContext);
			StringBuilder sb = new StringBuilder();
			
			sb.AppendLine("<ul id=\"related\" class=\"rounded10 clear\">");
			foreach (Page page in PageViewModel.ParseTags(currentPage.Tags).SelectMany(tag => _repository.FindPagesContainingTag(tag)).GroupBy(page => page.Id).Select(p => p.FirstOrDefault()).Where(p => p.Id != currentPage.Id)) {
				sb.AppendFormat("<li><a href=\"{0}\">{1}</a></li>", new object[] {
					helper.Action("Index", "Wiki", new {
						id = page.Id,
						title = PageViewModel.EncodePageTitle(page.Title)
					}),
					page.Title
				});
			}
			sb.AppendLine("</ul>");
			return _regex.Replace(html, sb.ToString());
		}
	}
}