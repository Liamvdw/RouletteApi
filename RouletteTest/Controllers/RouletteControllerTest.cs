using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Roulette.Controllers;
using Roulette.Models;
using Roulette.Providers;
using Roulette.Providers.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouletteTest.Controllers
{
    [TestClass]
    public class RouletteControllerTest
    {
        private Repository repository = new Repository();
        private readonly ILogger<Bets> logger;

        //Test seems to not get data from extising database.
        //As a workaround, delete the existing database and run the Tests.

        [TestMethod]
        public void GetAllBetsTest()
        {
            //arrange
            var controller = new RouletteController(repository, logger);

            //act            
            var result = controller.GetAllBets();

            //assert
            Assert.IsNotNull(result);
        }        

        [TestMethod]
        public void PlaceBetTest()
        {
            //arrange
            var controller = new RouletteController(repository, logger);
            var bets = new Bets
            {
                GuessBet = Guess.FirstTwelve,
                Amount = 200, 
                TotalAmount = 1000,                
            };

            //act
            controller.PlaceBet(bets);
            var result = controller.GetAllBets();

            //assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void SpinTest()
        {
            //arrange
            var controller = new RouletteController(repository, logger);

            //act            
            var result = controller.Spin();

            //assert
            Assert.IsNotNull(result);            
        }

        [TestMethod]
        public void ShowPreviousSpinsTest()
        {
            //arrange
            var controller = new RouletteController(repository, logger);

            //act            
            var result = controller.GetAllBets().Select(s => s.PreviousSpins).ToList();

            //assert
            Assert.IsNotNull(result);
        }
    }
}
