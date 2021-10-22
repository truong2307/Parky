using Newtonsoft.Json;
using ParkyWeb.Models;
using ParkyWeb.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ParkyWeb.Repository
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly IHttpClientFactory _clientFactory;
        public UserRepository(IHttpClientFactory clientFactory) : base(clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<User> Login(string url, UserRequest userRequest)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            if (userRequest != null)
            {
                request.Content = new StringContent(
                    JsonConvert.SerializeObject(userRequest), Encoding.UTF8, "application/json"); // tuần tự hóa 1 object thành json
            }
            else
            {
                return null;
            }

            var client = _clientFactory.CreateClient();
            client = ClientSslBypass();
            HttpResponseMessage response = await client.SendAsync(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<User>(jsonString);
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> Register(string url, UserRequest userRequest)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            if (userRequest != null)
            {
                request.Content = new StringContent(
                    JsonConvert.SerializeObject(userRequest), Encoding.UTF8, "application/json"); // tuần tự hóa 1 object thành json
            }
            else
            {
                return false;
            }
            var client = _clientFactory.CreateClient();
            client = ClientSslBypass();

            HttpResponseMessage response = await client.SendAsync(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private HttpClient ClientSslBypass()
        {
            var httpClientHandler = new HttpClientHandler();
            //bypass SSL Certificate 
            httpClientHandler.ServerCertificateCustomValidationCallback
                = (message, cert, chain, sslPolicyErrors) =>
                {
                    return true;
                };

            return new HttpClient(httpClientHandler); //
        }
    }
}
