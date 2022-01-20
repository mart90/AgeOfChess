namespace AgeOfChess
{
    class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public double LastElo { get; set; }
        public int? Bid { get; set; }
    }
}
