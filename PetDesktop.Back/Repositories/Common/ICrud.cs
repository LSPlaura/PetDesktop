namespace PetDesktop.Back.Repositories.Common;

public interface ICrud<TKey, TValue>
{
    Task<IEnumerable<TValue>> GetAllAsync(int page, int pageSize);
    Task<TValue> CreateAsync(TValue value);
    Task<TValue> DeleteAsync(TValue value);
    Task<TValue> UpdateAsync(TKey key);
    Task<TValue?> GetByIdAsync(TKey key);
    Task<bool> ExistsAsync(TKey key);
}