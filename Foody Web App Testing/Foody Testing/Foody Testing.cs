using Foody_Testing.Models;
using RestSharp;
using RestSharp.Authenticators;
using System.Net;
using System.Text.Json;

namespace Foody_Testing
{
    public class FoodyTesting
    {
        private RestClient client;
        private static string foodId;

        [OneTimeSetUp]
        public void Setup()
        {
            // Get Auth
            string accessToken = GetAccessToken("ivan111", "123456");

            var restOptions = new RestClientOptions("http://softuni-qa-loadbalancer-2137572849.eu-north-1.elb.amazonaws.com:86")
            {
                Authenticator = new JwtAuthenticator(accessToken),
            };

            this.client = new RestClient(restOptions);
        }

        private string GetAccessToken(string username, string password)
        {
            var authClient = new RestClient("http://softuni-qa-loadbalancer-2137572849.eu-north-1.elb.amazonaws.com:86");

            var authRequest = new RestRequest("/api/User/Authentication", Method.Post);
            authRequest.AddJsonBody(
            new 
            {
                UserName = username,
                Password = password
            });

            var response = authClient.Execute(authRequest);
            if (response.IsSuccessStatusCode)
            {
                var content = JsonSerializer.Deserialize<JsonElement>(response.Content);
                var accessToken = content.GetProperty("accessToken").GetString();
                return accessToken;
            }
            else
            {
                throw new InvalidOperationException("Authentication failed");
            }

        }
        [Order(1)]
        [Test]
        public void CreateFood_WithRequiredFields_ShouldSucceed()
        {
            //Arrange
            var newFood = new FoodDTO
            {
                Name = "New Food",
                Description = "New food with caramel",
                Url = ""
            };

            var request = new RestRequest("/api/Food/Create", Method.Post);
           
            request.AddJsonBody(newFood);

            //Act

            var response = client.Execute(request);
            //Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            var content = JsonSerializer.Deserialize<ApiResponseDTO>(response.Content);
            Assert.That(content.Msg, Is.Not.Empty);
            foodId = content.FOODID;
        }

        [Order(2)]
        [Test]

        public void Edit_The_Title_Of_The_Food_Crerated_Should_Return_Edited_Food_Title ()
        {   
            //Arrange
            var request = new RestRequest($"/api/Food/Edit/{foodId}");
            request.AddJsonBody(new[]
            {
                new
                {
                    path= "/name",
                    op = "replace",
                    value="Edited Title"
                }
            });
            //Act
            var response=client.Execute(request, Method.Patch);

            //Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }
        [Order(3)]
        [Test] 
        public void Get_all_Foods_Should_Return_Array_With_Fodds ()
        {
            var request = new RestRequest("/api/Food/All", Method.Get);

            var response = client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var content=JsonSerializer.Deserialize<List<ApiResponseDTO>>(response.Content);
            Assert.That(content, Is.Not.Empty);
        }
        [Order(4)]
        [Test]

        public void Delete_The_Food_Edited_Should_Correctlly_Deleted_the_Food ()
        {
            var request = new RestRequest($"/api/Food/Delete/{foodId}", Method.Delete);

            var response = client.Execute(request);                       
            
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var content = JsonSerializer.Deserialize<ApiResponseDTO>(response.Content);

            Assert.That(content.Msg, Is.EqualTo("Deleted successfully!"));

        }
        [Order(5)]
        [Test]

        public void Try_To_Create_Food_Without_the_Required_Fileds_Should_Return_Bad_Request ()
        {
            //Arrange
            var newFood = new FoodDTO
            {
                
                Description = "New food with caramel",
                Url = ""
            };

            var request = new RestRequest("/api/Food/Create", Method.Post);

            request.AddJsonBody(newFood);

            //Act

            var response = client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        }
        [Order(6)]
        [Test]

        public void Try_To_EDIT_Non_Existing_Food_Should_Return_Not_Found ()
        {
            //Arrange
            var request = new RestRequest($"/api/Food/Edit/ppppppp");
            request.AddJsonBody(new[]
            {
                new
                {
                    path= "/name",
                    op = "replace",
                    value="Edited Title"
                }
            });
            //Act
            var response = client.Execute(request, Method.Patch);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            var content=JsonSerializer.Deserialize<ApiResponseDTO>(response.Content);

            Assert.That(content.Msg, Is.EqualTo("No food revues..."));
        }
        [Order(7)]
        [Test]

        public void Try_To_Delete_Non_Existing_Food ()
        {
            var request = new RestRequest($"/api/Food/Delete/ooooooo", Method.Delete);

            var response = client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

            var content = JsonSerializer.Deserialize<ApiResponseDTO>(response.Content);

            Assert.That(content.Msg, Is.EqualTo("Unable to delete this food revue!"));

        }
    }
}