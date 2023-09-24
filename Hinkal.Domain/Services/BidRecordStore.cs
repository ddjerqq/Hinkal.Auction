using Hinkal.Domain.Models;
using Hinkal.Domain.Services.Interfaces;

namespace Hinkal.Domain.Services;

public sealed class BidRecordStore : List<BidRecord>, IBidRecordStore
{
}