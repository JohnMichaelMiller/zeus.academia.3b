using MediatR;
using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.ReferenceData.ManageRanks.Shared;
using Zeus.Academia.Features.SharedKernel.Foundation.Common;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;

namespace Zeus.Academia.Features.ReferenceData.ManageRanks.AddRank;

public sealed class AddRankHandler(ManageRanksDbContext dbContext) : IRequestHandler<AddRankCommand, Result<AddRankResponse>>
{
  public async Task<Result<AddRankResponse>> Handle(AddRankCommand request, CancellationToken cancellationToken)
  {
    var normalizedCode = SharedKernelNormalization.NormalizeCode(request.Code, nameof(request.Code), "Rank code");

    if (await dbContext.RankReferences.AsNoTracking().AnyAsync(x => x.Code == normalizedCode, cancellationToken))
    {
      return Result<AddRankResponse>.Failure(ManageRanksErrors.DuplicateCode(normalizedCode));
    }

    var rankReference = RankReference.Create(normalizedCode);

    dbContext.RankReferences.Add(rankReference);

    try
    {
      await dbContext.SaveChangesAsync(cancellationToken);
    }
    catch (DbUpdateException)
    {
      if (await dbContext.RankReferences.AsNoTracking().AnyAsync(x => x.Code == normalizedCode, cancellationToken))
      {
        return Result<AddRankResponse>.Failure(ManageRanksErrors.DuplicateCode(normalizedCode));
      }

      throw;
    }

    return Result<AddRankResponse>.Success(new AddRankResponse(rankReference.Code, rankReference.AccessLevel));
  }
}
