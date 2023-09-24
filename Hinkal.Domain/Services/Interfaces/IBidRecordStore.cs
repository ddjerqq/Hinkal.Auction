using Hinkal.Domain.Models;

namespace Hinkal.Domain.Services.Interfaces;

public interface IBidRecordStore : IList<BidRecord>
{
    public IEnumerable<BidRecordDto> GetBidRecordDtos(byte[] key)
        => this.Select(x => BidRecordDto.FromBidRecord(x, key));

    public BidRecord? GetWinnerRecord()
    {
        var winAmount = this
            .Select(x => x.BidAmount)
            .Order()
            .LastOrDefault();

        var winners = this
            .Where(x => x.BidAmount == winAmount)
            .ToList();

        if (winners is { Count: var count and > 1 })
        {
            // primitive random choosing, nothing special needed.
            var winnerIdx = Random.Shared.Next(0, count);
            return winners[winnerIdx];
        }

        return winners.FirstOrDefault() ?? default!;
    }

    public Winner GetWinner(byte[] key)
    {
        var winner = GetWinnerRecord();
        var price = GetMeanPriceToPay();
        var auctionKey = Convert.ToBase64String(key);

        return new Winner
        {
            WinnerId = winner?.UserId ?? default,
            WinnerBid = winner?.BidAmount ?? default,
            PriceToPay = price,
            AuctionKeyBase64 = auctionKey,
        };
    }

    public decimal GetMeanPriceToPay()
    {
        if (!this.Any())
            return 0;

        return this.Average(x => x.BidAmount);
    }
}