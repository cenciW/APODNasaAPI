using System.Text.Json.Serialization;

namespace APODNasaAPI.Domain
{
    public class ApodData
    {
        private DateTime _date;
        private string _explanation;
        private string _title;
        private string _url;

        public ApodData(DateTime date, string explanation, string title, string url)
        {
            _explanation = explanation;
            _date = date;
            _title = title;
            _url = url;
        }

        [JsonPropertyName("date")] 
        public DateTime Date { get { return _date; } set { _date = value; } }        

        [JsonPropertyName("explanation")]
        public string Explanation { get { return _explanation; } set { _explanation = value; } }



        [JsonPropertyName("title")]
        public string Title { get { return _title; } set { _title = value; } }



        [JsonPropertyName("url")]
        public string Url { get { return _url; } set { _url = value; } }


        public Object GetRaw()
        {
            return new { date = _date.ToString("yyyy-MM-dd"), explanation = _explanation, title = _title, url = _url };
        }

    }
}
