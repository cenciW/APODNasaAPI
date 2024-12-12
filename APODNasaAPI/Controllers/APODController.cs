//using Microsoft.AspNetCore.Mvc;
//using System.Net.Http;
//using System.Text.Json;
//using System.Threading.Tasks;

//namespace APODNasaAPI.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class APODController : ControllerBase
//    {
//        private readonly IHttpClientFactory _httpClientFactory;
//        private const string BaseUrl = "https://api.nasa.gov/planetary/apod";
//        private readonly string _apiKey;

//        public APODController(IHttpClientFactory httpClientFactory)
//        {
//            _httpClientFactory = httpClientFactory;
//            _apiKey = apiKey.APODkey; // Replace with dependency injection or configuration
//        }

//        // Fetch data from the API
//        private async Task<IActionResult> FetchDataAsync(string endpoint)
//        {
//            using var client = _httpClientFactory.CreateClient();
//            HttpResponseMessage response = await client.GetAsync(endpoint);

//            if (response.IsSuccessStatusCode)
//            {
//                var responseBody = await response.Content.ReadAsStringAsync();
//                return Ok(JsonSerializer.Deserialize<object>(responseBody)); // Ensure the response is properly formatted
//            }

//            return StatusCode((int)response.StatusCode, response.ReasonPhrase);
//        }

//        // Test get request
//        [HttpGet("test-connection")]
//        public async Task<IActionResult> TestConnection()
//        {
//            var endpoint = $"{BaseUrl}?api_key={_apiKey}";
//            return await FetchDataAsync(endpoint);
//        }


//        //GetImageByDay
//        [HttpGet("image/{date}")]
//        public async Task<IActionResult> GetImageByDay([FromRoute] string date)
//        {
//            var endpoint = $"{BaseUrl}?api_key={_apiKey}&date={date}";
//            return await FetchDataAsync(endpoint);
//        }


//        // GetRandomImageByAmount
//        [HttpGet("images/{startDate}/{endDate}")]
//        public async Task<IActionResult> GetImagesBetweenDates([FromRoute] string startDate, [FromRoute] string endDate)
//        {
//            var endpoint = $"{BaseUrl}?api_key={_apiKey}&start_date={startDate}&end_date={endDate}";
//            return await FetchDataAsync(endpoint);
//        }

//        // GetRandomImageByAmount
//        [HttpGet("random/{amount}")]
//        public async Task<IActionResult> GetRandomImages([FromRoute] int amount)
//        {
//            var endpoint = $"{BaseUrl}?api_key={_apiKey}&count={amount}";
//            return await FetchDataAsync(endpoint);
//        }
//    }
//}

using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;

namespace APODNasaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class APODController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<APODController> _logger;
        private const string BaseUrl = "https://api.nasa.gov/planetary/apod";
        private readonly string _apiKey;

        public APODController(IHttpClientFactory httpClientFactory, ILogger<APODController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _apiKey = apiKey.APODkey; // Replace with dependency injection or configuration
        }

        private async Task<IActionResult> FetchDataAsync(string endpoint)
        {
            using var client = _httpClientFactory.CreateClient();
            _logger.LogInformation("Fetching data from endpoint: {Endpoint}", endpoint);

            HttpResponseMessage response = await client.GetAsync(endpoint);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                return Ok(JsonSerializer.Deserialize<object>(responseBody)); // Ensure the response is properly formatted
            }

            _logger.LogWarning("Request failed with status code {StatusCode}: {ReasonPhrase}", (int)response.StatusCode, response.ReasonPhrase);
            return StatusCode((int)response.StatusCode, response.ReasonPhrase);
        }

        [HttpGet("test-connection")]
        public async Task<IActionResult> TestConnection()
        {
            var endpoint = $"{BaseUrl}?api_key={_apiKey}";
            return await FetchDataAsync(endpoint);
        }

        [HttpGet("image/{date}")]
        public async Task<IActionResult> GetImageByDay([FromRoute] string date)
        {
            if (!DateTime.TryParse(date, out _))
            {
                _logger.LogWarning("Invalid date format: {Date}", date);
                return BadRequest("Date must be in the format 'yyyy-MM-dd'.");
            }

            var endpoint = $"{BaseUrl}?api_key={_apiKey}&date={date}";
            return await FetchDataAsync(endpoint);
        }

        [HttpGet("images/{startDate}/{endDate}")]
        public async Task<IActionResult> GetImagesBetweenDates([FromRoute] string startDate, [FromRoute] string endDate)
        {
            if (!DateTime.TryParse(startDate, out var start) || !DateTime.TryParse(endDate, out var end))
            {
                _logger.LogWarning("Invalid date range: startDate={StartDate}, endDate={EndDate}", startDate, endDate);
                return BadRequest("Dates must be in the format 'yyyy-MM-dd'.");
            }

            if ((end - start).TotalDays > 100)
            {
                _logger.LogWarning("Date range too large: startDate={StartDate}, endDate={EndDate}", startDate, endDate);
                return BadRequest("The date range cannot exceed 100 days.");
            }

            var endpoint = $"{BaseUrl}?api_key={_apiKey}&start_date={startDate}&end_date={endDate}";
            return await FetchDataAsync(endpoint);
        }

        [HttpGet("random/{amount}")]
        public async Task<IActionResult> GetRandomImages([FromRoute] int amount)
        {
            if (amount <= 0)
            {
                _logger.LogWarning("Invalid amount requested: {Amount}", amount);
                return BadRequest("Amount must be a positive integer.");
            }

            var endpoint = $"{BaseUrl}?api_key={_apiKey}&count={amount}";
            return await FetchDataAsync(endpoint);
        }
    }
}
