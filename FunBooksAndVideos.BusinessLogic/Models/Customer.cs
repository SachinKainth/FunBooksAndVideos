using System.Collections.Generic;

namespace FunBooksAndVideos.BusinessLogic.Models
{
    public class Customer
    {
        public long Id { get; }

        public IEnumerable<Membership> Memberships { get; }
    }
}