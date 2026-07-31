using MediatR;
using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.SharedKernel.Foundation.Common;
using Zeus.Academia.Features.SharedKernel.Foundation.Domain;
using Zeus.Academia.Features.SharedKernel.Foundation.Persistence;

namespace Zeus.Academia.Features.SharedKernel.Foundation.ReferenceData.ManageRanks.AddRank;

public sealed class AddRankCommandHandler(SharedKernelDbContext db)
    : IRequestHandler<AddRankCommand, Result<AddRankCommandResponse>>
{
  public async Task<Result<AddRankCommandResponse>> Handle(AddRankCommand request, CancellationToken cancellationToken)
  {
    if (!RankExtensions.TryParseCode(request.Code, out var rank))
    {
      return Result<AddRankCommandResponse>.Failure(ManageRanksErrors.InvalidCode);
    }

    var normalizedCode = rank.ToCode();

    var exists = await db.Ranks.AnyAsync(x => x.Code == normalizedCode, cancellationToken);
    if (exists)
    {
      return Result<AddRankCommandResponse>.Failure(ManageRanksErrors.DuplicateCode(normalizedCode));
    }

    var managedRank = AddRankCommandMappings.ToEntity(rank);
    db.Ranks.Add(managedRank);

    try
    {
      await db.SaveChangesAsync(cancellationToken);
    }
    catch (DbUpdateException)
    {
      return Result<AddRankCommandResponse>.Failure(ManageRanksErrors.DuplicateCode(normalizedCode));
    }

    return Result<AddRankCommandResponse>.Success(AddRankCommandMappings.ToResponse(managedRank));
  }
}
