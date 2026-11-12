using System.Transactions;

namespace Evently.Models
{
    public class WalletTransaction
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
        public TransactionStatus Status { get; set; } = TransactionStatus.Pending;
        public string? Reference { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
        public int WalletId { get; set; }
        public Wallet Wallet { get; set; } = null!;
    }
    public enum TransactionType
    {
        Deposit = 1,
        Withdrawal = 2,
        Purchase = 3,
        Refund = 4,
        Payout = 5
    }
    public enum TransactionStatus
    {
        Pending = 1,
        Completed = 2,
        Failed = 3,
        Cancelled = 4
    }
}