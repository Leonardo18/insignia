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
            bundles.Add(new StyleBundle("~/Badges/styles").Include(
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
            bundles.Add(new StyleBundle("~/Tarefas/styles").Include(
                    "~/Content/css/google-code-prettify/prettify.css",
                    "~/Content/css/select2/select2.css",
                    "~/Content/css/loadmore/loadmore.css"));

            //Arquivos de javascript usados nas tarefas
            bundles.Add(new ScriptBundle("~/Tarefas/scripts").Include(
                    "~/Scripts/js/Upload/bootstrap-filestyle.js",
                    "~/Scripts/js/google-code-prettify/prettify.js",
                    "~/Scripts/js/hotkeys/jquery.hotkeys.js",
                    "~/Scripts/js/wysiwyg/bootstrap-wysiwyg.min.js",
                    "~/Scripts/js/moment/moment.min.js",
                    "~/Scripts/js/daterangepicker/daterangepicker.js",
                    "~/Scripts/js/inputmask/jquery.inputmask.bundle.min.js",
                    "~/Scripts/js/select2/select2.full.js"));

            //Arquivos de css usados para tables
            bundles.Add(new StyleBundle("~/DataTables/styles").Include(
                    "~/Content/css/datatables/jquery.dataTables.min.css",
                    "~/Content/css/datatables/buttons.bootstrap.min.css",
                    "~/Content/css/datatables/fixedHeader.bootstrap.min.css",
                    "~/Content/css/datatables/responsive.bootstrap.min.css",
                    "~/Content/css/datatables/scroller.bootstrap.min.css"));

            //Arquivos de javascript usados para tables
            bundles.Add(new ScriptBundle("~/DataTables/scripts").Include(
                    "~/Scripts/js/datatables/jquery.dataTables.js",
                    "~/Scripts/js/datatables/dataTables.bootstrap.js",
                    "~/Scripts/js/datatables/dataTables.buttons.min.js",
                    "~/Scripts/js/datatables/buttons.bootstrap.min.js",
                    "~/Scripts/js/datatables/jszip.min.js",
                    "~/Scripts/js/datatables/pdfmake.min.js",
                    "~/Scripts/js/datatables/vfs_fonts.js",
                    "~/Scripts/js/datatables/buttons.html5.min.js",
                    "~/Scripts/js/datatables/buttons.print.min.js",
                    "~/Scripts/js/datatables/dataTables.fixedHeader.min.js",
                    "~/Scripts/js/datatables/dataTables.keyTable.min.js",
                    "~/Scripts/js/datatables/dataTables.responsive.min.js",
                    "~/Scripts/js/datatables/responsive.bootstrap.min.js",
                    "~/Scripts/js/datatables/dataTables.scroller.min.js"));

            //Arquivos de javascript usados nos perfil
            bundles.Add(new ScriptBundle("~/Perfil/scripts").Include(
                    "~/Scripts/js/raphael/raphael.js",
                    "~/Scripts/js/morris/morris.js",
                    "~/Scripts/js/bootstrap-progressbar/bootstrap-progressbar.js",
                    "~/Scripts/js/fastclick/fastclick.js"));


            //Arquivos de javascript usados no editar do perfil
            bundles.Add(new ScriptBundle("~/PerfilEditar/scripts").Include(
                    "~/Scripts/js/Upload/bootstrap-filestyle.js"));

            //Arquivos de javascript usados nas competencias
            bundles.Add(new ScriptBundle("~/Competencias/scripts").Include(
                    "~/Scripts/js/echarts/echarts.js"));

            //Arquivos de css usados na agenda
            bundles.Add(new StyleBundle("~/Agenda/styles").Include(
                    "~/Content/css/fullcalendar/fullcalendar.css",
                    "~/Content/css/fullcalendar/fullcalendar.print.css"));

            //Arquivos de javascript usados na agenda
            bundles.Add(new ScriptBundle("~/Agenda/scripts").Include(
                    "~/Scripts/js/moment/moment.min.js",
                    "~/Scripts/js/fullcalendar/fullcalendar.js"));
        }
    }
}

