namespace AgeOfChess
{
    class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string HashedPassword { get; set; }
        public string LastElo { get; set; }
    }
}
