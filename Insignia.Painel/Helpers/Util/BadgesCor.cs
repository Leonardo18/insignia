using System;
using System.Drawing;
using System.Globalization;

namespace Insignia.Painel.Helpers.Util
{
    public class BadgesCor
    {
        /// <summary>
        /// Converte cor de Hex para RGB e retorna brano ou preto para a cor da fonte por contraste
        /// </summary>
        /// <param name="hexColor">Hexadecimal da cor</param>
        /// <returns>Retorna string com qual cor a letra deve ter</returns>
        public string HexToColor(string hexColor)
        {
            //Remove se contém #
            if (hexColor.IndexOf('#') != -1)
                hexColor = hexColor.Replace("#", "");

            int red = 0;
            int green = 0;
            int blue = 0;

            if (hexColor.Length == 6)
            {
                //#RRGGBB
                red = int.Parse(hexColor.Substring(0, 2), NumberStyles.AllowHexSpecifier);
                green = int.Parse(hexColor.Substring(2, 2), NumberStyles.AllowHexSpecifier);
                blue = int.Parse(hexColor.Substring(4, 2), NumberStyles.AllowHexSpecifier);
            }
            else if (hexColor.Length == 3)
            {
                //#RGB
                red = int.Parse(hexColor[0].ToString() + hexColor[0].ToString(), NumberStyles.AllowHexSpecifier);
                green = int.Parse(hexColor[1].ToString() + hexColor[1].ToString(), NumberStyles.AllowHexSpecifier);
                blue = int.Parse(hexColor[2].ToString() + hexColor[2].ToString(), NumberStyles.AllowHexSpecifier);
            }

            var intensivity = (PerceivedBrightness(Color.FromArgb(red, green, blue)) > 130 ? Color.Black : Color.White);

            return intensivity.Name;
        }

        /// <summary>
        /// Calcula o brilho da cor
        /// </summary>
        /// <param name="c">Contém a cor a ser verificada</param>
        /// <returns>Retorna um valor para o contraste</returns>
        private int PerceivedBrightness(Color c)
        {
            return (int)Math.Sqrt(
            c.R * c.R * .299 +
            c.G * c.G * .587 +
            c.B * c.B * .114);
        }
    }
}