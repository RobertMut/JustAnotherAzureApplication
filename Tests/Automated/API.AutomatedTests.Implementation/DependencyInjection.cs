using BoDi;

namespace API.AutomatedTests.Implementation;

public static class DependencyInjection
{
    public static IObjectContainer AddImplementation(this IObjectContainer objectContainer)
    {


        return objectContainer;
    }
}