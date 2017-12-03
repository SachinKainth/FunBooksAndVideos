using System.Collections.Generic;

namespace FunBooksAndVideos.DataAccess.DAOs
{
    public class CustomerDAO
    {
        public long Id { get; }

        public IEnumerable<MembershipDAO> Memberships { get; }
    }
}