using Microsoft.Extensions.DependencyInjection;

namespace BlazorBlocks.Tests.Model;

/// <summary>
/// Block registrations are static, so register the default blocks exactly once per test run.
/// </summary>
internal static class TestRegistration
{
    private static readonly Lazy<bool> Registered = new(() =>
    {
        new ServiceCollection().AddBlazorBlocks();
        return true;
    });

    public static void EnsureRegistered() => _ = Registered.Value;
}
