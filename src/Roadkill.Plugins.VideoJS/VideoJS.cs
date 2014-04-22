using Roadkill.Core.Plugins;

namespace Roadkill.Plugins
{
    public class VideoJS : Core.Plugins.TextPlugin
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
			get { return "Enables videos to be played using a HTML5 or Flash based player."; }
	    }

	    public override string Version
	    {
		    get { return "1.0.0"; }
	    }

		public VideoJS()
		{
			AddScript("video.js");
		}

		public override string GetHeadContent()
		{
			return string.Format("{0}{1}", GetJavascriptHtml(), GetCssLink("video-js.min.css"));
		}

		public override string AfterParse(string html)
		{
			return html;
		}

	    public override void OnInitializeSettings(Settings settings)
	    {
			settings.SetValue("File Extentions", "mp4,ogg,webm");
			base.OnInitializeSettings(settings);
	    }
    }
}
