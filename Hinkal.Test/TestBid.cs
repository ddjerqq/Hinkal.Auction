using System.Security.Cryptography;
using Hinkal.Domain.Models;
using Hinkal.Domain.Services;
using Hinkal.Domain.Services.Interfaces;

namespace Hinkal.Test;

public sealed class Tests
{
    public byte[] _auctionKey = null!;

    [SetUp]
    public void Setup()
    {
        // set up AES
        var aes = Aes.Create();
        aes.KeySize = 256;
        aes.GenerateKey();

        _auctionKey = aes.Key;
    }

    private static BidRecord generateRandomBidRecord(int range)
    {
        return new BidRecord
        {
            UserId = Guid.NewGuid(),
            BidAmount = (decimal)(Random.Shared.NextDouble() * range),
        };
    }

    [Test]
    public void Test10kBids()
    {
        IBidRecordStore bids = new BidRecordStore();

        Console.WriteLine("simulating 10k bidders");

        for (int i = 0; i < 10_000; i++)
        {
            var bid = generateRandomBidRecord(100);
            bids.Add(bid);
        }

        // although auction key is not needed, we can pass it anyways.
        var winner = bids.GetWinner(_auctionKey);
        Console.WriteLine(
            $"auction min bid: {bids.MinBy(x => x.BidAmount)!.BidAmount} \n" +
            $"auction max bid: {bids.MaxBy(x => x.BidAmount)!.BidAmount} \n" +
            $"winner is {winner.WinnerId} \n" +
            $"winner bid {winner.WinnerBid} \n" +
            $"winner has to pay {winner.PriceToPay}"
        );
    }

    [Test]
    public void TestRandomWinnerInDraw()
    {
        int winA = 0;
        int winB = 0;

        var userA = Guid.Parse("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA");
        var userB = Guid.Parse("BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB");

        var bidAmount = 200;

        for (int i = 0; i < 10_000; i++)
        {
            IBidRecordStore bids = new BidRecordStore();
            bids.Add(new BidRecord { UserId = userA, BidAmount = bidAmount });
            bids.Add(new BidRecord { UserId = userB, BidAmount = bidAmount });

            var winnerId = bids.GetWinnerRecord().UserId;
            if (winnerId == userA)
            {
                winA++;
            }
            else
            {
                winB++;
            }
        }

        Console.WriteLine(
            "distribution of wins: \n" +
            $"A: {winA / 100}% \n" +
            $"B: {winB / 100}% \n" +
            "this must be close to 50 50"
        );
    }

    [Test]
    public void TestPayoffIncreasesWithVariance()
    {
        {
            // test low variance.
            IBidRecordStore bids = new BidRecordStore();

            bids.Add(new BidRecord {UserId = Guid.NewGuid(), BidAmount = 100});
            bids.Add(new BidRecord {UserId = Guid.NewGuid(), BidAmount = 200});

            var winner = bids.GetWinner(_auctionKey);
            var difference = 200 - winner.PriceToPay;
            Console.WriteLine($"difference for low variance: {difference}");
        }

        {
            // test high variance, give winner 10x the average.
            IBidRecordStore bids = new BidRecordStore();

            bids.Add(new BidRecord {UserId = Guid.NewGuid(), BidAmount = 100});
            bids.Add(new BidRecord {UserId = Guid.NewGuid(), BidAmount = 1000});

            var winner = bids.GetWinner(_auctionKey);
            var difference = 1000 - winner.PriceToPay;
            Console.WriteLine($"difference for high variance: {difference}");
        }
    }
}