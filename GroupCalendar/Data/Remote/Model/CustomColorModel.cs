using Newtonsoft.Json;
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
    }
}
