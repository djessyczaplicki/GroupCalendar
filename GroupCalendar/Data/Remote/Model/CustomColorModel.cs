using Newtonsoft.Json;
using System;
using System.Windows.Media;

namespace GroupCalendar.Data.Remote.Model
{
    public partial class CustomColorModel
    {
        [JsonProperty("blue")]
        public int Blue { get; set; }

        [JsonProperty("green")]
        public int Green { get; set; }

        [JsonProperty("red")]
        public int Red { get; set; }

        public CustomColorModel() { }

        public CustomColorModel(int blue, int green, int red)
        {
            Blue = blue;
            Green = green;
            Red = red;
        }

        public CustomColorModel(Color color)
        {
            Blue = color.B;
            Green = color.G;
            Red = color.R;
        }

        public override string ToString()
        {
            string rs = DecimalToHexadecimal(Red);
            string gs = DecimalToHexadecimal(Green);
            string bs = DecimalToHexadecimal(Blue);
            return '#' + rs + gs + bs;
        }

        private string DecimalToHexadecimal(int color)
        {
            if (color <= 0)
                return "00";

            int hex = color;
            string hexStr = string.Empty;

            while (color > 0)
            {
                hex = color % 16;

                if (hex < 10)
                    hexStr = hexStr.Insert(0, Convert.ToChar(hex + 48).ToString());
                else
                    hexStr = hexStr.Insert(0, Convert.ToChar(hex + 55).ToString());

                color /= 16;
            }

            return hexStr;
        }
    }
}
