using System.Collections.Generic;
using System.Threading.Tasks;
using APP_Proyec.Models;

namespace APP_Proyec.Services
{
    public interface IApiService
    {
        Task<IEnumerable<Product>> GetProductsAsync();
        Task<IEnumerable<Product>> GetTopSellingAsync();
        Task<IEnumerable<Order>> GetOrdersAsync();
        Task<IEnumerable<Account>> GetPendingAccountsAsync();
    }
}
