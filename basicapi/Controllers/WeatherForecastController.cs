using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
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

        [HttpGet("asd/{id}")]
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


        [HttpGet("cur/{type}")]
        public async Task<ActionResult<WeatherForecast>> Curreny(string type)
        {
          string siteContent = string.Empty;
            string url = "https://www.bloomberght.com/doviz/dolar";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())  // Go query google
            using (Stream responseStream = response.GetResponseStream())               // Load the response stream
            using (StreamReader streamReader = new StreamReader(responseStream))       // Load the stream reader to read the response
            {
                siteContent = streamReader.ReadToEnd(); // Read the entire response and store it in the siteContent variable
            }

            int firstStringPosition = siteContent.IndexOf("widget-interest-detail type1");
              int secondStringPosition = siteContent.IndexOf("DİĞER HABERLER");
              string stringBetweenTwoStrings = siteContent.Substring(firstStringPosition,
                  secondStringPosition - firstStringPosition);
              Regex regex = new Regex("([0-9],[0-9]{4})");
               Match match = regex.Match(stringBetweenTwoStrings);

               if (match.Success)
               {
                   return Ok(match.Groups[1].Value);
               }

            return Ok(stringBetweenTwoStrings);
        }


        [HttpGet("token")]
        public async Task<ActionResult<WeatherForecast>> token(string type)
        {

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ThisismySecretKey"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken("Test.com",
             "Test.com",
              null,
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);

            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
           
        }



    }
}
