using FunBooksAndVideos.DataAccess.DAOs;

namespace FunBooksAndVideos.DataAccess.Repositories
{
    public interface ICustomerRepository
    {
        void ActivateMembership(long customerId, MembershipDAO membershipProduct);
    }
}