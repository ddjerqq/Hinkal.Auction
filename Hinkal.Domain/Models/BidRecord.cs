using Hinkal.Domain.Services;

namespace Hinkal.Domain.Models;

public sealed class BidRecord
{
    public Guid UserId { get; set; }

    public decimal BidAmount { get; set; }
}

public sealed class BidRecordDto
{
    public string UserId { get; set; } = string.Empty;

    public string BidAmount { get; set; } = string.Empty;

    public static BidRecordDto FromBidRecord(BidRecord record, byte[] key)
    {
        return new BidRecordDto
        {
            UserId = EncryptionService.Encrypt(record.UserId.ToString(), key),
            BidAmount = EncryptionService.Encrypt(record.UserId.ToString(), key),
        };
    }
}