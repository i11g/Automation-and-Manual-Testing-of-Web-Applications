using FoodyTestProject.Models;
using RestSharp;
using RestSharp.Authenticators;
using System.Net;
using System.Text.Json;


namespace FoodyTestProject
{
    public class FoodyTests
    {
        private RestClient client;
        private static string foodId; 

        [OneTimeSetUp]
        public void Setup()
        {
            string accessToken = GetJwtToken("ivan111", "123456");
                
            var options = new RestClientOptions("http://softuni-qa-loadbalancer-2137572849.eu-north-1.elb.amazonaws.com:86")
            {
                Authenticator=new JwtAuthenticator(accessToken)
            };           
            
            this.client=new RestClient(options);
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

        public void CreateFood_WithRequiredFields_ShouldSucceed()
        {
            // Arrange
            var newFood = new FoodDTO
            {
                Name = "New Test Food",
                Description = "Description",
                Url = "",
            };

            var request = new RestRequest("/api/Food/Create", Method.Post);
            request.AddJsonBody(newFood);

            // Act
            var response = this.client.Execute(request);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

            var data = JsonSerializer.Deserialize<ApiResponseDTO>(response.Content);

            Assert.That(data.FoodId, Is.Not.Empty);

            foodId = data.FoodId;
        }
        [Order (2)]
        [Test]

        public void Edit_The_Title_With_ValidCredantials_Should_Be_Succesfull ()
        {
            var request = new RestRequest($"/api/Food/Edit/ {foodId}");

            request.AddJsonBody(new []
            {
                new
                { 
                    path="name",

                }
            });
            


        }


    }
}