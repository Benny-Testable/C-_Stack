using ScholarshipCMGroups.Api.Common;
using ScholarshipCMGroups.Api.Contracts;
using ScholarshipCMGroups.Api.Models;
using ScholarshipCMGroups.Api.Repositories;

namespace ScholarshipCMGroups.Api.Services;

/// <summary>
/// Application logic for managing applicant profiles.
/// </summary>
public interface IApplicantService
{
    Task<OperationResult<ApplicantResponse>> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<OperationResult<ApplicantResponse>> CreateAsync(ApplicantRequest request, CancellationToken cancellationToken);

    Task<OperationResult<ApplicantResponse>> UpdateAsync(
        int id,
        ApplicantRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Erases an applicant and every record referencing them.
    /// </summary>
    /// <remarks>
    /// Backs the Excel "Data Deletion Verification Rate" metric, whose expected value is 100% of
    /// data removed across all storage layers. Deletion cascades to applications and to the linked
    /// user account through the foreign keys declared in <c>ScholarshipDbContext</c>.
    /// </remarks>
    Task<OperationResult<ErasureReceipt>> EraseAsync(int id, CancellationToken cancellationToken);
}

/// <summary>Confirmation of how many rows an erasure removed, per storage area.</summary>
public record ErasureReceipt
{
    public int ApplicantId { get; init; }

    public int ApplicationsDeleted { get; init; }

    public DateTimeOffset ErasedAtUtc { get; init; }
}

/// <inheritdoc cref="IApplicantService"/>
public sealed class ApplicantService : IApplicantService
{
    private readonly IApplicantRepository _repository;
    private readonly IClock _clock;

    public ApplicantService(IApplicantRepository repository, IClock clock)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
    }

    public async Task<OperationResult<ApplicantResponse>> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var applicant = await _repository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);

        return applicant is null
            ? OperationResult.NotFound<ApplicantResponse>($"Applicant {id} was not found.")
            : OperationResult.Success(applicant.ToResponse());
    }

    public async Task<OperationResult<ApplicantResponse>> CreateAsync(
        ApplicantRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var email = Normalise(request.Email);
        if (await _repository.EmailExistsAsync(email, null, cancellationToken).ConfigureAwait(false))
        {
            return OperationResult.Conflict<ApplicantResponse>("An applicant with that email already exists.");
        }

        var applicant = new Applicant
        {
            FullName = request.FullName.Trim(),
            Email = email,
            InstitutionName = request.InstitutionName?.Trim(),
            CreatedAtUtc = _clock.UtcNow,
        };

        await _repository.AddAsync(applicant, cancellationToken).ConfigureAwait(false);
        await _repository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return OperationResult.Success(applicant.ToResponse());
    }

    public async Task<OperationResult<ApplicantResponse>> UpdateAsync(
        int id,
        ApplicantRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var applicant = await _repository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (applicant is null)
        {
            return OperationResult.NotFound<ApplicantResponse>($"Applicant {id} was not found.");
        }

        var email = Normalise(request.Email);
        if (await _repository.EmailExistsAsync(email, id, cancellationToken).ConfigureAwait(false))
        {
            return OperationResult.Conflict<ApplicantResponse>("An applicant with that email already exists.");
        }

        applicant.FullName = request.FullName.Trim();
        applicant.Email = email;
        applicant.InstitutionName = request.InstitutionName?.Trim();
        applicant.UpdatedAtUtc = _clock.UtcNow;

        _repository.Update(applicant);
        await _repository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return OperationResult.Success(applicant.ToResponse());
    }

    public async Task<OperationResult<ErasureReceipt>> EraseAsync(int id, CancellationToken cancellationToken)
    {
        var applicant = await _repository.GetWithApplicationsAsync(id, cancellationToken).ConfigureAwait(false);
        if (applicant is null)
        {
            return OperationResult.NotFound<ErasureReceipt>($"Applicant {id} was not found.");
        }

        var applicationCount = applicant.Applications.Count;

        _repository.Remove(applicant);
        await _repository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return OperationResult.Success(new ErasureReceipt
        {
            ApplicantId = id,
            ApplicationsDeleted = applicationCount,
            ErasedAtUtc = _clock.UtcNow,
        });
    }

    private static string Normalise(string email) => email.Trim().ToLowerInvariant();
}
