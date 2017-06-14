using System.Web.Optimization;

namespace Insignia.Site.App_Start
{
    public class BundleConfig
    {
        /// <summary>
        /// Bundle com arquivos usados nos cshtml
        /// </summary>
        /// <param name="bundles">Objeto para criar os bundles</param>
        public static void RegisterBundles(BundleCollection bundles)
        {
            BundleTable.EnableOptimizations = false;

            //Arquivos de css usados no index
            bundles.Add(new StyleBundle("~/Main/styles").Include(
                    "~/Content/css/main.css"));

            //Arquivos de javascript usados no index
            bundles.Add(new ScriptBundle("~/Main/scripts").Include(
                    "~/Scripts/js/jquery.min.js",
                    "~/Scripts/js/jquery.dropotron.min.js",
                    "~/Scripts/js/skel.min.js",
                    "~/Scripts/js/skel-viewport.min.js",
                    "~/Scripts/js/util.js",
                    "~/Scripts/js/main.js"));
        }
    }
}