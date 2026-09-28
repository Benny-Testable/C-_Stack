using ScholarshipCMGroups.Api.Common;
using ScholarshipCMGroups.Api.Contracts;
using ScholarshipCMGroups.Api.Models;
using ScholarshipCMGroups.Api.Repositories;

namespace ScholarshipCMGroups.Api.Services;

/// <summary>
/// Application logic for managing scholarship programmes.
/// </summary>
public interface IScholarshipService
{
    Task<PagedResult<ScholarshipResponse>> GetPageAsync(
        int? page,
        int? pageSize,
        bool? activeOnly,
        string? searchTerm,
        CancellationToken cancellationToken);

    Task<OperationResult<ScholarshipResponse>> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<OperationResult<ScholarshipResponse>> CreateAsync(ScholarshipRequest request, CancellationToken cancellationToken);

    Task<OperationResult<ScholarshipResponse>> UpdateAsync(
        int id,
        ScholarshipRequest request,
        CancellationToken cancellationToken);

    Task<OperationResult<bool>> DeleteAsync(int id, CancellationToken cancellationToken);
}

/// <inheritdoc cref="IScholarshipService"/>
public sealed class ScholarshipService : IScholarshipService
{
    private readonly IScholarshipRepository _repository;
    private readonly IClock _clock;

    public ScholarshipService(IScholarshipRepository repository, IClock clock)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
    }

    public async Task<PagedResult<ScholarshipResponse>> GetPageAsync(
        int? page,
        int? pageSize,
        bool? activeOnly,
        string? searchTerm,
        CancellationToken cancellationToken)
    {
        var pageRequest = new PageRequest(page, pageSize);
        var (items, totalCount) = await _repository
            .GetPageAsync(pageRequest, activeOnly, searchTerm, cancellationToken)
            .ConfigureAwait(false);

        return new PagedResult<ScholarshipResponse>
        {
            Items = items.Select(s => s.ToResponse()).ToArray(),
            Page = pageRequest.Page,
            PageSize = pageRequest.PageSize,
            TotalCount = totalCount,
        };
    }

    public async Task<OperationResult<ScholarshipResponse>> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var scholarship = await _repository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);

        return scholarship is null
            ? OperationResult.NotFound<ScholarshipResponse>($"Scholarship {id} was not found.")
            : OperationResult.Success(scholarship.ToResponse());
    }

    public async Task<OperationResult<ScholarshipResponse>> CreateAsync(
        ScholarshipRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (await _repository.NameExistsAsync(request.Name, null, cancellationToken).ConfigureAwait(false))
        {
            return OperationResult.Conflict<ScholarshipResponse>(
                $"A scholarship named '{request.Name}' already exists.");
        }

        var scholarship = new Scholarship { CreatedAtUtc = _clock.UtcNow };
        Apply(request, scholarship);

        await _repository.AddAsync(scholarship, cancellationToken).ConfigureAwait(false);
        await _repository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return OperationResult.Success(scholarship.ToResponse());
    }

    public async Task<OperationResult<ScholarshipResponse>> UpdateAsync(
        int id,
        ScholarshipRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var scholarship = await _repository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (scholarship is null)
        {
            return OperationResult.NotFound<ScholarshipResponse>($"Scholarship {id} was not found.");
        }

        if (await _repository.NameExistsAsync(request.Name, id, cancellationToken).ConfigureAwait(false))
        {
            return OperationResult.Conflict<ScholarshipResponse>(
                $"A scholarship named '{request.Name}' already exists.");
        }

        Apply(request, scholarship);
        scholarship.UpdatedAtUtc = _clock.UtcNow;

        _repository.Update(scholarship);
        await _repository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return OperationResult.Success(scholarship.ToResponse());
    }

    public async Task<OperationResult<bool>> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var scholarship = await _repository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (scholarship is null)
        {
            return OperationResult.NotFound<bool>($"Scholarship {id} was not found.");
        }

        _repository.Remove(scholarship);
        await _repository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return OperationResult.Success(true);
    }

    private static void Apply(ScholarshipRequest request, Scholarship target)
    {
        target.Name = request.Name.Trim();
        target.Description = request.Description?.Trim();
        target.SponsorName = request.SponsorName.Trim();
        target.AwardAmount = request.AwardAmount;
        target.TotalSlots = request.TotalSlots;
        target.ApplicationOpensOn = request.ApplicationOpensOn;
        target.ApplicationClosesOn = request.ApplicationClosesOn;
        target.IsActive = request.IsActive;
    }
}
