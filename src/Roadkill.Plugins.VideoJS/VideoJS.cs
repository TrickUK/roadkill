using System;
using System.Text;
using System.Web;
using System.Text.RegularExpressions;
using Roadkill.Core.Plugins;

namespace Roadkill.Plugins
{
    public class VideoJS : TextPlugin
    {
        public override string Id
	    {
			get { return "VideoJS"; }
	    }

	    public override string Name
	    {
			get { return "VideoJS"; }
	    }

	    public override string Description
	    {
			get { return "Enables videos to be played using an HTML5 or Flash based player."; }
	    }

	    public override string Version
	    {
		    get { return "1.0.0"; }
	    }

		public VideoJS()
		{
            AddScript("video.js");
            AddScript("video.setup.js");
		}

		public override string GetHeadContent()
		{
			return string.Format("{0}{1}", GetJavascriptHtml(), GetCssLink("video-js.min.css"));
		}

        public override string BeforeParse(string markupText)
        {
            // RegEx for replacement.
			// Because we are injecting the HTML on BeforeParse (to prevent conflicting with images),
			// the video and source tags need to be included in the HTML whitelist and the whitelist
			// should be enabled.
			// \{\{(?'src'.*\.(?'ext'mp4|webm|ogg))(?:\|(?'alt'.*))*\}\}
            
            string[] exts = Settings.GetValue("FileExtensions").Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            StringBuilder sb = new StringBuilder();
            sb.Append(@"\{\{(?'src'.*\.(?'ext'");
            foreach (string ext in exts) {
                sb.AppendFormat("{0}|", ext);
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append(@"))(?:\|(?'alt'.*))*\}\}");

            string RegexString = sb.ToString();
            string ReplacementPattern = string.Format("<video class=\"video-js vjs-default-skin vjs-big-play-centered\"><source src=\"{0}${{src}}\" type=\"video/${{ext}}\" /></video>", ApplicationSettings.AttachmentsUrlPath);

            return !Regex.IsMatch(markupText, RegexString) ? markupText : Regex.Replace(markupText, RegexString, ReplacementPattern);
        }

        public override string AfterParse(string html)
        {
            // This will clean up any HtmlEncoded characters in our video and source tags
			// that the sanitizer may have introduced.
			return HttpUtility.HtmlDecode(html);
        }

        public override void OnInitializeSettings(Settings settings)
	    {
			settings.SetValue("FileExtensions", "mp4,ogg,webm");
			base.OnInitializeSettings(settings);
	    }
    }
}
