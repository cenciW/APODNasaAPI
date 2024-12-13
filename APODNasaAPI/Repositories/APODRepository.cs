using APODNasaAPI.Domain;

namespace APODNasaAPI.Repositories
{
    public class APODRepository
    {

        private List<ApodData> cacheImages = new List<ApodData>();

        public APODRepository()
        {

            /*
             
             {
                  "date": "2024-12-11",
                  "explanation": "What's the closest active galaxy to planet Earth? That would be Centaurus A, cataloged as NGC 5128, which is only 12 million light-years distant. Forged in a collision of two otherwise normal galaxies, Centaurus A shows several distinctive features including a dark dust lane across its center, outer shells of stars and gas, and jets of particles shooting out from a supermassive black hole at its center.  The featured image captures all of these in a composite series of visible light images totaling over 310 hours captured over the past 10 years with a homebuilt telescope operating in Auckland, New Zealand. The brightness of Cen A's center from low-energy radio waves to high-energy gamma rays underlies its designation as an active galaxy.    Astrophysicists: Browse 3,500+ codes in the Astrophysics Source Code Library",
                  "hdurl": "https://apod.nasa.gov/apod/image/2412/CenAShellsJets_Olsen_6150.jpg",
                  "media_type": "image",
                  "service_version": "v1",
                  "title": "The Shells and Jets of Galaxy Centaurus A",
                  "url": "https://apod.nasa.gov/apod/image/2412/CenAShellsJets_Olsen_1080.jpg"
            }
             */

            cacheImages.Add(new ApodData(
                DateTime.Parse("2024-12-11"), 
                "What's the closest active galaxy to planet Earth? That would be Centaurus A, cataloged as NGC 5128, which ",
                "The Shells and Jets of Galaxy Centaurus A",
                "https://apod.nasa.gov/apod/image/2412/CenAShellsJets_Olsen_1080.jpg")
                );


        }

        public ApodData? GetApodData(DateTime dateTime)
        {
            ApodData? ret = cacheImages.FirstOrDefault(data => data.Date == dateTime);

            return ret;
        }

        public void addData(ApodData data)
        {
            if (data == null) return;

            ApodData? check = GetApodData(data.Date);

            if (check == null)
            {
                cacheImages.Add(data);
            }
        }
    }
}
