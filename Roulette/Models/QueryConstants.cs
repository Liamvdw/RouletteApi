using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Roulette.Models
{
    public static class QueryConstants
    {
        public const string SelectAll = "SELECT * FROM PlaceBet";
        public const string CreateTable = "create table PlaceBet (id INTEGER PRIMARY KEY, guessBet varchar(20), amount int, isWinner int, previousSpins int, totalAmount int, createdDate datetime)";
        public const string Insert = "INSERT INTO PlaceBet(guessBet, amount, isWinner, previousSpins, totalAmount, createdDate) VALUES(@guessBet, @amount, @isWinner, @previousSpins, @totalAmount, @createdDate)";
    }
}
