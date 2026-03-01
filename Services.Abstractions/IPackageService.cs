using Shared.SubscriptionModels;

namespace Services.Abstractions
{
    public interface IPackageService
    {
        Task<PackageResponseDTO> CreatePackageAsync(CreatePackageDTO dto, CancellationToken cancellationToken = default);
        Task<PackageResponseDTO> UpdatePackageAsync(Guid packageId, UpdatePackageDTO dto, CancellationToken cancellationToken = default);
        Task DeletePackageAsync(Guid packageId, CancellationToken cancellationToken = default);
        Task<PackageResponseDTO> GetPackageByIdAsync(Guid packageId, CancellationToken cancellationToken = default);
        Task<IEnumerable<PackageResponseDTO>> GetAllPackagesAsync(bool activeOnly = false, CancellationToken cancellationToken = default);
    }
}
