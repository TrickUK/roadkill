#region

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Roadkill.Core.Database;
using Roadkill.Core.Mvc.ViewModels;
using Roadkill.Core.Plugins;

#endregion

namespace Roadkill.Plugins.Top10
{
	public class Top10 : TextPlugin
	{
		private IRepository _repository;
		private static Regex _regex = new Regex(@"(?<!{)(?:\{Top10\})(?!})", RegexOptions.Compiled);

		public override string Id
		{
			get { return "Top10"; }
		}

		public override string Name
		{
			get { return "Top Ten Pages"; }
		}

		public override string Description
		{
			get { return "Displays the top ten new/modified pages."; }
		}

		public override string Version
		{
			get { return "1.0.0"; }
		}

		public Top10(IRepository repository)
		{
			_repository = repository;
		}

		public override string AfterParse(string html)
		{
			if (!html.Contains("{Top10}")) return html;

			List<Page> topPages = _repository.AllPages().OrderByDescending(p => p.ModifiedOn).Take(10).ToList();
			if (topPages.Count == 0) return _regex.Replace(html, string.Empty);

			UrlHelper helper = new UrlHelper(HttpContext.Current.Request.RequestContext);
			StringBuilder sb = new StringBuilder();

			sb.AppendLine("<ol class=\"clear\">");
			foreach (Page page in topPages) {
				sb.AppendFormat("<li><a href=\"{0}\">{1}</a> <span class=\"smaller\">{2} {3}</span></li>", new object[] {
					helper.Action("Index", "Wiki", new {
						id = page.Id,
						title = PageViewModel.EncodePageTitle(page.Title)
					}),
					page.Title, page.ModifiedOn.ToLongDateString(), page.ModifiedOn.ToShortTimeString()
				});
			}
			sb.AppendLine("</ol>");
			return _regex.Replace(html, sb.ToString());
		}
	}
}