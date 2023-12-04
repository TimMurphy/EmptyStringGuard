using NUnit.Framework;
using NUnit.Framework.Legacy;

[TestFixture]
public class ErrorsTests
{
    [Test]
    public void ErrorsForAbstract()
    {
        ClassicAssert.Contains("Method 'System.Void ClassWithBadAttributes::MethodWithEmptyStringCheckOnParam(System.String)' is abstract but has an [AllowEmptyAttribute]. Remove this attribute.", AssemblyWeaver.Errors);
        ClassicAssert.Contains("Method 'System.Void InterfaceBadAttributes::MethodWithEmptyStringCheckOnParam(System.String)' is abstract but has an [AllowEmptyAttribute]. Remove this attribute.", AssemblyWeaver.Errors);
    }
}