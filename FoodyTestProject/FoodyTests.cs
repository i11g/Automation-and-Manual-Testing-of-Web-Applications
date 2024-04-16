using RestSharp;
using RestSharp.Authenticators;
using System.Text.Json;


namespace FoodyTestProject
{
    public class FoodyTests
    {
        private RestClient client; 

        [OneTimeSetUp]
        public void Setup()
        {
            string accessToken = GetJwtToken("ivan111", "123456");
                
            var options = new RestClientOptions("http://softuni-qa-loadbalancer-2137572849.eu-north-1.elb.amazonaws.com:86")
            {
                Authenticator=new JwtAuthenticator(accessToken)
            };           
            
            this.client=new RestClient();
        } 

        private string GetJwtToken (string username, string password)
        {
            var tempClient = new RestClient("http://softuni-qa-loadbalancer-2137572849.eu-north-1.elb.amazonaws.com:86");

            var request = new RestRequest("/api/User/Authentication", Method.Post);
            request.AddJsonBody(new
            {
                userName=username,
                password=password
            });

            var response = tempClient.Execute(request);

            if(response.IsSuccessStatusCode)
            {
                var content = JsonSerializer.Deserialize<JsonElement>(response.Content);
                var accessToken = content.GetProperty("accessToken").GetString();

                return (accessToken);
            }
            else
            {
                throw new InvalidOperationException("AccessToken is empty or null");
            }
            
        }
        [Order (1)]
        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}