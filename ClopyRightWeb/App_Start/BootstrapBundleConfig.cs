using System.Web.Optimization;

[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(ClopyRightWeb.App_Start.BootstrapBundleConfig), "RegisterBundles")]

namespace ClopyRightWeb.App_Start
{
	public class BootstrapBundleConfig
	{
		public static void RegisterBundles()
		{
			// When <compilation debug="true" />, MVC4 will render the full readable version. When set to <compilation debug="false" />, the minified version will be rendered automatically
			BundleTable.Bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include("~/Scripts/bootstrap*"));
			BundleTable.Bundles.Add(new StyleBundle("~/Content/bootstrap").Include("~/Content/bootstrap.css"));
		}
	}
}
