using System.Web;
using System.Web.Optimization;

namespace Insignia.Painel
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            BundleTable.EnableOptimizations = false;

            bundles.Add(new StyleBundle("~/Login/styles").Include(
                      "~/Content/css/custom.css",
                      "~/Content/css/bootstrap/bootstrap.css",
                      "~/Content/css/animate/animate.css",
                      "~/Content/css/font-aewsome/font-aewsome.css",
                      "~/Content/css/font-aewsome/nprogress.css"));
        }
    }
}
