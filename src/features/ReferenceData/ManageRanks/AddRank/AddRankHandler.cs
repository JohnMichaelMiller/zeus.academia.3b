using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update;
using Zeus.Academia.Features.ReferenceData.ManageRanks.Persistence;
using Zeus.Academia.Features.SharedKernel.Foundation.Common;

namespace Zeus.Academia.Features.ReferenceData.ManageRanks.AddRank;

public sealed class AddRankHandler(ManageRanksDbContext dbContext) : IRequestHandler<AddRankCommand, Result<AddRankResponse>>
{
  public async Task<Result<AddRankResponse>> Handle(AddRankCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    if (!RankCatalog.TryNormalizeCode(request.Code, out var normalizedCode))
    {
      return Result<AddRankResponse>.Failure(ManageRanksErrors.InvalidCode);
    }

    var duplicateExists = await dbContext.Ranks
        .AsNoTracking()
        .AnyAsync(rank => rank.Code == normalizedCode, cancellationToken);

    if (duplicateExists)
    {
      return Result<AddRankResponse>.Failure(ManageRanksErrors.DuplicateCode);
    }

    dbContext.Ranks.Add(RankReference.Create(normalizedCode));

    try
    {
      await dbContext.SaveChangesAsync(cancellationToken);
    }
    catch (DbUpdateException exception) when (IsDuplicateCodeViolation(exception))
    {
      return Result<AddRankResponse>.Failure(ManageRanksErrors.DuplicateCode);
    }

    var response = new AddRankResponse(normalizedCode, RankCatalog.ToAccessLevel(normalizedCode));
    return Result<AddRankResponse>.Success(response);
  }

  private static bool IsDuplicateCodeViolation(DbUpdateException exception)
  {
    ArgumentNullException.ThrowIfNull(exception);

    if (exception.InnerException is SqlException sqlException)
    {
      return sqlException.Number is 2601 or 2627;
    }

    var innerException = exception.InnerException;
    if (innerException is null)
    {
      return false;
    }

    if (!string.Equals(innerException.GetType().Name, "SqliteException", StringComparison.Ordinal))
    {
      return false;
    }

    var codeProperty = innerException.GetType().GetProperty("SqliteErrorCode");
    var codeValue = codeProperty?.GetValue(innerException);
    return codeValue is int sqliteCode && sqliteCode == 19;
  }
}
