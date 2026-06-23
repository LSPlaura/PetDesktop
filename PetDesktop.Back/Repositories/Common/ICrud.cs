namespace PetDesktop.Back.Repositories;

public interface ICrud<TKey, TValue>
{
    TValue Create(TValue value);
    TValue Delete(TKey key);
    TValue Update(TKey key);
    TValue GetById(TKey key);

}