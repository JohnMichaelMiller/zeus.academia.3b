using MediatR;
using Microsoft.EntityFrameworkCore;
using Zeus.Academia.Features.ReferenceData.ManageRanks.Shared;

namespace Zeus.Academia.Features.ReferenceData.ManageRanks.AddRank;

public sealed class AddRankHandler : IRequestHandler<AddRankCommand, AddRankResponse>
{
  private readonly ManageRanksDbContext _dbContext;

  public AddRankHandler(ManageRanksDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  public async Task<AddRankResponse> Handle(AddRankCommand request, CancellationToken cancellationToken)
  {
    var rankRecord = request.ToRankRecord();

    var codeAlreadyExists = await _dbContext.Ranks
      .AsNoTracking()
      .AnyAsync(x => x.Code == rankRecord.Code, cancellationToken);

    if (codeAlreadyExists)
    {
      throw new RankConflictException($"Rank code '{rankRecord.Code}' already exists.");
    }

    _dbContext.Ranks.Add(rankRecord);

    try
    {
      await _dbContext.SaveChangesAsync(cancellationToken);
    }
    catch (DbUpdateException)
    {
      var nowExists = await _dbContext.Ranks
        .AsNoTracking()
        .AnyAsync(x => x.Code == rankRecord.Code, cancellationToken);

      if (nowExists)
      {
        throw new RankConflictException($"Rank code '{rankRecord.Code}' already exists.");
      }

      throw;
    }

    return rankRecord.ToResponse();
  }
}
