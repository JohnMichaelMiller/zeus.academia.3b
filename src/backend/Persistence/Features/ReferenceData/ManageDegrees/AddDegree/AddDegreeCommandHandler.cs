using MediatR;
using Microsoft.EntityFrameworkCore;
using Zeus.Academia.SharedKernel.Domain.Entities;
using Zeus.Academia.SharedKernel.Domain.Exceptions;

namespace Zeus.Academia.Persistence.Features.ReferenceData.ManageDegrees.AddDegree;

/// <summary>
/// Persists canonical degree reference data and rejects duplicates.
/// </summary>
public sealed class AddDegreeCommandHandler(AppDbContext dbContext)
    : IRequestHandler<AddDegreeCommand, AddDegreeResponse>
{
    public async Task<AddDegreeResponse> Handle(AddDegreeCommand request, CancellationToken cancellationToken)
    {
        string normalizedCode = DegreeCatalogEntry.Normalize(request.Code);

        bool exists = await dbContext.Degrees
            .AsNoTracking()
            .AnyAsync(d => d.Code == normalizedCode, cancellationToken);

        if (exists)
            throw new ConflictException($"Degree code '{normalizedCode}' already exists.");

        DegreeCatalogEntry entry = DegreeCatalogEntry.Create(normalizedCode);
        dbContext.Degrees.Add(entry);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new AddDegreeResponse(entry.Code);
    }
}
