using System;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using FluentAssertions;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Kernel;
using Xunit;

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
      SomeClass s = fixture.Create<SomeClass>();
      s.SomeProperty.Should().StartWith("SomeProperty");
      s.SomeEnumeration.Count().Should().Be(3);
      s.Name.Should().StartWith("name", "uses constructor property name");
    }

    [Fact]
    public void CustomizeInts()
    {
      ISpecimenBuilder builder = new IntBuilder();
      fixture.Customize<int>(composer => { return builder; } );
      fixture.Create<int>().Should().Be(10);
      fixture.Create<int>().Should().Be(9);
      SomeClass s = fixture.Create<SomeClass>();
      
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
  }
}