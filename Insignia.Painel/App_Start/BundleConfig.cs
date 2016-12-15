using System.Web.Optimization;

namespace Insignia.Painel
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

            //Arquivos de css usados no login
            bundles.Add(new StyleBundle("~/Login/styles").Include(
                    "~/Content/css/bootstrap/bootstrap.css",
                    "~/Content/css/custom.css",
                    "~/Fonts/css/font-awesome.css",
                    "~/Content/css/nprogress/nprogress.css",
                    "~/Content/css/logo/logo.css",
                    "~/Content/css/animate/animate.css"));

            //Arquivos de javascript usados no login
            bundles.Add(new ScriptBundle("~/Login/scripts").Include(
                    "~/Scripts/js/bootstrap/bootstrap.js",
                    "~/Scripts/js/custom.js",
                    "~/Scripts/js/nprogress/nprogress.js",
                    "~/Scripts/js/logo/logo.js",
                    "~/Scripts/js/inputmask/jquery.inputmask.bundle.min.js"));

            //Arquivos de css usados no layout principal
            bundles.Add(new StyleBundle("~/MainLayout/styles").Include(
                    "~/Content/css/bootstrap/bootstrap.css",
                    "~/Content/css/custom.css",
                    "~/Fonts/css/font-awesome.css",
                    "~/Content/css/nprogress/nprogress.css",
                    "~/Content/css/animate/animate.css"));

            //Arquivos de javascript usados no layout principal
            bundles.Add(new ScriptBundle("~/MainLayout/scripts").Include(
                    "~/Scripts/js/bootstrap/bootstrap.js",
                    "~/Scripts/js/custom.js",
                    "~/Scripts/js/nprogress/nprogress.js",
                    "~/Scripts/js/fastclick/fastclick.js"));

            //Arquivos de css usados nas badges
            bundles.Add(new ScriptBundle("~/Badges/styles").Include(
                    "~/Content/css/badges/badge.css",
                    "~/Content/css/colorpicker/bootstrap-colorpicker.css",
                    "~/Content/css/tags/jquery.tagsinput.css"));

            //Arquivos de javascript usados nas badges
            bundles.Add(new ScriptBundle("~/Badges/scripts").Include(
                    "~/Scripts/js/badges/badge.min.js",
                    "~/Scripts/js/colorpicker/bootstrap-colorpicker.js",
                    "~/Scripts/js/inputmask/jquery.inputmask.bundle.min.js",
                    "~/Scripts/js/tags/jquery.tagsinput.js"));

            //Arquivos de css usados nas tarefas
            bundles.Add(new ScriptBundle("~/Tarefas/styles").Include(
                    "~/Content/css/google-code-prettify/prettify.css"));

            //Arquivos de javascript usados nas tarefas
            bundles.Add(new ScriptBundle("~/Tarefas/scripts").Include(
                    "~/Scripts/js/Upload/bootstrap-filestyle.js",
                    "~/Scripts/js/google-code-prettify/prettify.js",
                    "~/Scripts/js/hotkeys/jquery.hotkeys.js",
                    "~/Scripts/js/wysiwyg/bootstrap-wysiwyg.min.js",
                    "~/Scripts/js/moment/moment.min.js",
                    "~/Scripts/js/daterangepicker/daterangepicker.js",
                    "~/Scripts/js/inputmask/jquery.inputmask.bundle.min.js"));
        }
    }
}

