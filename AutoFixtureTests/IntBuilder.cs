using System;
using Ploeh.AutoFixture.Kernel;

namespace AutoFixtureTests
{
  /// <summary>
  /// Simple rule for ints.  Start with 10, then go down from
  /// there.
  /// 
  /// </summary>
  public class IntBuilder : ISpecimenBuilder
  {
    private int current = 10;


    public object Create(object request, ISpecimenContext context)
    {
      // AutoFixture sends propertyInfo, parameterInfo and types.
      Type t = request as Type;

      // Only handle requests for ints!
      if (t == null || t != typeof(int)) return new NoSpecimen();

      return current--;
    }
  }
}