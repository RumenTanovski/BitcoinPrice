namespace TT.Api.Model
{
    public class CoinData
    {
        public int Id { get; set; }
        public string Symbol { get; set; }
        public string Name { get; set; }
        public int Rank { get; set; }
        public decimal Price_usd { get; set; }
        public decimal Percent_change_24h { get; set; }
        public decimal Percent_change_1h { get; set; }
        public decimal Percent_change_7d { get; set; }
        public decimal Price_btc { get; set; }
        public decimal Market_cap_usd { get; set; }
        public decimal Volume24 { get; set; }
        public decimal Volume24a { get; set; }
        public decimal? Csupply { get; set; }
        public decimal? Tsupply { get; set; }
        public decimal? Msupply { get; set; }
    }
}
