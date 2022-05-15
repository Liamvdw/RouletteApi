using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Roulette.Models
{
    public class Bets
    {
        public int Id { get; set; }
        public Guess GuessBet { get; set; }
        public int Amount { get; set; }
        public int TotalAmount { get; set; }
        public int IsWinner { get; set; }
        public int PreviousSpins { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public enum Guess
    {
        Even = 1,
        Odd = 2,
        OneToEighteen = 3,
        NineteenToThirtySix = 4,
        Red = 5,
        Black = 6,
        FirstTwelve = 7,
        SecondTwelve = 8,
        ThirdTwelve = 9
    }

    public class Spin
    {
        public int Roll { get; set; }
        public bool IsEven { get; set; }
        public string Color { get; set; }
    }

}
