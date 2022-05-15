using Roulette.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Roulette.Providers.Interface
{
    public interface IRoulette
    {
        void PlaceBet(Bets model);
        List<Bets> GetAllBets();
        Spin Spin();
        List<int> ShowPreviousSpins();
    }
}
