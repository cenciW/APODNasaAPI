using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using APODNasaAPI;


namespace APODNasaAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class APODController : Controller
    {
        string url = "https://api.nasa.gov/planetary/apod?api_key=" + apiKey.APODkey;

        // Test get request
        [HttpGet(Name = "TestConn")]
        public async Task<IActionResult> TestConnection()
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    return Ok(responseBody);
                }
                else
                {
                    return StatusCode((int)response.StatusCode, response.ReasonPhrase);
                }
            }
        }

        //    // GetImageByDay

        [HttpGet("{date}", Name = "GetImageByDay")]
        public async Task<IActionResult> GetImageByDay(DateTime date)
        {
            string url = "https://api.nasa.gov/planetary/apod?api_key=" + apiKey.APODkey + "&date=" + date.ToString("yyyy-MM-dd");
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    return Ok(responseBody);
                }
                else
                {
                    return StatusCode((int)response.StatusCode, response.ReasonPhrase);
                }
            }
        }


       
    }
}
