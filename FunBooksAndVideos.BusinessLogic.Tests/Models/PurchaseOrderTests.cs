using System;
using System.Collections.Generic;
using AutoMapper;
using FluentAssertions;
using FunBooksAndVideos.BusinessLogic.Models;
using FunBooksAndVideos.BusinessLogic.Services;
using FunBooksAndVideos.DataAccess.DAOs;
using FunBooksAndVideos.DataAccess.Repositories;
using Moq;
using Moq.Sequences;
using NUnit.Framework;

namespace FunBooksAndVideos.BusinessLogic.Tests.Models
{
    [TestFixture]
    class PurchaseOrderTests
    {
        private PurchaseOrder _purchaseOrder;
        private Customer _customer;
        private IEnumerable<Product> _lineItems;
        private Mock<ICustomerRepository> _customerRepositoryMock;
        private Mock<IShippingSlipGenerator> _shippingSlipGeneratorMock;
        private Mock<IMapper> _mapperMock;

        [SetUp]
        public void Setup()
        {
            _customer = new Customer();
            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _shippingSlipGeneratorMock = new Mock<IShippingSlipGenerator>();
            _shippingSlipGeneratorMock.Setup(_ => _.Generate(It.IsAny<IEnumerable<PhysicalProduct>>())).Returns(42);
            _mapperMock = new Mock<IMapper>();
            _mapperMock
                .Setup(_ => _.Map<MembershipDAO>(It.IsAny<object>()))
                .Returns((object arg) => {
                    switch (arg)
                    {
                        case BookMembership _:
                            return new BookMembershipDAO();
                        case VideoMembership _:
                            return new VideoMembershipDAO();
                        case PremiumMembership _:
                            return new PremiumMembershipDAO();
                    }
                    return null;
                });
        }

        [Test]
        public void Process_WhenInputLineItemsAreNull_ThrowsException()
        {
            _lineItems = null;

            Action action = () => CallProcess();

            action.ShouldThrow<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: LineItems");
        }
        
        [Test]
        public void Process_WhenInputLineItemsAreEmpty_ThrowsException()
        {
            _lineItems = new List<Product>();

            Action action = () => CallProcess();

            action.ShouldThrow<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: LineItems");
        }

        [Test]
        public void Process_WhenThereAreNoMemberships_DoesntActivateAnyMemberships()
        {
            _lineItems = new List<Product>
            {
                new Book("A Brief History of Time")
            };

            var shippingSlipId = CallProcess();

            _customerRepositoryMock.Verify(_ =>
                _.ActivateMembership(It.IsAny<long>(), It.IsAny<MembershipDAO>()), Times.Never());

            shippingSlipId.Should().Be(42);
        }

        [Test]
        public void Process_WhenThereAreNoPhysicalProducts_DoesntGenerateShippingSlip()
        {
            _lineItems = new List<Product>
            {
                new BookMembership()
            };

            var shippingSlipId = CallProcess();

            _shippingSlipGeneratorMock.Verify(_ =>
                _.Generate(It.IsAny<IEnumerable<PhysicalProduct>>()), Times.Never());

            shippingSlipId.Should().BeNull();
        }

        [Test]
        public void Process_WhenThereAreMemberships_ActivatesMemberships()
        {
            // Having this combination of memberships may not be allowed by subsequent tickts
            _lineItems = new List<Product>
            {
                new BookMembership(),
                new VideoMembership(),
                new PremiumMembership()
            };

            var shippingSlipId = CallProcess();

            _customerRepositoryMock.Verify(_ =>
                _.ActivateMembership(It.IsAny<long>(), It.IsAny<BookMembershipDAO>()), Times.Once);
            _customerRepositoryMock.Verify(_ =>
                _.ActivateMembership(It.IsAny<long>(), It.IsAny<VideoMembershipDAO>()), Times.Once);
            _customerRepositoryMock.Verify(_ =>
                _.ActivateMembership(It.IsAny<long>(), It.IsAny<PremiumMembershipDAO>()), Times.Once);

            shippingSlipId.Should().BeNull();
        }

        [Test]
        public void Process_WhenThereArePhysicalProducts_GeneratesShippingSlip()
        {
            _lineItems = new List<Product>
            {
                new Book("A Brief History of Time"),
                new Video("Gone with the Wind")
            };

            var shippingSlipId = CallProcess();

            _shippingSlipGeneratorMock.Verify(_ =>
                _.Generate(It.IsAny<IEnumerable<PhysicalProduct>>()), Times.Once);

            shippingSlipId.Should().Be(42);
        }

        [Test]
        public void Process_WhenThereArePhysicalProductsAndMemberships_GeneratesShippingSlipAndActivatesMemberships()
        {
            _lineItems = new List<Product>
            {
                new Book("A Brief History of Time"),
                new Video("Gone with the Wind"),
                new PremiumMembership()
            };

            var shippingSlipId = CallProcess();

            _shippingSlipGeneratorMock.Verify(_ =>
                _.Generate(It.IsAny<IEnumerable<PhysicalProduct>>()), Times.Once);
            _customerRepositoryMock.Verify(_ =>
                _.ActivateMembership(It.IsAny<long>(), It.IsAny<MembershipDAO>()), Times.Once());

            shippingSlipId.Should().Be(42);
        }

        [Test]
        public void Process_WhenThereArePhysicalProductsAndMemberships_ActivatesMembershipBeforeGeneratingShippingSlip()
        {
            _lineItems = new List<Product>
            {
                new Book("A Brief History of Time"),
                new Video("Gone with the Wind"),
                new PremiumMembership()
            };

            using (Sequence.Create())
            {
                _customerRepositoryMock.Setup(_ => _.ActivateMembership(It.IsAny<long>(), It.IsAny<MembershipDAO>()))
                    .InSequence();
                _shippingSlipGeneratorMock.Setup(_ => _.Generate(It.IsAny<IEnumerable<PhysicalProduct>>()))
                    .InSequence();

                CallProcess();
            }
         
            _shippingSlipGeneratorMock.Verify(_ =>
                _.Generate(It.IsAny<IEnumerable<PhysicalProduct>>()), Times.Once);
            _customerRepositoryMock.Verify(_ =>
                _.ActivateMembership(It.IsAny<long>(), It.IsAny<MembershipDAO>()), Times.Once());
        }

        private long? CallProcess()
        {
            _purchaseOrder = new PurchaseOrder(
                _customer, _lineItems, _customerRepositoryMock.Object, _shippingSlipGeneratorMock.Object, _mapperMock.Object);

            var shippingSlipId = _purchaseOrder.Process();

            return shippingSlipId;
        }
    }
}