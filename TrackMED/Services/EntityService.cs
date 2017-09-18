using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;  // From NuGet Package: Microsoft.AspNet.WebApi.Client 5.2.3. Supplies 'ReadAsAsync' method
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TrackMED.Models;

namespace TrackMED.Services
{
    public class EntityService<T>: IEntityService<T> where T: IEntity
    {
        private readonly Settings _settings;
        private string uri = null;

        // https://docs.asp.net/en/latest/mvc/controllers/dependency-injection.html#accessing-settings-from-a-controller
        public EntityService(IOptions<Settings> optionsAccessor)
        {
            _settings = optionsAccessor.Value; // reads appsettings.json
        }

        public async Task<List<T>> GetEntitiesAsync(string id = null, CancellationToken cancelToken = default(CancellationToken))
        {
            uri = id == null ? getServiceUri(typeof(T).Name) : getServiceUri(typeof(T).Name + "/" + id);

            // HttpClient Class: https://msdn.microsoft.com/en-us/library/system.net.http.httpclient%28v=vs.118%29.aspx?f=255&MSPPError=-2147217396
            using (HttpClient httpClient = new HttpClient())
            {
                // From http://www.asp.net/web-api/overview/advanced/calling-a-web-api-from-a-net-client
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await httpClient.GetAsync(uri, cancelToken);
               
                var dataString = response.Content.ReadAsStringAsync().Result;
                List<T> objList = JsonConvert.DeserializeObject<List<T>>(dataString);

                return objList;
            }
        }

        /*
        public async Task<T> GetEntityAsyncByOtherThanId(string OtherThanId,
            CancellationToken cancelToken = default(CancellationToken))
        {
            // String.IndexOfAny Method https://msdn.microsoft.com/en-us/library/11w09h50(v=vs.110).aspx
            char[] chars = { '&', '/', '%', '$' };
            //int indexSlash = OtherThanId.IndexOf("/");
            int indexSpecial = OtherThanId.IndexOfAny(chars);
            if (indexSpecial >= 0)
            {
                // http://stackoverflow.com/questions/24978885/asp-c-special-characters-cant-pass-trough-url-parameter
                OtherThanId = Uri.EscapeDataString(OtherThanId);
            }

            uri = getServiceUri(typeof(T).Name + "/OtherThanId/" + OtherThanId);
            using (HttpClient httpClient = new HttpClient()) 
            {
                var response = await httpClient.GetAsync(uri, cancelToken);
                //return (await response.Content.ReadAsAsync<T>());

                var dataString = response.Content.ReadAsStringAsync().Result;
                T obj = JsonConvert.DeserializeObject<T>(dataString);
                return obj;
            }
        }
        */

        public async Task<List<T>> GetEntitiesManyAsync(List<string> ids, CancellationToken cancelToken = default(CancellationToken))
        {
            uri = getServiceUri(typeof(T).Name + "/multiples" + "/" + ids);
            // HttpClient Class: https://msdn.microsoft.com/en-us/library/system.net.http.httpclient%28v=vs.118%29.aspx?f=255&MSPPError=-2147217396
            using (HttpClient httpClient = new HttpClient())
            {
                // From http://www.asp.net/web-api/overview/advanced/calling-a-web-api-from-a-net-client
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await httpClient.GetAsync(uri, cancelToken);

                var dataString = response.Content.ReadAsStringAsync().Result;
                List<T> objList = JsonConvert.DeserializeObject<List<T>>(dataString);

                return objList;
            }
        }

        public async Task<List<T>> GetSelectedEntitiesAsync(string tableID, string id, CancellationToken cancelToken = default(CancellationToken))
        {
            // http://www.c-sharpcorner.com/UploadFile/2b481f/pass-multiple-parameter-in-url-in-web-api/
            uri = getServiceUri(typeof(T).Name + "/" + tableID + "/" + id);
            // HttpClient Class: https://msdn.microsoft.com/en-us/library/system.net.http.httpclient%28v=vs.118%29.aspx?f=255&MSPPError=-2147217396
            using (HttpClient httpClient = new HttpClient())
            {
                // From http://www.asp.net/web-api/overview/advanced/calling-a-web-api-from-a-net-client
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await httpClient.GetAsync(uri, cancelToken);
                
                var dataString = response.Content.ReadAsStringAsync().Result;
                List<T> objList = JsonConvert.DeserializeObject<List<T>>(dataString);

                return objList;
                /*
                    {"assetnumber":"018291",
                     "descriptionID":"59ba4d551bd003082cc15437",
                     "model_ManufacturerID":"59ba4dc91bd003082cc15460",
                     "serviceProviderID":null,
                     "description":{"id":"59ba4d551bd003082cc15437","desc":"Thermal Mass Flow Meter","tag":null,"createdAtUtc":"2017-09-14T17:35:09.969+08:00"},
                     "model_Manufacturer":{"id":"59ba4dc91bd003082cc15460","desc":"TSI 40401",
                     "createdAtUtc":"2017-09-14T17:37:00.561+08:00"},
                     "serviceProvider":null,
                     "calibrationDate":null,
                     "calibrationInterval":null,
                     "maintenanceDate":null,
                     "maintenanceInterval":null,
                     "imteModule":null,
                     "id":"59bb62b11bd0032a68eed939",
                     "imte":"018291",
                     "serialnumber":"40401608001",
                     "notes":"",
                     "createdAtUtc":"2017-09-15T13:18:34.887+08:00",
                     "ownerID":"59ba4dc51bd003082cc15454",
                     "statusID":null,
                     "activityTypeID":null,
                     "owner":{"id":"59ba4dc51bd003082cc15454","desc":"Thap Tien","createdAtUtc":"2017-09-14T17:37:00.954+08:00"},
                     "status":null,
                     "activityType":null},
                 */
            }
        }

        public async Task<T> GetEntityAsync(string id, CancellationToken cancelToken = default(CancellationToken))
        {
            uri = getServiceUri(typeof(T).Name + "/" + id);
            using (HttpClient httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(uri, cancelToken);

                var dataString = response.Content.ReadAsStringAsync().Result;
                T obj = JsonConvert.DeserializeObject<T>(dataString);
                return obj;
            }
        }

        public async Task<T> GetEntityAsyncByDescription(string Description, CancellationToken cancelToken = default(CancellationToken))
        {
            char[] chars = { '&', '/', '%', '$' };
            int indexSpecial = Description.IndexOfAny(chars);   // String.IndexOfAny Method https://msdn.microsoft.com/en-us/library/11w09h50(v=vs.110).aspx
            if (indexSpecial >= 0)
            {
                // http://stackoverflow.com/questions/24978885/asp-c-special-characters-cant-pass-trough-url-parameter
                Description = Uri.EscapeDataString(Description);
            }

            uri = getServiceUri(typeof(T).Name + "/Desc/" + Description);
            using (HttpClient httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(uri, cancelToken);

                var dataString = response.Content.ReadAsStringAsync().Result;
                T obj = JsonConvert.DeserializeObject<T>(dataString);
                return obj;
            }
        }

        public async Task<T> GetEntityAsyncByFieldID(string fieldID, string id, string tableID, CancellationToken cancelToken = default(CancellationToken))
        {
            uri = getServiceUri(typeof(T).Name + "/" + fieldID + "/" + id + "/" + tableID);
            using (HttpClient httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(uri, cancelToken);
                //return (await response.Content.ReadAsAsync<T>());

                var dataString = response.Content.ReadAsStringAsync().Result;
                T obj = JsonConvert.DeserializeObject<T>(dataString);
                return obj;
            }
        }

        public async Task<bool> DeleteEntityAsync(string id, CancellationToken cancelToken = default(CancellationToken))
        {
            uri = getServiceUri(typeof(T).Name + "/" + id);
            using (HttpClient httpClient = new HttpClient())
            {
                var response = await httpClient.DeleteAsync(uri, cancelToken);
                return (response.IsSuccessStatusCode);
            }
        }

        public async Task<HttpResponseMessage> EditEntityAsync(T Entity, CancellationToken cancelToken = default(CancellationToken))
        {
            uri = getServiceUri(typeof(T).Name);
            using (HttpClient httpClient = new HttpClient())
            {
                //var response = await httpClient.PutAsJsonAsync(uri, Entity, cancelToken);

                string json = JsonConvert.SerializeObject(Entity, Formatting.Indented);
                HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PutAsync(uri, httpContent, cancelToken);
               
                return response.EnsureSuccessStatusCode();
            }
        }

        /*
        public async Task<HttpResponseMessage> EditEntitiesAsync(List<string> ids, CancellationToken cancelToken = default(CancellationToken))
        {
            uri = getServiceUri(typeof(T).Name + "/multiples/" + ids);
            using (HttpClient httpClient = new HttpClient())
            {
                var response = await httpClient.PutAsJsonAsync(uri, Entity, cancelToken);
                return response.EnsureSuccessStatusCode();
            }
        }
        */

        public async Task<T> PostEntityAsync( T Entity, CancellationToken cancelToken = default(CancellationToken))
        {
            uri = getServiceUri(typeof(T).Name);
            using (HttpClient httpClient = new HttpClient())
            {
                // old code
                // var response = await httpClient.PostAsJsonAsync(uri, Entity);
                // return (await response.Content.ReadAsAsync<T>());

                string json = JsonConvert.SerializeObject(Entity, Formatting.Indented);
                HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(uri, httpContent, cancelToken);

                var dataString = response.Content.ReadAsStringAsync().Result;
                T obj = JsonConvert.DeserializeObject<T>(dataString);
                return obj;
            }
        }

        // public async Task<HttpResponseMessage> PostEntitiesAsync(List<T> Entities, CancellationToken cancelToken = default(CancellationToken))
        public async Task<T> PostEntitiesAsync(List<T> Entities, CancellationToken cancelToken = default(CancellationToken))
        {
            uri = getServiceUri(typeof(T).Name + "/multiples/" + Entities);
            using (HttpClient httpClient = new HttpClient())
            {
                // old code
                // var response = await httpClient.PostAsJsonAsync(uri, Entities);
                // return (await response.Content.ReadAsAsync<T>());

                string json = JsonConvert.SerializeObject(Entities, Formatting.Indented);
                HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(uri, httpContent, cancelToken);

                var dataString = response.Content.ReadAsStringAsync().Result;
                T obj = JsonConvert.DeserializeObject<T>(dataString);
                return obj;
            }
        }

        public async Task<T> VerifyEntityAsync(string id, CancellationToken cancelToken = default(CancellationToken))
        {
            uri = getServiceUri(typeof(T).Name + "/" + id);
            using (HttpClient httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(uri, cancelToken);
                //return (await response.Content.ReadAsAsync<T>());

                var dataString = response.Content.ReadAsStringAsync().Result;
                T obj = JsonConvert.DeserializeObject<T>(dataString);
                return obj;
            }
        }
        /* Comment out temporarily
        public async Task<bool> DropDatabaseAsync(CancellationToken cancelToken = default(CancellationToken))
        {
            uri = _settings.TrackMEDApi;
            using (HttpClient httpClient = new HttpClient())
            {
                var response = await httpClient.PutAsJsonAsync(uri, cancelToken);
                return (response.IsSuccessStatusCode);
            }
        }
        */
        public string getServiceUri(string srv)
        {
            return _settings.TrackMEDApi + "api/" + srv;
        }
    }
}