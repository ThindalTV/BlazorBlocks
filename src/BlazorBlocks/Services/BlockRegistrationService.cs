using BlazorBlocks.Blocks.CarouselBlock;
using BlazorBlocks.Blocks.ImageBlock;
using BlazorBlocks.Blocks.QuoteBlock;
using BlazorBlocks.Blocks.RawTextBlock;
using BlazorBlocks.Blocks.TitleBlock;
using BlazorBlocks.Model;
using BlazorBlocks.Support;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorBlocks;
/// <summary>
/// Service for registering blocks and rows in BlazorBlocks.
/// </summary>
public static class BlockRegistrationService
{
    private static List<BlazorBlocksBlockRegistration> _registeredBlocks { get; } = new List<BlazorBlocksBlockRegistration>();
    private static List<RowRegistration> _registeredRows { get; } = new List<RowRegistration>();

    internal static IReadOnlyList<BlazorBlocksBlockRegistration> RegisteredBlocks => _registeredBlocks;
    internal static IReadOnlyList<RowRegistration> RegisteredRows => _registeredRows;

    /// <summary>
    /// Registers a block.
    /// </summary>
    /// <param name="block">The block to register.</param>
    private static void RegisterBlock(BlazorBlocksBlockRegistration block)
    {
        _registeredBlocks.Add(block);
    }

    /// <summary>
    /// Registers a row.
    /// </summary>
    /// <param name="row">The row to register.</param>
    private static void RegisterRow(RowRegistration row)
    {
        _registeredRows.Add(row);
    }

    /// <summary>
    /// Adds the default blocks and rows to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddBlazorBlocks(this IServiceCollection services)
    {
        AddDefaultBlocks();
        AddDefaultRows();

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
        AddDefaultRows();

        if (includeDefaultBlocks)
        {
            AddDefaultBlocks();
        }

        foreach (var block in blockRegistrations)
        {
            RegisterBlock(block);
        }

        services = RegisterServices(services);

        return services;
    }

    /// <summary>
    /// Adds the specified row registrations to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="rowRegistrations">The row registrations to add.</param>
    /// <param name="includeDefaultRows">Whether to include the default rows.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddBlazorBlocks(this IServiceCollection services, List<RowRegistration> rowRegistrations, bool includeDefaultRows)
    {
        AddDefaultBlocks();

        if (includeDefaultRows)
        {
            AddDefaultRows();
        }

        foreach (var row in rowRegistrations)
        {
            RegisterRow(row);
        }
        return services;
    }

    /// <summary>
    /// Adds the specified block and row registrations to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="blockRegistrations">The block registrations to add.</param>
    /// <param name="rowRegistrations">The row registrations to add.</param>
    /// <param name="includeDefaults">Whether to include the default blocks and rows.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddBlazorBlocks(this IServiceCollection services, List<BlazorBlocksBlockRegistration> blockRegistrations, List<RowRegistration> rowRegistrations, bool includeDefaults = true)
    {
        if (includeDefaults)
        {
            AddDefaultBlocks();
            AddDefaultRows();
        }

        foreach (var block in blockRegistrations)
        {
            RegisterBlock(block);
        }

        foreach (var row in rowRegistrations)
        {
            RegisterRow(row);
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
    private static void AddDefaultRows()
    {
        // Add default column definitions
        RegisterRow(new RowRegistration("1 column", new List<ColumnRegistration> { new ColumnRegistration("col-12") }));
        RegisterRow(new RowRegistration("2 columns",
            new List<ColumnRegistration> { new ColumnRegistration("col-6"), new ColumnRegistration("col-6") }));
        RegisterRow(new RowRegistration("3 columns",
            new List<ColumnRegistration> { new ColumnRegistration("col-4"), new ColumnRegistration("col-4"), new ColumnRegistration("col-4") }));
        RegisterRow(new RowRegistration("4 columns",
                       new List<ColumnRegistration> { new ColumnRegistration("col-3"), new ColumnRegistration("col-3"), new ColumnRegistration("col-3"), new ColumnRegistration("col-3") }));
        RegisterRow(new RowRegistration("1+2 columns",
            new List<ColumnRegistration> { new ColumnRegistration("col-4"), new ColumnRegistration("col-8") }));

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
