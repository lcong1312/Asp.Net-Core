using System.Collections.Generic;
using System.Threading.Tasks;

namespace webbanhangmvc.Casso
{
    public interface ICassoService
    {
        Task<List<Transaction>> GetRecentTransactionsAsync(string fromDate, string toDate);
        Task<Transaction> GetTransactionByIdAsync(int id);
    }
}
