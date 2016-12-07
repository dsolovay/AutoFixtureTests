using Ploeh.AutoFixture;

namespace AutoFixtureTests
{

  /// <summary>
  /// Customizations let you package up rules, so you can add a 
  /// bunch at once. This one bundles the IntBuilder with a 
  /// rule that all strings should say "hello".
  /// </summary>
  public class MyCustomization:ICustomization
  {
    public void Customize(IFixture fixture)
    {
      var intBuilder = new IntBuilder();
      fixture.Customizations.Add(intBuilder); 
      fixture.Inject("hello");
    }

  }
}