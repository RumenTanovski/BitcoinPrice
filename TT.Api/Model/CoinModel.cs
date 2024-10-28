using System;
using System.Collections.Generic;
namespace TT.Api.Model
{
    public class CoinModel
    {
        public decimal NumCoin { get; set; } // Current value
        public string Name { get; set; } // ETH, BTC, etc.
        public decimal InitialPriceCoin { get; set; } // Initial buy price        
        public decimal NewPriceCoin { get; set; } 
    }
}
