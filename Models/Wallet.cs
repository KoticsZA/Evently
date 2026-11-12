namespace Evently.Models
{
    public class Wallet
    {
        public int Id { get; set; }
        public decimal Balance { get; set; } = 0.00m;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string UserId { get; set; } = string.Empty;
        public AppUser User { get; set; } = null!;
        public ICollection<WalletTransaction> Transactions { get; set; }= new List<WalletTransaction>();
    }
}