using BlazorBlocks.Blocks.CarouselBlock;
using BlazorBlocks.Blocks.ImageBlock;
using BlazorBlocks.Blocks.QuoteBlock;
using BlazorBlocks.Blocks.RawTextBlock;
using BlazorBlocks.Blocks.TitleBlock;
using BlazorBlocks.Customization;
using BlazorBlocks.Internals.Support;
using BlazorBlocks.Services.Registrations;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorBlocks;

/// <summary>
/// Service for registering blocks and rows in BlazorBlocks.
/// </summary>
public static class BlockRegistrationService
{
    private static readonly List<BlockRegistration> RegisteredBlocks = [];
    private static readonly List<GroupRegistration> RegisteredGroups = [];

    internal static IReadOnlyList<BlockRegistration> Blocks => RegisteredBlocks;
    internal static IReadOnlyList<GroupRegistration> Groups => RegisteredGroups;

    /// <summary>
    /// Registers a block.
    /// </summary>
    /// <param name="block">The block to register.</param>
    private static void RegisterBlock(BlockRegistration block)
    {
        RegisteredBlocks.Add(block);
    }

    private static void RegisterBlock<TModel, TEditor>(string name, string? image)
        where TModel : BaseBlockModel
        where TEditor : BlockEditor<TModel>
    {
        var blockRegistration = new BlockRegistration<TModel, TEditor>(name, image);
        RegisterBlock(blockRegistration);
    }

    /// <summary>
    /// Registers a group.
    /// </summary>
    /// <param name="group">The group to register.</param>
    private static void RegisterGroup(GroupRegistration group)
    {
        RegisteredGroups.Add(group);
    }

    /// <summary>
    /// Adds the default blocks and rows to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddBlazorBlocks(this IServiceCollection services)
    {
        services = RegisterServices(services);
        AddDefaultBlocks();
        AddDefaultGroups();

        return services;
    }

    /// <summary>
    /// Adds the specified block registrations to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="includeDefaultGroups">Whether to include the default groups.</param>
    /// <param name="includeDefaultBlocks">Whether to include the default blocks.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddBlazorBlocks(this IServiceCollection services, bool includeDefaultGroups = true, bool includeDefaultBlocks = true)
    {
        services = RegisterServices(services);

        if (includeDefaultGroups)
        {
            AddDefaultGroups();
        }

        if (includeDefaultBlocks)
        {
            AddDefaultBlocks();
        }

        return services;
    }

    /// <summary>
    /// Registers a custom block editor for a specific block model type.
    /// </summary>
    /// <typeparam name="TBlockModel">
    /// The type of the block model. Must inherit from <see cref="BaseBlockModel"/>.
    /// </typeparam>
    /// <typeparam name="TBlockEditor">
    /// The type of the block editor. Must inherit from <see cref="BlockEditor{T}"/> where T is <typeparamref name="TBlockModel"/>.
    /// </typeparam>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to which the block editor will be added.
    /// </param>
    /// <param name="blockName">
    /// The name of the block to be displayed in the editor.
    /// </param>
    /// <param name="blockIcon">
    /// (Optional) The icon associated with the block.
    /// </param>
    /// <returns>
    /// The updated <see cref="IServiceCollection"/>.
    /// </returns>
    public static IServiceCollection AddBlazorBlockEditor<TBlockModel, TBlockEditor>(this IServiceCollection services, string blockName, string? blockIcon = null)
        where TBlockModel : BaseBlockModel
        where TBlockEditor : BlockEditor<TBlockModel>
    {
        // Create blockregistration
        var reg = new BlockRegistration<TBlockModel, TBlockEditor>(blockName, blockIcon);
        RegisterBlock(reg);
        return services;
    }

    /// <summary>
    /// Adds a group registration to the service collection.
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to which the group registration will be added.
    /// </param>
    /// <param name="groupRegistration">
    /// The <see cref="GroupRegistration"/> instance representing the group to be registered.
    /// </param>
    /// <returns>
    /// The updated <see cref="IServiceCollection"/>.
    /// </returns>
    public static IServiceCollection AddBlazorBlocksGroup(this IServiceCollection services, GroupRegistration groupRegistration)
    {
        RegisterGroup(groupRegistration);
        return services;
    }

    private static bool IsRegistered<T>(IServiceCollection services)
    {
        return services.Any(x => x.ServiceType == typeof(T));
    }

    private static IServiceCollection RegisterServices(IServiceCollection services)
    {
        if (!IsRegistered<DragService>(services))
        {
            services.AddScoped<DragService>();
        }
        return services;
    }

    /// <summary>
    /// Adds the default rows to the service.
    /// </summary>
    /// <remarks>
    /// Uses bb- prefixed class names for all rendered HTML output to ensure framework compatibility.
    /// All classes are unique to BlazorBlocks and won't conflict with other UI frameworks.
    /// - Layout containers use "bb-layout-grid"
    /// - Columns use "bb-layout-column--{width}" pattern
    /// - Responsive columns use "bb-layout-column--md-{width}" pattern
    /// </remarks>
    private static void AddDefaultGroups()
    {
        var colRegs = new ColumnRegistration[13];
        for (int i = 1; i < 13; i++)
        {
            // BlazorBlocks-prefixed class names for framework compatibility
            colRegs[i] = new ColumnRegistration($"bb-layout-column--{i}");
        }

        // Add default column definitions
        // Using "bb-layout-grid" for all rendered output
        RegisterGroup(new GroupRegistration("1 column", "bb-layout-grid",
            [colRegs[12]]));
        RegisterGroup(new GroupRegistration("2 columns", "bb-layout-grid",
            [colRegs[6], colRegs[6]]));
        RegisterGroup(new GroupRegistration("3 columns", "bb-layout-grid",
            [colRegs[4], colRegs[4], colRegs[4]]));
        RegisterGroup(new GroupRegistration("4 columns", "bb-layout-grid",
            [colRegs[3], colRegs[3], colRegs[3], colRegs[3]]));
        RegisterGroup(new GroupRegistration("1+2 columns", "bb-layout-grid",
            [colRegs[4], colRegs[8]]));
        RegisterGroup(new GroupRegistration("1+2+full columns", "bb-layout-grid",
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
        RegisterBlock<QuoteBlockModel, QuoteBlockEditor>("Quote", null);
        RegisterBlock<CarouselBlockModel, CarouselBlockEditor>("Carousel", null);
        RegisterBlock<TitleBlockModel, TitleEditorBlock>("Title", null);
        RegisterBlock<ImageBlockModel, ImageBlockEditor>("Image", null);
        RegisterBlock<RawTextBlockModel, RawTextEditorBlock>("Raw Text",
            "https://icon-sets.iconify.design/logo-iconify.svg");
    }
}
