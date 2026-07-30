using MediatR;
using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.ReferenceData.ManageRanks.Persistence;
using Zeus.Academia.Features.ReferenceData.ManageRanks.Shared;
using Zeus.Academia.Features.SharedKernel.Foundation.Common;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;

namespace Zeus.Academia.Features.ReferenceData.ManageRanks.AddRank;

public sealed class AddRankHandler(ManageRanksDbContext dbContext)
    : IRequestHandler<AddRankCommand, Result<AddRankResponse>>
{
  public async Task<Result<AddRankResponse>> Handle(AddRankCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    if (!AddRankMappings.TryMapCodeToRank(request.Code, out var rank))
    {
      return Result<AddRankResponse>.Failure(ManageRanksErrors.InvalidCode);
    }

    if (await RankExistsAsync(rank, cancellationToken))
    {
      return Result<AddRankResponse>.Failure(ManageRanksErrors.DuplicateCode);
    }

    var managedRank = request.ToManagedRank();
    dbContext.Ranks.Add(managedRank);

    try
    {
      await dbContext.SaveChangesAsync(cancellationToken);
    }
    catch (DbUpdateException)
    {
      if (await RankExistsAsync(rank, cancellationToken))
      {
        return Result<AddRankResponse>.Failure(ManageRanksErrors.DuplicateCode);
      }

      throw;
    }

    return Result<AddRankResponse>.Success(managedRank.ToResponse());
  }

  private Task<bool> RankExistsAsync(Rank rank, CancellationToken cancellationToken)
  {
    return dbContext.Ranks.AsNoTracking().AnyAsync(x => x.Rank == rank, cancellationToken);
  }
}
