using Foody_Web_App_Testing.Models;
using RestSharp;
using RestSharp.Authenticators;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Xml.Linq;

namespace Foody_Web_App_Testing
{
    public class FoodyAppTesting
    {
        private RestClient client;
        private static string foodId;

        [OneTimeSetUp]
        public void Setup()
        {
            string jwtToken = GetJwtToken("ivan111", "123456");

            var options = new RestClientOptions("http://softuni-qa-loadbalancer-2137572849.eu-north-1.elb.amazonaws.com:86")
            {
                Authenticator = new JwtAuthenticator(jwtToken)
            };

            this.client = new RestClient(options);
        }
        private string GetJwtToken(string username, string password)
        {
            var tempClient = new RestClient("http://softuni-qa-loadbalancer-2137572849.eu-north-1.elb.amazonaws.com:86");
            var request = new RestRequest("/api/User/Authentication", Method.Post);
            request.AddJsonBody(new
            {
                username,
                password
            });

            var response = tempClient.Execute(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = JsonSerializer.Deserialize<JsonElement>(response.Content);
                var token = content.GetProperty("accessToken").GetString();
                if (string.IsNullOrWhiteSpace(token))
                {
                    throw new InvalidOperationException("The JWT token is null or empty.");
                }
                return token;
            }
            else
            {
                throw new InvalidOperationException($"Authentication failed: {response.StatusCode}, {response.Content}");
            }
        }

        [Order(1)]
        [Test]
        public void Create_A_New_Food ()
        {
            var newFood = new FoodDTO
            {
                Name = "New food",
                Description = "Good",
                Url = ""
            };

            var request = new RestRequest("/api/Food/Create", Method.Post);
            request.AddJsonBody(newFood);

            var respose = client.Execute(request);

            Assert.That(respose.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            var content=JsonSerializer.Deserialize<ApiResponseDTO>(respose.Content);
            Assert.That(content.FoodId, Is.Not.Null);
            foodId= content.FoodId;

        }

        [Order(2)]
        [Test] 
        public void Edit ()
        {               
            var request = new RestRequest($"/api/Food/Edit/{foodId}", Method.Patch);
            request.AddJsonBody(new[]
            {
                new
                {
                    path = "/name",
                    op ="replace",
                    value ="edit"
                }
            });

            var respose = client.Execute(request);

            Assert.That(respose.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        }
        [Order(3)]
        [Test] 

        public void Get_All ()
        {
            var request = new RestRequest("/api/Food/All", Method.Get);

            var response = client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var content=JsonSerializer.Deserialize<List<ApiResponseDTO>>(response.Content);

            Assert.That(content, Is.Not.Empty);
        }
        [Order(4)]
        [Test]

        public void Delete ()
        {
            var request = new RestRequest($"/api/Food/Delete/{foodId}", Method.Delete);

            var response = client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var content=JsonSerializer.Deserialize<ApiResponseDTO>(response.Content);

            Assert.That(content.Msg, Is.EqualTo("Deleted successfully!"));
        }
        [Order(5)]
        [Test]

        public void Try_To_Create ( )
        {
            var newFood = new FoodDTO
            {
                
                Description = "Good",
                Url = ""
            };

            var request = new RestRequest("/api/Food/Create", Method.Post);
            request.AddJsonBody(newFood);

            var response = client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Order(6)]
        [Test]
        public void Try_To_Edit ()
        {
            var request = new RestRequest($"/api/Food/Edit/oooo", Method.Patch);
            request.AddJsonBody(new[]
            {
                new
                {
                    path = "/name",
                    op ="replace",
                    value ="edit"
                }
            });

            var response = client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            var content = JsonSerializer.Deserialize<ApiResponseDTO>(response.Content);
            Assert.That(content.Msg, Is.EqualTo("No food revues..."));

        }
        [Order(7)]
        [Test]
        public void Try_Delete ()
        {
            var request = new RestRequest($"/api/Food/Delete/pppppp", Method.Delete);

            var response = client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

            var content = JsonSerializer.Deserialize<ApiResponseDTO>(response.Content);

            Assert.That(content.Msg, Is.EqualTo("Unable to delete this food revue!"));
        }
    }
}