using BlazorBlocks.Blocks.CarouselBlock;
using BlazorBlocks.Blocks.ImageBlock;
using BlazorBlocks.Blocks.QuoteBlock;
using BlazorBlocks.Blocks.RawTextBlock;
using BlazorBlocks.Blocks.TitleBlock;
using BlazorBlocks.Model;
using BlazorBlocks.Model.Registrations;
using BlazorBlocks.Support;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorBlocks;
/// <summary>
/// Service for registering blocks and rows in BlazorBlocks.
/// </summary>
public static class BlockRegistrationService
{
    private static List<BlazorBlocksBlockRegistration> _registeredBlocks { get; } = new List<BlazorBlocksBlockRegistration>();
    private static List<GroupRegistration> _registeredGroups { get; } = new List<GroupRegistration>();

    internal static IReadOnlyList<BlazorBlocksBlockRegistration> RegisteredBlocks => _registeredBlocks;
    internal static IReadOnlyList<GroupRegistration> RegisteredGroups => _registeredGroups;

    /// <summary>
    /// Registers a block.
    /// </summary>
    /// <param name="block">The block to register.</param>
    private static void RegisterBlock(BlazorBlocksBlockRegistration block)
    {
        _registeredBlocks.Add(block);
    }

    /// <summary>
    /// Registers a group.
    /// </summary>
    /// <param name="group">The group to register.</param>
    private static void RegisterGroup(GroupRegistration group)
    {
        _registeredGroups.Add(group);
    }

    /// <summary>
    /// Adds the default blocks and rows to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddBlazorBlocks(this IServiceCollection services)
    {
        AddDefaultBlocks();
        AddDefaultGroups();

        return services;
    }

    /// <summary>
    /// Adds the specified block registrations to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="blockRegistrations">The block registrations to add.</param>
    /// <param name="includeDefaultBlocks">Whether to include the default blocks.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddBlazorBlocks(this IServiceCollection services, List<BlazorBlocksBlockRegistration> blockRegistrations, bool includeDefaultBlocks = false)
    {
        services = RegisterServices(services);

        AddDefaultGroups();

        if (includeDefaultBlocks)
        {
            AddDefaultBlocks();
        }

        foreach (var block in blockRegistrations)
        {
            RegisterBlock(block);
        }


        return services;
    }

    /// <summary>
    /// Adds the specified group registrations to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="groupRegistrations">The group registrations to add.</param>
    /// <param name="includeDefaultGroups">Whether to include the default rows.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddBlazorBlocks(this IServiceCollection services, List<GroupRegistration> groupRegistrations, bool includeDefaultGroups)
    {
        services = RegisterServices(services);

        AddDefaultBlocks();

        if (includeDefaultGroups)
        {
            AddDefaultGroups();
        }

        foreach (var group in groupRegistrations)
        {
            RegisterGroup(group);
        }
        return services;
    }

    /// <summary>
    /// Adds the specified block and group registrations to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="blockRegistrations">The block registrations to add.</param>
    /// <param name="groupRegistrations">The group registrations to add.</param>
    /// <param name="includeDefaults">Whether to include the default blocks and rows.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddBlazorBlocks(this IServiceCollection services, List<BlazorBlocksBlockRegistration> blockRegistrations, List<GroupRegistration> groupRegistrations, bool includeDefaults = true)
    {
        services = RegisterServices(services);

        if (includeDefaults)
        {
            AddDefaultBlocks();
            AddDefaultGroups();
        }

        foreach (var block in blockRegistrations)
        {
            RegisterBlock(block);
        }

        foreach (var group in groupRegistrations)
        {
            RegisterGroup(group);
        }

        return services;
    }

    private static bool IsRegistered<T>(IServiceCollection services)
    {
        return services.Any(x => x.ServiceType == typeof(T));
    }

    private static IServiceCollection RegisterServices(IServiceCollection services)
    {
        if(!IsRegistered<DragService>(services))
        {
            services.AddScoped<DragService>();
        }
        return services;
    }

    /// <summary>
    /// Adds the default rows to the service.
    /// </summary>
    private static void AddDefaultGroups()
    {
        var colRegs = new ColumnRegistration[13];
        for (int i = 1; i < 13; i++)
        {
            colRegs[i] = new ColumnRegistration($"col-{i}");
        }
        
        // Add default column definitions
        RegisterGroup(new GroupRegistration("1 column", "row",
            [ colRegs[12] ] ));
        RegisterGroup(new GroupRegistration("2 columns", "row",
            [ colRegs[6], colRegs[6] ]));
        RegisterGroup(new GroupRegistration("3 columns", "row",
            [ colRegs[4], colRegs[4], colRegs[4] ]));
        RegisterGroup(new GroupRegistration("4 columns", "row",
            [ colRegs[3], colRegs[3], colRegs[3], colRegs[3] ]));
        RegisterGroup(new GroupRegistration("1+2 columns", "row",
            [colRegs[4], colRegs[8]]));
        RegisterGroup(new GroupRegistration("1+2+full columns", "row",
        [
            colRegs[4], colRegs[8],
            colRegs[12]
        ]));

    }

    /// <summary>
    /// Adds the default blocks to the service.
    /// </summary>
    private static void AddDefaultBlocks()
    {
        // Add default block types
        RegisterBlock(new BlazorBlocksBlockRegistration("Quote", null, typeof(QuoteBlockModel), typeof(QuoteBlockEditor)));
        RegisterBlock(new BlazorBlocksBlockRegistration("Carousel", null, typeof(CarouselBlockModel), typeof(CarouselBlockEditor)));
        RegisterBlock(new BlazorBlocksBlockRegistration("Title", null, typeof(TitleBlockModel), typeof(TitleEditorBlock)));
        RegisterBlock(new BlazorBlocksBlockRegistration("Image", null, typeof(ImageBlockModel), typeof(ImageEditorBlock)));
        RegisterBlock(new BlazorBlocksBlockRegistration("Raw text", "https://icon-sets.iconify.design/logo-iconify.svg", typeof(RawTextBlockModel), typeof(RawTextEditorBlock)));
    }
}
