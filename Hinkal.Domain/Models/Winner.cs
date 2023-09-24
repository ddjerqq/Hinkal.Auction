namespace Hinkal.Domain.Models;

public sealed class Winner
{
    public Guid WinnerId { get; set; }
    public decimal WinnerBid { get; set; }
    public decimal PriceToPay { get; set; }
    public string AuctionKeyBase64 { get; set; } = null!;
}