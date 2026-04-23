using ConnectDB.Dtos;
using ConnectDB.Helpers;
using ConnectDB.Models;
using ConnectDB.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ConnectDB.Services
{
    public class DealerService : IDealerService
    {
        private readonly IDealerRepository _repo;

        public DealerService(IDealerRepository repo)
        {
            _repo = repo;
        }

        public async Task<PagedResult<DealerDto>> GetAsync(PagedQuery query)
        {
            var q = _repo.Query();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var s = query.Search.Trim();
                q = q.Where(d =>
                    d.DealerCode.Contains(s) ||
                    d.DealerName.Contains(s) ||
                    d.ContactPerson.Contains(s) ||
                    d.PhoneNumber.Contains(s));
            }

            q = ApplySort(q, query.SortBy, query.SortDir);

            var total = await q.CountAsync();
            var items = await q
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(d => new DealerDto
                {
                    Id = d.Id,
                    DealerCode = d.DealerCode,
                    DealerName = d.DealerName,
                    DealerType = d.DealerType,
                    ContactPerson = d.ContactPerson,
                    PhoneNumber = d.PhoneNumber,
                    Email = d.Email,
                    Address = d.Address,
                    Description = d.Description
                })
                .ToListAsync();

            return new PagedResult<DealerDto>
            {
                Page = query.Page,
                PageSize = query.PageSize,
                Total = total,
                Items = items
            };
        }

        public async Task<DealerDto?> GetByIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity == null ? null : ToDto(entity);
        }

        public async Task<(DealerDto? dto, string? error)> CreateAsync(DealerCreateDto dto)
        {
            if (await _repo.ExistsCodeAsync(dto.DealerCode))
            {
                return (null, "DealerCode already exists.");
            }

            var dealerType = NormalizeDealerType(dto.DealerType);
            if (dealerType == null)
            {
                return (null, "DealerType must be Import, Export, or Both.");
            }

            var entity = new Dealer
            {
                DealerCode = dto.DealerCode,
                DealerName = dto.DealerName,
                DealerType = dealerType,
                ContactPerson = dto.ContactPerson,
                PhoneNumber = dto.PhoneNumber,
                Email = dto.Email,
                Address = dto.Address,
                Description = dto.Description
            };

            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();

            return (ToDto(entity), null);
        }

        public async Task<string?> UpdateAsync(int id, DealerUpdateDto dto)
        {
            if (id != dto.Id) return "Id mismatch.";
            if (await _repo.ExistsCodeAsync(dto.DealerCode, dto.Id))
            {
                return "DealerCode already exists.";
            }

            var dealerType = NormalizeDealerType(dto.DealerType);
            if (dealerType == null)
            {
                return "DealerType must be Import, Export, or Both.";
            }

            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return "Not found.";

            entity.DealerCode = dto.DealerCode;
            entity.DealerName = dto.DealerName;
            entity.DealerType = dealerType;
            entity.ContactPerson = dto.ContactPerson;
            entity.PhoneNumber = dto.PhoneNumber;
            entity.Email = dto.Email;
            entity.Address = dto.Address;
            entity.Description = dto.Description;

            _repo.Update(entity);
            await _repo.SaveChangesAsync();
            return null;
        }

        public async Task<string?> DeleteAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return "Not found.";

            if (await _repo.HasTransactionsAsync(id))
            {
                return "Dealer has import/export history and cannot be deleted.";
            }

            _repo.Remove(entity);
            await _repo.SaveChangesAsync();
            return null;
        }

        private static DealerDto ToDto(Dealer dealer)
        {
            return new DealerDto
            {
                Id = dealer.Id,
                DealerCode = dealer.DealerCode,
                DealerName = dealer.DealerName,
                DealerType = dealer.DealerType,
                ContactPerson = dealer.ContactPerson,
                PhoneNumber = dealer.PhoneNumber,
                Email = dealer.Email,
                Address = dealer.Address,
                Description = dealer.Description
            };
        }

        private static string? NormalizeDealerType(string? value)
        {
            return (value ?? string.Empty).Trim().ToLowerInvariant() switch
            {
                "import" => "Import",
                "export" => "Export",
                "both" => "Both",
                _ => null
            };
        }

        private static IQueryable<Dealer> ApplySort(IQueryable<Dealer> query, string? sortBy, string? sortDir)
        {
            var desc = string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase);
            return (sortBy ?? string.Empty).ToLowerInvariant() switch
            {
                "code" => desc ? query.OrderByDescending(d => d.DealerCode) : query.OrderBy(d => d.DealerCode),
                "name" => desc ? query.OrderByDescending(d => d.DealerName) : query.OrderBy(d => d.DealerName),
                "type" => desc ? query.OrderByDescending(d => d.DealerType) : query.OrderBy(d => d.DealerType),
                _ => desc ? query.OrderByDescending(d => d.Id) : query.OrderBy(d => d.Id)
            };
        }
    }
}
