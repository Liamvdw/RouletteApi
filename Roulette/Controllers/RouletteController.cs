using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Roulette.Models;
using Roulette.Providers;
using Roulette.Providers.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Roulette.Controllers
{
    public class RouletteController : Controller, IRoulette
    {
        private IRepository repository;
        private readonly ILogger<Bets> logger;
        public RouletteController(IRepository _repository, ILogger<Bets> _logger)
        {
            repository = _repository;
            logger = _logger;
        }

        [HttpPost]
        [Route("PlaceBet")]
        public void PlaceBet([FromBody] Bets model)
        {
            try
            {
                int previousTotal = 0;
                int isWinner = 1; //Winner
                int isLoser = 0; //Loser
                int multiply = 0;

                if (model.TotalAmount < 100 || model.TotalAmount == 0)
                    throw new Exception("Sorry, the minimum buy in id R100!");

                //Gets the last played bet and subtract the amount from the total.
                var result = GetAllBets();
                if (result.Count > 0)
                {
                    var lastTotalAmount = result.LastOrDefault(x => x.TotalAmount > 0);
                    previousTotal = lastTotalAmount.TotalAmount;
                    previousTotal -= model.Amount;

                    if (lastTotalAmount.TotalAmount < 0)
                        throw new Exception("Sorry, you have to have money to play!");
                }

                //Spin function
                Spin spinResults = Spin();

                //BET: Even, Odd, Red, Black
                if ((model.GuessBet == Guess.Even) && (spinResults.IsEven == true)
                    || ((model.GuessBet == Guess.Odd) && (spinResults.IsEven == false))
                    || ((model.GuessBet == Guess.Red) && (spinResults.Color == "Red".ToLower()))
                    || ((model.GuessBet == Guess.Black) && (spinResults.Color == "Black".ToLower())))
                {
                    SaveBets(model, previousTotal, isWinner, spinResults.Roll, multiply = 2);
                }

                //BET: 1-18
                else if ((model.GuessBet == Guess.OneToEighteen) && ((spinResults.Roll > 0) && (spinResults.Roll < 19)))
                {
                    SaveBets(model, previousTotal, isWinner, spinResults.Roll, multiply = 2);
                }

                //BET: 19-37
                else if ((model.GuessBet == Guess.NineteenToThirtySix) && ((spinResults.Roll > 18) && (spinResults.Roll < 37)))
                {
                    SaveBets(model, previousTotal, isWinner, spinResults.Roll, multiply = 2);
                }

                //BET: FirstTwelve, SecondTwelve, ThirdTwelve
                else if (((model.GuessBet == Guess.FirstTwelve) && (spinResults.Roll > 0 && spinResults.Roll < 13))
                    || ((model.GuessBet == Guess.SecondTwelve) && (spinResults.Roll > 12 && spinResults.Roll < 25))
                    || ((model.GuessBet == Guess.ThirdTwelve) && (spinResults.Roll > 24 && spinResults.Roll < 37)))
                {
                    SaveBets(model, previousTotal, isWinner, spinResults.Roll, multiply = 3);
                }

                //This function gets called when you lose.
                else
                    SaveBets(model, previousTotal, isLoser, spinResults.Roll, multiply = 0);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetAllBets")]
        public List<Bets> GetAllBets()
        {
            var allBets = new List<Bets>();
            try
            {
                var args = new Dictionary<string, object>();

                DataTable dt = repository.Execute(QueryConstants.SelectAll, args);
                string jsonString = string.Empty;
                var response = JsonConvert.SerializeObject(dt);

                //Returns JSON Object
                allBets = JsonConvert.DeserializeObject<List<Bets>>(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
            return allBets;
        }

        public List<Bets> SaveBets(Bets model, int previousTotal, int isWinnerLoser, int roll, int multiply)
        {
            var results = new List<Bets>();
            try
            {
                if (isWinnerLoser == 1)
                {
                    var args = new Dictionary<string, object>
                        {
                             {"@guessBet", model.GuessBet.ToString()},
                             {"@amount", model.Amount},
                             {"@isWinner", isWinnerLoser},
                             {"@previousSpins", roll},
                             {"@totalAmount", previousTotal > 0 ? (previousTotal += (model.Amount * multiply)) : model.TotalAmount += (model.Amount * multiply)},
                             {"@createdDate", DateTime.Now},
                        };

                    if (model.TotalAmount > 0)
                        repository.ExecuteWrite(QueryConstants.Insert, args);
                    else
                        throw new Exception();

                    results = GetAllBets();
                }
                else
                {
                    var args = new Dictionary<string, object>
                        {
                             {"@guessBet", model.GuessBet},
                             {"@amount", model.Amount},
                             {"@isWinner", isWinnerLoser},
                             {"@previousSpins", roll},
                             {"@totalAmount", previousTotal > 0 ? previousTotal : model.TotalAmount -= model.Amount},
                             {"@createdDate", DateTime.Now},
                        };

                    repository.ExecuteWrite(QueryConstants.Insert, args);
                    throw new Exception("Sorry, you've lost! Better luck next time!");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }

            return results;
        }

        public Spin Spin()
        {
            //This will generate a random number between 0-36
            //and get the remainder of the that number and determine if it's even or odd
            //and returns the color for that number that was rolled.
            Random randomNumber = new Random();
            Random randomColor = new Random();
            string[] colors = { "Red", "Black" };
            string color = string.Empty;
            int roll = 0;
            bool isEven = false;

            try
            {
                color = colors[randomColor.Next(colors.Length)];
                roll = randomNumber.Next(0, 36);
                isEven = roll % 2 == 0;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }

            return new Spin() { IsEven = isEven, Roll = roll, Color = color };

        }

        public List<int> ShowPreviousSpins()
        {
            var result = new List<int>();
            try
            {
               result = GetAllBets().Select(s => s.PreviousSpins).ToList();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
            return result;
        }
    }
}
