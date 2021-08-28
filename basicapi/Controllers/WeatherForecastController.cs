using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace basicapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WeatherForecast>> GetTodoItem(long id)
        {
            string siteContent = string.Empty;

      
            string url = "https://www.haberturk.com/havadurumu/Turkiye-TR/Istanbul/LTSI";

          
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip;

            
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())  // Go query google
            using (Stream responseStream = response.GetResponseStream())               // Load the response stream
            using (StreamReader streamReader = new StreamReader(responseStream))       // Load the stream reader to read the response
            {
                siteContent = streamReader.ReadToEnd(); // Read the entire response and store it in the siteContent variable
            }

            int firstStringPosition = siteContent.IndexOf("widget-weather-day-list type1");
            int secondStringPosition = siteContent.IndexOf("widget-weather-map type1");
            string stringBetweenTwoStrings = siteContent.Substring(firstStringPosition,
                secondStringPosition - firstStringPosition );
             firstStringPosition = stringBetweenTwoStrings.IndexOf("<li>");
             secondStringPosition = stringBetweenTwoStrings.IndexOf("</li>");
             stringBetweenTwoStrings = stringBetweenTwoStrings.Substring(firstStringPosition+4,
                secondStringPosition - firstStringPosition-4);
            Regex regex = new Regex("low-degree\\\">([0-9]{2})°</span>");
            Match match = regex.Match(stringBetweenTwoStrings);

            if (match.Success)
            {
                return Ok(match.Groups[1].Value);
            }

            return Ok("olamadi be hacim");

            //return NotFound();
        }


    }
}
