using System.Collections.Generic;
using FunBooksAndVideos.BusinessLogic.Models;

namespace FunBooksAndVideos.BusinessLogic.Services
{
    public interface IShippingSlipGenerator
    {
        long? Generate(IEnumerable<PhysicalProduct> physicalProducts);
    }
}