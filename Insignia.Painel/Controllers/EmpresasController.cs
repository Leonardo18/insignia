using Insignia.DAO.Competencias;
using Insignia.DAO.Empresas;
using Insignia.DAO.Tarefas;
using Insignia.DAO.Util;
using Insignia.Model.Empresa;
using Insignia.Painel.Helpers.AmazonS3;
using Insignia.Painel.Helpers.CustomAttributes;
using Insignia.Painel.Helpers.Util;
using Insignia.Painel.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace Insignia.Painel.Controllers
{
    public class EmpresasController : Controller
    {
        private EmpresasDAO EmpresaDAO = new EmpresasDAO(ConfigurationManager.ConnectionStrings["strConMain"].ConnectionString);

        /// <summary>
        /// GET: Empresa Perfil
        /// </summary>        
        /// <returns>Retorna a view com os dados da empresa</returns>
        [HttpGet, IsLogged]
        public ActionResult Perfil(int id)
        {
            var ViewModel = new ViewModelPerfil();

            ViewModel.Empresa = EmpresaDAO.Carregar(id);

            TarefasDAO TarefasDAO = new TarefasDAO(ConfigurationManager.ConnectionStrings["strConMain"].ConnectionString);

            //Busca as tarefa com status finalizada e top 5
            ViewModel.ListFinalizadas = TarefasDAO.ListarTop(ConfigurationManager.AppSettings["Finalizada"], 0, 5);

            ViewModel.TarefasMes = new List<int>();

            for (int i = 1; i <= 12; i++)
            {
                ViewModel.TarefasMes.Add(TarefasDAO.QuantidadeTarefasMes(i));
            }

            CompetenciasDAO CompetenciasDAO = new CompetenciasDAO(ConfigurationManager.ConnectionStrings["strConMain"].ConnectionString);

            ViewModel.ListCompetencias = CompetenciasDAO.Listar();

            foreach (var item in ViewModel.ListCompetencias)
            {
                item.Pontos = CompetenciasDAO.CompetenciaPontos(item.ID);
            }

            return View(ViewModel);
        }

        /// <summary>
        /// GET: Empresa PerfilEditar
        /// </summary>        
        /// <returns>Retorna a view com os dados da empresa para edição</returns>
        [HttpGet, IsLogged]
        public ActionResult PerfilEditar(int id)
        {
            var EmpresaModel = EmpresaDAO.Carregar(id);

            //Busca os  e retorna um dictionary contendo os dados
            var TodosEstados = SelectListMVC.CriaListaSelecao(EmpresaDAO.Estados());

            //Retorna na list o valor marcado atualmente para o cadastro
            foreach (var item in TodosEstados)
            {
                if (item.Text == EmpresaModel.Estado)
                {
                    item.Selected = true;
                    break;
                }
            }

            ViewBag.Estados = TodosEstados;

            return View(EmpresaModel);
        }

        /// <summary>
        /// POST: Empresa PerfilEditar
        /// </summary>        
        /// <returns>Retorna a view com os dados da empresa para edição</returns>
        [HttpPost, IsLogged]
        public ActionResult PerfilEditar(Empresa EmpresaModel, HttpPostedFileBase Foto)
        {
            // Verifica se existe um arquivo escolhido
            if (Foto != null && Foto.ContentLength > 0)
            {
                AmazonUpload AmazonS3 = new AmazonUpload();

                //Verifica se possui arquivo antigo para substituição na amazon
                string ArquivoAntigo = Database.DBBuscaInfo("Empresas", "ID", Convert.ToString(EmpresaModel.ID), "Foto");

                // Pega o nome do arquivo
                EmpresaModel.Foto = Path.GetFileName(Foto.FileName);

                // Grava o arquivo em uma pasta local
                var Caminho = Path.Combine(Server.MapPath("~/Content/uploads"), EmpresaModel.Foto);
                Foto.SaveAs(Caminho);

                //Verifica se existe a pasta da empresa no Bucket
                if (!AmazonS3.ExistePasta(Convert.ToString(Session["EmpresaNome"]), "Fotos", ConfigurationManager.AppSettings["BucketName"]))
                {
                    //Cria uma pasta no Bucket com o nome da empresa
                    AmazonS3.CriaPasta(Convert.ToString(Session["EmpresaNome"]), "Fotos", ConfigurationManager.AppSettings["BucketName"]);
                }

                //Apaga arquivo antigo para fazer upload de um novo
                AmazonS3.ApagaArquivo(ConfigurationManager.AppSettings["BucketName"], Convert.ToString(Session["EmpresaNome"]), "Fotos", ArquivoAntigo);

                //Faz Upload do arquivo para o S3
                AmazonS3.EnviaArquivoS3(Caminho, ConfigurationManager.AppSettings["BucketName"], Convert.ToString(Session["EmpresaNome"]), "Fotos", EmpresaModel.Foto);

                //Seleta o arquivo salvo localmente
                System.IO.File.Delete(Caminho);
            }
            else
            {
                //Se não tem arquivo novo, mantém o antigo
                EmpresaModel.Foto = Database.DBBuscaInfo("Empresas", "ID", Convert.ToString(EmpresaModel.ID), "Foto");
            }

            if (EmpresaDAO.EditarPerfil(EmpresaModel))
            {
                Session["UsuarioFoto"] = EmpresaModel.Foto;

                return RedirectToAction("Perfil", new { ID = EmpresaModel.ID });
            }

            //Busca e retorna um dictionary contendo os dados
            var TodosEstados = SelectListMVC.CriaListaSelecao(EmpresaDAO.Estados());

            foreach (var item in TodosEstados)
            {
                if (item.Text == EmpresaModel.Estado)
                {
                    item.Selected = true;
                    break;
                }
            }

            ViewBag.Estados = TodosEstados;

            return RedirectToAction("PerfilEditar", EmpresaModel);
        }
    }
}