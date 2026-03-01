using AutoMapper;
using Domain.Contracts;
using Domain.Entities.SubscriptionEntities;
using Domain.Exceptions;
using Services.Abstractions;
using Shared.SubscriptionModels;

namespace Services
{
    public class PackageService : IPackageService
    {
        private readonly IUnitOFWork _unitOfWork;
        private readonly IMapper _mapper;

        public PackageService(IUnitOFWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PackageResponseDTO> CreatePackageAsync(CreatePackageDTO dto, CancellationToken cancellationToken = default)
        {
            var package = _mapper.Map<Package>(dto);
            package.CreatedAt = DateTime.UtcNow;
            package.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.GetRepository<Package, Guid>().AddAsync(package);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<PackageResponseDTO>(package);
        }

        public async Task<PackageResponseDTO> UpdatePackageAsync(Guid packageId, UpdatePackageDTO dto, CancellationToken cancellationToken = default)
        {
            var package = await _unitOfWork.GetRepository<Package, Guid>().GetAsync(packageId);
            if (package == null)
                throw new PackageNotFoundException(packageId);

            _mapper.Map(dto, package);
            package.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.GetRepository<Package, Guid>().Update(package);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<PackageResponseDTO>(package);
        }

        public async Task DeletePackageAsync(Guid packageId, CancellationToken cancellationToken = default)
        {
            var package = await _unitOfWork.GetRepository<Package, Guid>().GetAsync(packageId);
            if (package == null)
                throw new PackageNotFoundException(packageId);

            _unitOfWork.GetRepository<Package, Guid>().Delete(package);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<PackageResponseDTO> GetPackageByIdAsync(Guid packageId, CancellationToken cancellationToken = default)
        {
            var package = await _unitOfWork.GetRepository<Package, Guid>().GetAsync(packageId);
            if (package == null)
                throw new PackageNotFoundException(packageId);

            return _mapper.Map<PackageResponseDTO>(package);
        }

        public async Task<IEnumerable<PackageResponseDTO>> GetAllPackagesAsync(bool activeOnly = false, CancellationToken cancellationToken = default)
        {
            var packages = await _unitOfWork.GetRepository<Package, Guid>().GetAllAsync();

            if (activeOnly)
                packages = packages.Where(p => p.IsActive);

            return _mapper.Map<IEnumerable<PackageResponseDTO>>(packages);
        }
    }
}
