using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Ploeh.AutoFixture;
using Xunit;

namespace AutoFixtureTests
{
    public class FreezeTests
    {
        [Fact]
        public void InjectInt()
        {
            var fixture = new Fixture();
            fixture.Inject(1);

            fixture.Create<int>().Should().Be(1, "because 1 was injected");
            fixture.Create<short>().Should().NotBe(1, "because 1 is not a short");
            fixture.Create<decimal>().Should().NotBe(1, "because 1 is not a short");
        }

        [Fact]
        public void InjectIn()
        {
            var fixture = new Fixture();
            int i = fixture.Freeze<int>();
            fixture.Create<int>().Should().Be(i);
        }

        [Fact]
        public void CanCustomizeSequencesWithoutCustomizingIndividuals()
        {
            var fixture = new Fixture();
            fixture.Register(() => new int[] {1, 2, 3, 5, 8, 13, 21, 34});
            fixture.Create<int>().Should().NotBe(1, "not array");
            fixture.Create<int[]>().First().Should().Be(1);
            fixture.Create<int[]>().Skip(2).First().Should().Be(3);
            fixture.CreateMany<int>().First().Should().NotBe(1);
        }



    }
}
