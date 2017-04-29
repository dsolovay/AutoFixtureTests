using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace AutoFixtureTests
{
  public class SampleClass1
  {
    public string SampleStringField { get; private set; }

    // AutoFixture can handle constructor properties.
    public SampleClass1(string sampleStringField)
    {
      SampleStringField = sampleStringField;
    }

    public string SomeProperty { get; set; }
    public IEnumerable<string> SomeEnumeration { get; set; }
    public int SomeInt { get; set; }
  }
}
