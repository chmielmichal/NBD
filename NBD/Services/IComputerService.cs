using NBD.Models;

namespace NBD.Services
{
    public interface IComputerService
    {
        Task<IEnumerable<ComputerModel>> GetComputers(int? year, string name);
        Task<ComputerModel> GetComputer(string id);
        Task Create(ComputerModel c);
        Task Update(ComputerModel c);
        Task Remove(string id);
        Task<byte[]> GetImage(string id);
        Task StoreImage(string id, Stream imageStream, string imageName);
    }
}