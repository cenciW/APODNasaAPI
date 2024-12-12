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

        //// Other methods...

        //// GET: APODController
        //public ActionResult Index()
        //{
        //    return View();
        //}

        //// GET: APODController/Details/5
        //public ActionResult Details(int id)
        //{
        //    return View();
        //}

        //// GET: APODController/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //// POST: APODController/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(IFormCollection collection)
        //{
        //    try
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: APODController/Edit/5
        //public ActionResult Edit(int id)
        //{
        //    return View();
        //}

        //// POST: APODController/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(int id, IFormCollection collection)
        //{
        //    try
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: APODController/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        //// POST: APODController/Delete/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Delete(int id, IFormCollection collection)
        //{
        //    try
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
    }
}
