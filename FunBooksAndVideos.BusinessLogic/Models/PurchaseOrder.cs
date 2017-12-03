using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using FunBooksAndVideos.BusinessLogic.Services;
using FunBooksAndVideos.DataAccess.DAOs;
using FunBooksAndVideos.DataAccess.Repositories;

namespace FunBooksAndVideos.BusinessLogic.Models
{
    public class PurchaseOrder : IPurchaseOrder
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IShippingSlipGenerator _shippingSlipGenerator;
        private readonly IMapper _mapper;

        public PurchaseOrder(
            Customer customer, IEnumerable<Product> lineItems, ICustomerRepository customerRepository, 
            IShippingSlipGenerator shippingSlipGenerator, IMapper mapper)
        {
            Customer = customer;
            LineItems = lineItems;

            _customerRepository = customerRepository;
            _shippingSlipGenerator = shippingSlipGenerator;
            _mapper = mapper;
        }

        public Customer Customer { get; }
        public long Id { get; }
        public IEnumerable<Product> LineItems { get; }
        public decimal TotalPrice { get; private set; }

        public long? Process()
        {
            if (LineItems == null || !LineItems.Any())
            {
                throw new ArgumentNullException(nameof(LineItems));
            }

            var memberships = LineItems.Where(_ => _ is Membership);

            foreach (var membership in memberships)
            {
                var membershipDAO = _mapper.Map<MembershipDAO>(membership);
                _customerRepository.ActivateMembership(Customer.Id, membershipDAO);
            }

            var physicalProducts = LineItems.Where(_ => _ is PhysicalProduct).Cast<PhysicalProduct>().ToList();

            if (physicalProducts.Any())
            {
                return _shippingSlipGenerator.Generate(physicalProducts);
            }

            return null;
        }
    }
}