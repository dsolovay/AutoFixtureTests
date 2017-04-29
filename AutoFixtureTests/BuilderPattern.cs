using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using FluentAssertions;
using FluentAssertions.Equivalency;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Kernel;
using Xunit;
using Xunit.Sdk;

namespace AutoFixtureTests
{
    public class BuilderPattern
    {
        [Fact]
        public void UsingConstructors()
        {
            // order has customer, customer has address
            // new Order(new Customer, new Address)
            Order order = new Order(new Customer(new Address("Some City")));
            order.Customer.Address.City.Should().Be("Some City");
        }

        [Fact]
        public void UsingBuilders()
        {
            // order has customer, customer has address
            // new Order(new Customer, new Address)
            Order order = new OrderBuilder()
                .WithCustomer(
                    new CustomerBuilder()
                        .WithAddress(
                            new AddressBuilder()
                                .WithCity("Some City")
                                .Build()
                        )
                        .Build()
                ).Build();
            order.Customer.Address.City.Should().Be("Some City");
        }

        [Fact]
        public void UsingAutoFixture()
        {
            var fixture = new Fixture();
            fixture.Inject(new Address("Some City"));
            fixture.Create<Order>().Customer.Address.City.Should().Be("Some City");
        }

        [Fact]
        public void UsingRestrictedSpecimenBuilder()
        {
            // Scenario: We want city set to "Some City" only on Address constructor parameters
            var fixture = new Fixture();
            fixture.Customizations.Add(new AddressCityBuilder());

            fixture.Create<Order>().Customer.Address.City.Should().Be("Some City");
            fixture.Create<CityHolder>().City.Should().NotBe("Some City",
                "because the SpecimenBuilder is restricted to the Address class");
        }


        [Fact]
        public void UsingSpecimenBuilderForAllCityConstructorParameters()
        {
            // Scenario: We want city set to "Some City" only on Address constructor parameters
            var fixture = new Fixture();
            fixture.Customizations.Add(new CityParameterBuilder());

            fixture.Create<Order>().Customer.Address.City.Should().Be("Some City");
            fixture.Create<CityHolder>().City.Should().Be("Some City",
                "because the SpecimenBuilder is NOT restricted to the Address class");
        }

        [Fact]
        public void UsingGenericParameterBuilderRestricted()
        {
            // Scenario: We want city set to "Some City" only on Address constructor parameters
            var fixture = new Fixture();
            fixture.Customizations.Add(new ConstructorStringParameterBuilder("city", "Some City", typeof(Address)));

            fixture.Create<Order>().Customer.Address.City.Should().Be("Some City");
            fixture.Create<CityHolder>().City.Should().NotBe("Some City",
                "because the SpecimenBuilder is restricted to the Address class");
        }

        [Fact]
        public void UsingGenericParameterNotRestricted()
        {
            // Scenario: We want city set to "Some City" only on Address constructor parameters
            var fixture = new Fixture();
            fixture.Customizations.Add(new ConstructorStringParameterBuilder("city", "Some City");

            fixture.Create<Order>().Customer.Address.City.Should().Be("Some City");
            fixture.Create<CityHolder>().City.Should().Be("Some City"); 
        }




    }

    #region Specimen Builders
    // Using AutoFixture ISpecimenBuilder
    public class ConstructorStringParameterBuilder : ISpecimenBuilder
    {
        private readonly string _name;
        private readonly string _value;
        private readonly Type _constructedType;

        public ConstructorStringParameterBuilder(string name, string value, Type ConstructedType=null)
        {
            _name = name;
            _value = value;
            _constructedType = ConstructedType; 
             
        }

        public object Create(object request, ISpecimenContext context)
        {
            ParameterInfo pi = request as ParameterInfo;
            if (pi?.ParameterType == typeof(string) &&
               pi.Member.MemberType == MemberTypes.Constructor &&
               pi.Name.ToLower().Contains(_name.ToLower()) &&
               (_constructedType == null || _constructedType.Equals(pi.Member.DeclaringType))) 
            {
                return _value;
            }
            return new NoSpecimen();
        }
    }

    public class CityParameterBuilder : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            ParameterInfo pi = request as ParameterInfo;
            if (pi?.ParameterType == typeof(string) &&
                pi.Member.MemberType == MemberTypes.Constructor &&
                pi.Name.ToLower().Contains("city")
                )
            {
                return "Some City";
            }
            return new NoSpecimen();
        }
    }
    
    public class AddressCityBuilder
        : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            ParameterInfo pi = request as ParameterInfo;
            if (pi?.ParameterType == typeof(string) &&
                pi.Member.MemberType == MemberTypes.Constructor &&
                pi.Member.DeclaringType == typeof(Address))
            {
                return "Some City";
            }
            return new NoSpecimen();
        }
    }
#endregion

    #region Hand Coded Builders

    public class OrderBuilder
    {
        private Customer Customer = new CustomerBuilder().Build();

        public OrderBuilder WithCustomer(Customer customer)
        {
            this.Customer = customer;
            return this;
        }

        public Order Build()
        {
            return new Order(Customer);
        }


    }

    public class AddressBuilder
    {
        private string City = "Anonymous City";

        public AddressBuilder WithCity(string city)
        {
            this.City = city;
            return this;
        }

        public Address Build()
        {
            return new Address(City);
        }
    }

    public class CustomerBuilder
    {
        private Address Address = new AddressBuilder().Build();

        public CustomerBuilder WithAddress(Address address)
        {
            this.Address = address;
            return this;
        }

        public Customer Build()
        {
            return new Customer(Address);
        }

    }

    #endregion

    #region Sample Classe

    public class Order
    {
        public Order(Customer customer)
        {
            Customer = customer;
            
        }

        public Customer Customer { get; }
    }

    public class Customer
    {
        public Customer(Address address)
        {
            Address = address;
        }

        public Address Address { get; }
    }

    public class Address
    {
        public string City { get; }

        public Address(string city)
        {
            City = city;
        }
    }

    public class CityHolder
    {

        public string City { get; }

        public CityHolder(string city)
        {
            this.City = city;
        }
    }
    #endregion

}