using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;
using APODNasaAPI.Secrets;
using APODNasaAPI.Repositories;
using APODNasaAPI.Domain;
using Microsoft.AspNetCore.Mvc.Diagnostics;

namespace APODNasaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class APODController : ControllerBase
    {

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<APODController> _logger;
        private readonly APODRepository _podRepository;
        private const string BaseUrl = "https://api.nasa.gov/planetary/apod";
        private readonly string _apiKey;

        public APODController(IHttpClientFactory httpClientFactory, ILogger<APODController> logger, APODRepository aPODRepository)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _apiKey = ApiKey.APODkey;
            _podRepository = aPODRepository;
        }

        private async Task<IActionResult> FetchDataAsync(string endpoint, bool multipleData)
        {
            using var client = _httpClientFactory.CreateClient();
            _logger.LogInformation("Buscando dados do Endpoint: {Endpoint}", endpoint);

            HttpResponseMessage response = await client.GetAsync(endpoint);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();

                if (multipleData)
                {
                    ApodData[]? dataMultiple = JsonSerializer.Deserialize<ApodData[]>(responseBody);


                    for (int i = 0; i < dataMultiple?.Length;i++) {
                        if (_podRepository.GetApodData(dataMultiple[i].Date) == null)
                            _podRepository.addData(dataMultiple[i]);
                    }

                    var formattedData = dataMultiple?.Select(data => data.GetRaw());


                    return Ok(formattedData);
                }


                ApodData? data = JsonSerializer.Deserialize<ApodData>(responseBody);
                
                _podRepository.addData(data);

                return Ok(data.GetRaw());
            }

            _logger.LogWarning("Requisição falhou, código: {StatusCode}: {ReasonPhrase}", (int)response.StatusCode, response.ReasonPhrase);
            return StatusCode((int)response.StatusCode, response.ReasonPhrase);
        }

        [HttpGet("image/{date}")]
        public async Task<IActionResult> GetImageByDay([FromRoute] string date)
        {

            if (!DateTime.TryParse(date, out var dateTime))
            {
                _logger.LogWarning("Formato de data inválido: {Date}", date);
                return BadRequest(new { error = "A data deve estar nesse formato: 'yyyy-MM-dd'." });
            }
            ApodData? apodDataCache = _podRepository.GetApodData(dateTime);
            if (apodDataCache != null)
            {
                Console.WriteLine("Retornando pelo cache.");
                return Ok(apodDataCache.GetRaw());
            }


            var endpoint = $"{BaseUrl}?api_key={_apiKey}&date={date}";
            return await FetchDataAsync(endpoint, false);
        }

        [HttpGet("images/{startDate}/{endDate}")]
        public async Task<IActionResult> GetImagesBetweenDates([FromRoute] string startDate, [FromRoute] string endDate)
        {

            if (!DateTime.TryParse(startDate, out DateTime start))
            {
                _logger.LogWarning("Formato de data inválido: {Date}", startDate);
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

            //Console.WriteLine(end.Subtract(start).Days.ToString());
            var endpoint = $"{BaseUrl}?api_key={_apiKey}&start_date={startDate}&end_date={endDate}";
            _logger.LogInformation("Endpoint gerado: {Endpoint}", endpoint);
            Console.WriteLine($"Endpoint gerado: {endpoint}");

            Console.WriteLine($"Received request: start={start}, end={end}");

            int days = end.Subtract(start).Days;
            List<ApodData> apodDatas = new List<ApodData>();
            for (int i = 0; i < days + 1; i++)
            {
                ApodData? apod = _podRepository.GetApodData(start.AddDays(i));

                if (apod != null) apodDatas.Add(apod);
                else break;
                
            }

            if (apodDatas.Count != (days + 1)) {
                return await FetchDataAsync(endpoint, true);
            }

            Console.WriteLine("Retornando dados do cache");
            return Ok(apodDatas.ToArray());
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
            return await FetchDataAsync(endpoint, true);
        }
    }
}
