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
                Authenticator = new JwtAuthenticator(accessToken)
            };

            this.client = new RestClient(options);
        }

        private string GetJwtToken(string username, string password)
        {
            var tempClient = new RestClient("http://softuni-qa-loadbalancer-2137572849.eu-north-1.elb.amazonaws.com:86");

            var request = new RestRequest("/api/User/Authentication", Method.Post);
            request.AddJsonBody(new
            {
                userName = username,
                password = password
            });

            var response = tempClient.Execute(request);

            if (response.IsSuccessStatusCode)
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
        [Order(1)]
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
        [Order(2)]
        [Test]

        public void Edit_The_Title_With_ValidCredantials_Should_Be_Succesfull()
        {
            var request = new RestRequest($"/api/Food/Edit/{foodId}", Method.Patch);

            request.AddJsonBody(new[]
            {
                new
                {
                    path="/name",
                    op="replace",
                    value="Edited value",

                },
            });

            var response = this.client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var content = JsonSerializer.Deserialize<ApiResponseDTO>(response.Content);

            Assert.That(content.Message, Is.EqualTo("Successfully edited"));

        }

        [Order(3)]
        [Test] 
        
        public void Get_All_Foods_Should_Return_Correct_Foods ()
        {    
            //Aarrange
            var request = new RestRequest("/api/Food/All", Method.Get); 

            //Act
            var  response = this.client.Execute(request);

            //Assert 

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var content=JsonSerializer.Deserialize<List<ApiResponseDTO>>(response.Content);

            Assert.IsNotEmpty(content);

        }

        [Order(4)]
        [Test] 

        public void Delete_Food_With_Valid_Id_Should_Deleted_The_Food ()
        {   
            //Arrange
            var request = new RestRequest($"/api/Food/Delete/{foodId}", Method.Delete); 

            //Act
            var response= this.client.Execute(request);

            //Assert

            var content = JsonSerializer.Deserialize<ApiResponseDTO>(response.Content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            Assert.That(content.Message, Is.EqualTo("Deleted successfully!"));

        }

        [Order(5)]
        [Test] 

        public void Create_Food_Without_Required_Fileds_Should_Return_Bad_Request ()
        {
            var newFood = new FoodDTO
            {
                Name = "New Name",
                Description = "",
                Url = "",
            };

            var request = new RestRequest("/api/Food/Create", Method.Post);

            request.AddJsonBody(newFood);

            var response = this.client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Order(6)]
        [Test]

        public void Edit_Non_Existing_Food_Should_Return_Not_Found () 
        {
            var request = new RestRequest($"/api/Food/Edit/{2000}", Method.Patch);

            request.AddJsonBody(new[]
            {
                new
                {
                    path="/name",
                    op="replace",
                    value="Edited value",

                },
            });

            var response = this.client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

            var content = JsonSerializer.Deserialize<ApiResponseDTO>(response.Content);

            Assert.That(content.Message, Is.EqualTo("No food revues..."));
        }

        [Order(7)]
        [Test]

        public void Delete_Non_Existing_Food_Should_Return_Bad_Request ()
        {
            var request = new RestRequest($"/api/Food/Delete{3000}", Method.Delete); 

            var response=this.client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            var content=JsonSerializer.Deserialize<ApiResponseDTO>(response.Content);

            Assert.That(content.Message, Is.EqualTo("Unable to dlete this food revue!"));

        }


    }
}