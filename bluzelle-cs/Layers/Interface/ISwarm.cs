using System.Threading.Tasks;

namespace bluzelle_cs
{
    public interface ISwarm
    {
        Task<bool> CreateDB();
        Task<bool> Create(string key, string value);
        Task<string> Read(string key);

        Task<bool> Update(string key, string value);
        Task<bool> Delete(string key);
    }
}
