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
                    "~/Content/css/bootstrap/bootstrap.css",
                    "~/Content/css/custom.css",
                    "~/Fonts/css/font-awesome.css",
                    "~/Content/css/nprogress/nprogress.css",
                    "~/Content/css/logo/logo.css",
                    "~/Content/css/animate/animate.css"));

            bundles.Add(new ScriptBundle("~/Login/scripts").Include(
                    "~/Scripts/js/bootstrap/bootstrap.js",
                    "~/Scripts/js/custom.js",
                    "~/Scripts/js/nprogress/nprogress.js",
                    "~/Scripts/js/logo/logo.js",
                    "~/Scripts/js/inputmask/jquery.inputmask.bundle.min.js"));

            bundles.Add(new StyleBundle("~/MainLayout/styles").Include(
                    "~/Content/css/bootstrap/bootstrap.css",
                    "~/Content/css/custom.css",
                    "~/Fonts/css/font-awesome.css",
                    "~/Content/css/nprogress/nprogress.css",
                    "~/Content/css/animate/animate.css"));

            bundles.Add(new ScriptBundle("~/MainLayout/scripts").Include(
                    "~/Scripts/js/bootstrap/bootstrap.js",
                    "~/Scripts/js/custom.js",
                    "~/Scripts/js/nprogress/nprogress.js",
                    "~/Scripts/js/fastclick/fastclick.js"));


            bundles.Add(new ScriptBundle("~/Badges/styles").Include(
                    "~/Content/css/badges/badge.css",
                    "~/Content/css/colorpicker/bootstrap-colorpicker.css",
                    "~/Content/css/tags/jquery.tagsinput.css"));

            bundles.Add(new ScriptBundle("~/Badges/scripts").Include(
                    "~/Scripts/js/badges/badge.min.js",
                    "~/Scripts/js/colorpicker/bootstrap-colorpicker.js",
                    "~/Scripts/js/inputmask/jquery.inputmask.bundle.min.js",
                    "~/Scripts/js/tags/jquery.tagsinput.js"));

            bundles.Add(new ScriptBundle("~/Tarefas/styles").Include(
                    "~/Content/css/google-code-prettify/prettify.css"));

            bundles.Add(new ScriptBundle("~/Tarefas/scripts").Include(
                    "~/Scripts/js/google-code-prettify/prettify.js",
                    "~/Scripts/js/hotkeys/jquery.hotkeys.js",
                    "~/Scripts/js/wysiwyg/bootstrap-wysiwyg.min.js",
                    "~/Scripts/js/moment/moment.min.js",
                    "~/Scripts/js/daterangepicker/daterangepicker.js",
                    "~/Scripts/js/inputmask/jquery.inputmask.bundle.min.js"));
        }
    }
}

