using RestSharp;
using RestSharp.Authenticators;
using Story_Spoil_Testing.Models;
using System.Net;
using System.Text.Json;

namespace Story_Spoil_Testing
{
    public class StorySpoilTesting
    {
        private RestClient client;

        [OneTimeSetUp]
        public void Setup()
        {
            string jwtToken = GetJwtToken("iv111","123456");

            var options = new RestClientOptions("https://d5wfqm7y6yb3q.cloudfront.net")
            {
                Authenticator = new JwtAuthenticator(jwtToken)
            };
                        
            this.client = new RestClient(options);
        }

        private string GetJwtToken(string username, string password)
        {
            RestClient tempClient = new RestClient("https://d5wfqm7y6yb3q.cloudfront.net");

            var request = new RestRequest("/api/User/Authentication", Method.Post);
            request.AddJsonBody(new
            {
                Username = username,
                Password = password
            });

            var response = tempClient.Post(request);
            

            if (response.StatusCode==HttpStatusCode.OK)
            {
                var content = JsonSerializer.Deserialize<JsonElement>(response.Content);
                var token = content.GetProperty("accessToken").GetString();
                if(string.IsNullOrEmpty(token))
                {
                    throw new InvalidOperationException("Token is null or empty");
                }
                return token;
            }
            else
            {
                throw new InvalidOperationException($"Invalid operation: {response.StatusCode} ");
            }


        }
        [Order(1)]
        [Test]
        public void Create_New_Story()
        {
            var newStory = new NewStoryDTO
            {
                Title = "New",
                Description = "Created",
                Url="http:"
            };

            var request = new RestRequest("/api/Story/Create", Method.Post);
            
            request.AddJsonBody(newStory);

            var response = this.client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        }
    }
}