using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Insignia.Painel.Helpers.Util
{
    public static class SelectListMVC
    {
        /// <summary>
        /// Cria Lista de seleção para dropdown com id do tipo inteiro
        /// </summary>
        /// <param name="dicionario">Dicionário que contém os dados</param>
        /// <returns>Lista de options</returns>
        public static List<SelectListItem> CriaListaSelecao(Dictionary<int, string> dicionario)
        {
            List<SelectListItem> itens = new List<SelectListItem>();

            foreach (var item in dicionario.Keys)
            {
                itens.Add(new SelectListItem { Text = dicionario[item].Replace(",", ", "), Value = Convert.ToString(item) });
            }

            return itens;
        }

        /// <summary>
        /// Cria lista de seleção para dropdown com id no formato de string
        /// </summary>
        /// <param name="dicionario">Dicionário que contem os dados</param>
        /// <returns>Lista de options</returns>
        public static List<SelectListItem> CriaListaSelecao(Dictionary<string, string> dicionario)
        {
            List<SelectListItem> itens = new List<SelectListItem>();

            foreach (var item in dicionario.Keys)
            {
                itens.Add(new SelectListItem { Text = dicionario[item].Replace(",", ", "), Value = Convert.ToString(item) });
            }

            return itens;
        }
    }
}