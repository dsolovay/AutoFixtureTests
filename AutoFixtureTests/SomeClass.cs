using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace AutoFixtureTests
{
  public class SomeClass
  {
    public string Name { get; private set; }

    // AutoFixture can handle constructor properties.
    public SomeClass(string name)
    {
      Name = name;
    }

    public string SomeProperty { get; set; }
    public IEnumerable<string> SomeEnumeration { get; set; }
    public int SomeInt { get; set; }
  }
}
