using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Moq;
using TreasureHuntEndpoints;
using System.Data.SqlClient;
using TreasureHuntEndpoints.Models;
using System.Threading.Tasks;
using System.Net.Http;
using System;
using System.Text;
using Microsoft.Extensions.Logging;
using System.Net;

namespace TreasureHunt.Tests
{
    [TestClass]
    public class GetUsersTests
    {
        private List<UserMo> Users { get; set; }

        [TestMethod]
        public async Task GivenRequestToGetUsers_WhenGetUsersExecuted_ThenReturnAllUsers()
        {
            //set up expected values
            Users = new List<UserMo>(){
                new UserMo(){UserId = "1", HashedPassword = "12345", Username = "jmalbert7"},
                new UserMo(){UserId = "2", HashedPassword = "12346", Username = "logan"},
                new UserMo(){UserId = "3", HashedPassword = "12346", Username = "lewis"}
            };
            //create mock for readResultFromRequest, returns expected values
            var azureMock = new Mock<IAzureService>();
            azureMock
                .Setup(mock => mock.readResultFromRequest())
                .ReturnsAsync(() => (Users));
            //create http message to hit endpoint
            var reqHelper = new RequestHelpers();
            var request = reqHelper.GenerateRequest(IRequestHelpers.HttpRequestTypes.get, "users/");
            //run http req, set results to actual
            var actual = await GetUsers.Run(
                request,
                Mock.Of<ILogger>(),
                azureMock);
            //assert reponse type is 200
            Assert.AreEqual(HttpStatusCode.OK, actual.StatusCode);
        }
    }

    public interface IRequestHelpers
    {
        public enum HttpRequestTypes
        {
            get = 1,
            post = 2,
            delete = 3
        }
        HttpRequestMessage GenerateRequest(HttpRequestTypes verb, string route);
    }
    public class RequestHelpers : IRequestHelpers
    {
        public string BaseAddress = "http://localhost/api/";
        public HttpRequestMessage GenerateRequest(IRequestHelpers.HttpRequestTypes verb, string route)
        {
            HttpMethod method;
            switch (verb)
            {
                case IRequestHelpers.HttpRequestTypes.get:
                    method = HttpMethod.Get;
                    break;
                case IRequestHelpers.HttpRequestTypes.post:
                    method = HttpMethod.Post;
                    break;
                case IRequestHelpers.HttpRequestTypes.delete:
                    method = HttpMethod.Delete;
                    break;
                default:
                    throw new Exception("Invalid Method Type");
            }
            var request = new HttpRequestMessage
            {
                Method = method,
                RequestUri = new Uri(new StringBuilder($"{BaseAddress}{route}").ToString())
            };
            return request;
        }
    }
}
