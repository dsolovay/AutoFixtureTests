using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using FluentAssertions;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Kernel;
using Xunit;
using Xunit.Sdk;

namespace AutoFixtureTests
{
    public class AutoFixtureTests
    {
        Fixture fixture = new Fixture();

        [Fact]
        public void SimpleTests()
        {
            int i = fixture.Create<int>();
            i.Should().NotBe(0, "returns sample values");
            int j = fixture.Create<int>();
            i.Should().NotBe(j, "doesn't repeat");
            i.Should().BeLessOrEqualTo(256, "starts with small numbers");
        }

        [Fact]
        public void StringTests()
        {
            string s = fixture.Create<string>();
            Guid g;
            Guid.TryParse(s, out g).Should().BeTrue("anonymous strings are guids");

            s = fixture.Create<string>("SomeName");
            s.Should().StartWith("SomeName");
            var end = s.Substring("SomeName".Length);
            Guid.TryParse(end, out g).Should().BeTrue("named strings end with Guids");
        }

        [Fact]
        public void ClassPropertyNames()
        {
            SampleClass1 s = fixture.Create<SampleClass1>();
            s.SomeProperty.Should().StartWith("SomeProperty");
            s.SomeEnumeration.Count().Should().Be(3);
            s.SampleStringField.Should().StartWith("sampleStringField", "uses constructor property name");
        }

        [Fact]
        public void CustomizeInts()
        {
            ISpecimenBuilder builder = new IntBuilder();
            fixture.Customizations.Add(builder);
            fixture.Create<int>().Should().Be(10);
            fixture.Create<int>().Should().Be(9);
            SampleClass1 s = fixture.Create<SampleClass1>();

        }

        [Fact]
        public void UseCustomization()
        {
            ICustomization customization = new MyCustomization();
            fixture.Customize(customization);
            fixture.Create<int>().Should().Be(10);
            fixture.Create<int>().Should().Be(9);
            fixture.Create<string>().Should().Be("hello");
        }

        [Fact]
        public void ContextualCustomization()
        {
            // create two specimen builders.  One is a third-party one. The other is a sting builder.
            // String builder should set to "inner" if called from inside third party customization.



            IFixture fixture = new Fixture();
            ISpecimenBuilder thirdPartyCustomization = new ThirdPartyCustomization();
           // fixture.Customizations.Add(thirdPartyCustomization);
            ISpecimenBuilder localCustomization = new LocalCustomization();
            fixture.Customizations.Add(localCustomization);

            var foo = fixture.Create<Foo>();
            foo.StringProperty1.Should().Be("inner");
            fixture.Create<string>().Should().NotBe("inner");
        }
    }

    public class Foo
    {
        public string StringProperty1 { get; set; }

    }

    // This doesn't work becaue it is not resolving the string
    public class LocalCustomization
        : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            if (typeof(Foo).Equals(request))
            {
                var foo = new Foo();
                foo.StringProperty1 = "inner";
                return foo;

            }

            return new NoSpecimen();
        }
    }

    public class ThirdPartyCustomization : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            var t = request as Type;
            if (typeof(Foo).Equals(t))
            {
                var foo = new Foo();
                foo.StringProperty1 = context.Resolve(typeof(string)) as string;
                return foo;
            }
            else
            {

                return new NoSpecimen();
            }
        }
    }
}