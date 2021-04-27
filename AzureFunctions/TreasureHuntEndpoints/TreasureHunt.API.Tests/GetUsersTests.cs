using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using TreasureHunt.API;

namespace TreasureHunt.API.Tests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class GetUsersTests
    {

        private List<string> Users { get; set; }

        [TestMethod]
        public void GivenRequestToGetUsers_WhenGetUsersExecuted_ThenReturnAllUsers()
        {
            //set up expected values
            Users = new List<string>(){
                "jmalbert7",
                "louis",
                "logan"
            };
            //create mock for readResultFromRequest, returns expected values
            //var mock = new Mock<>();
            //create http message to hit endpoint
            //run http req, set results to actual
            //assert reponse type is 200
        }
    }
}
