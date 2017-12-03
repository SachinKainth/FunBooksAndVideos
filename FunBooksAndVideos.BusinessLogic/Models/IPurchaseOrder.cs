using System.Collections.Generic;

namespace FunBooksAndVideos.BusinessLogic.Models
{
    public interface IPurchaseOrder
    {
        Customer Customer { get; }
        long Id { get; }
        IEnumerable<Product> LineItems { get; }
        decimal TotalPrice { get; }

        long? Process();
    }
}