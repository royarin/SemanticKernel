using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KernelWithMultiplePlugins.Plugins
{
    public sealed class WeatherPlugin
    {
        [KernelFunction("GetWeather"), Description("Retrieve the weather for a city")]
        public int GetWeather([Description("The city for which weather must be retrieved")] string city)
        {
            //generate a random number between 0 and 30

            Random random = new Random();
            int temperature = random.Next(0, 30);

            return temperature;

        }
    }
}
