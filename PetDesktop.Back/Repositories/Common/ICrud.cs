namespace PetDesktop.Back.Repositories;

public interface ICrud<TKey, TValue>
{
    IEnumerable<TValue> GetAll(int page, int pageSize);
    TValue Create(TValue value);
    TValue Delete(TValue value);
    TValue Update(TKey key);
    TValue GetById(TKey key);
    bool Exists(TKey key);

}