using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WPFGen
{
    public enum BrandColors 
    { 
        GulfCoast,
        SummerSky,
        BlueBonnet,
        TexasNights,
        TexasSunset,
        RedRiver,
        HillCountry
    }

    public static class BrandStyles
    {
        public static string HexFromBrandColor(BrandColors brandColor)
        {
            var hexString = "$000000";
            switch (brandColor)
            {
                case BrandColors.GulfCoast:
                    hexString = "#BEDFEE";
                    break;
                case BrandColors.SummerSky:
                    hexString = "#30A7D6";
                    break;
                case BrandColors.BlueBonnet:
                    hexString = "#184467";
                    break;
                case BrandColors.TexasNights:
                    hexString = "#0A131B";
                    break;
                case BrandColors.TexasSunset:
                    hexString = "#F75A3B";
                    break;
                case BrandColors.RedRiver:
                    hexString = "#CF1710";
                    break;
                case BrandColors.HillCountry:
                    hexString = "#1DB051";
                    break;
            }
            return hexString;
        }

        public static Brush BrushFromBrandColor(BrandColors brandColor)
        {
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString(HexFromBrandColor(brandColor)));
        }
    }
}
