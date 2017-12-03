using System;
using FunBooksAndVideos.DataAccess.DAOs;

namespace FunBooksAndVideos.DataAccess.Repositories
{
    public class CustomerRepository : Repository<CustomerDAO>, ICustomerRepository
    {
        public override CustomerDAO Get(long id)
        {
            throw new NotImplementedException();
        }

        public void ActivateMembership(long customerId, MembershipDAO membershipProduct)
        {
            throw new NotImplementedException();
        }
    }
}