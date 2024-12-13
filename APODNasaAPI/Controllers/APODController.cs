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
            _apiKey = ApiKey.APODkey;
        }

        private async Task<IActionResult> FetchDataAsync(string endpoint)
        {
            using var client = _httpClientFactory.CreateClient();
            _logger.LogInformation("Buscando dados do Endpoint: {Endpoint}", endpoint);

            HttpResponseMessage response = await client.GetAsync(endpoint);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                return Ok(JsonSerializer.Deserialize<object>(responseBody));
            }

            _logger.LogWarning("Requisição falhou, código: {StatusCode}: {ReasonPhrase}", (int)response.StatusCode, response.ReasonPhrase);
            return StatusCode((int)response.StatusCode, response.ReasonPhrase);
        }

        [HttpGet("image/{date}")]
        public async Task<IActionResult> GetImageByDay([FromRoute] string date)
        {
            if (!DateTime.TryParse(date, out _))
            {
                _logger.LogWarning("Formato de data inválido: {Date}", date);
                return BadRequest(new { error = "A data deve estar nesse formato: 'yyyy-MM-dd'." });
            }

            var endpoint = $"{BaseUrl}?api_key={_apiKey}&date={date}";
            return await FetchDataAsync(endpoint);
        }

        [HttpGet("images/{startDate}/{endDate}")]
        public async Task<IActionResult> GetImagesBetweenDates([FromRoute] string startDate, [FromRoute] string endDate)
        {

            if (!DateTime.TryParse(startDate, out DateTime start))
            {
                _logger.LogWarning("Formato de data inválido: {Date}", start);
                return BadRequest(new { error = "A data inicial inserida deve ser válida" });
            }

            if (!DateTime.TryParse(endDate, out DateTime end))
            {
                _logger.LogWarning("Formato de data inválido: {Date}", endDate);
                return BadRequest(new { error = "A data final inserida deve ser válida" });
            }

            if (DateTime.Compare(start, end) > 0)
            {
                _logger.LogWarning("Intervalo de datas inválido: Data de Inicio={StartDate}, Data de Fim={EndDate}", startDate, endDate);
                return BadRequest(new { error = "A data final deve ser maior que a atual" });
            }

            if (end.Subtract(start).Days > 30)
            {
                _logger.LogWarning("Intervalo de datas muito grande: Data de Inicio={StartDate}, Data de Fim={EndDate}", startDate, endDate);
                return BadRequest(new { error = "A diferença entre datas deve ser menor que 30 dias" });
            }

            var endpoint = $"{BaseUrl}?api_key={_apiKey}&start_date={startDate}&end_date={endDate}";
            _logger.LogInformation("Endpoint gerado: {Endpoint}", endpoint);

            return await FetchDataAsync(endpoint);
        }

        [HttpGet("random/{amount}")]
        public async Task<IActionResult> GetRandomImages([FromRoute] int amount)
        {
            if (amount <= 0)
            {
                _logger.LogWarning("Quantidade de imagens solicitadas inválido: {Amount}", amount);
                return BadRequest(new { error = "Quantidade de imagens precisa ser um número positivo." });
            }

            var endpoint = $"{BaseUrl}?api_key={_apiKey}&count={amount}";
            return await FetchDataAsync(endpoint);
        }
    }
}
