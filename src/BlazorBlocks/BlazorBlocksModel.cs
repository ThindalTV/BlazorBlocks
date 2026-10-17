using System.Text;
using System.Text.Json;
using BlazorBlocks.Internals.Models;

namespace BlazorBlocks;

/// <summary>
/// The model used for the BlazorBlocks editor.
/// It contains properties and methods for managing the editor groups and serializing/deserializing the model to JSON/HTML.
/// </summary>
public class BlazorBlocksModel : BaseModel
{
    /// <summary>
    /// Gets or sets the list of editor group models.
    /// </summary>
    public List<EditorGroupModel> Groups { get; set; }

    private JsonSerializerOptions? _jsonSerializerOptions;
    private int _optionsBlockCount = -1;

    /// <summary>
    /// Initializes a new instance of the <see cref="BlazorBlocksModel"/> class.
    /// </summary>
    public BlazorBlocksModel()
    {
        Groups = [];
    }

    /// <summary>
    /// Gets the serializer options, built on first use so blocks registered after construction are included.
    /// Rebuilt if the set of registered blocks has changed since they were last built.
    /// </summary>
    private JsonSerializerOptions GetJsonSerializerOptions()
    {
        var blocks = BlockRegistrationService.Blocks;
        if (_jsonSerializerOptions == null || _optionsBlockCount != blocks.Count)
        {
            _jsonSerializerOptions = new JsonSerializerOptions();
            _jsonSerializerOptions.Converters.Add(new BlockJsonConverter(blocks));
            _optionsBlockCount = blocks.Count;
        }

        return _jsonSerializerOptions;
    }

    /// <summary>
    /// Gets the HTML representation of the editor model.
    /// </summary>
    /// <param name="renderer">
    /// The renderer to use, typically the injected <see cref="BlockHtmlRenderer"/> so block renderers can use dependency injection.
    /// When omitted, a standalone renderer without services is used for this call.
    /// </param>
    /// <returns>The HTML string.</returns>
    public async Task<string> GetHtmlAsync(BlockHtmlRenderer? renderer = null)
    {
        var ownedRenderer = renderer is null ? BlockHtmlRenderer.CreateStandalone() : null;
        try
        {
            renderer ??= ownedRenderer!;
            var sb = new StringBuilder();
            foreach (var group in Groups)
            {
                sb.Append(await group.RenderAsync(renderer));
            }
            return sb.ToString();
        }
        finally
        {
            if (ownedRenderer is not null)
            {
                await ownedRenderer.DisposeAsync();
            }
        }
    }

    /// <summary>
    /// Gets the JSON representation of the editor model.
    /// </summary>
    /// <returns>The JSON string.</returns>
    public string GetJson()
    {
        return JsonSerializer.Serialize(this, GetJsonSerializerOptions());
    }

    /// <summary>
    /// Loads the editor model from the specified JSON string.
    /// </summary>
    /// <param name="data">The JSON string representing the editor model.</param>
    public void Load(string data)
    {
        if (string.IsNullOrWhiteSpace(data))
        {
            Groups = [];
            return;
        }

        var model = JsonSerializer.Deserialize<BlazorBlocksModel>(data, GetJsonSerializerOptions());

        Groups = model?.Groups ?? [];

        SetGroupParents(Groups);
    }

    private void SetGroupParents(List<EditorGroupModel> groups)
    {
        foreach (var group in groups)
        {
            group.ParentModel = this;
            SetColumnParents(group, group.Columns);
        }
    }

    private static void SetColumnParents(EditorGroupModel parent, List<EditorColumnModel> groupColumns)
    {
        foreach (var column in groupColumns)
        {
            column.ParentModel = parent;
            SetBlockParents(column, column.Blocks);
        }
    }

    private static void SetBlockParents(EditorColumnModel column, List<EditorBlockModel> columnBlocksModels)
    {
        foreach (var block in columnBlocksModels)
        {
            block.ParentModel = column;
        }
    }

    public void RemoveGroup(EditorGroupModel editorGroupModel)
    {
        Groups.Remove(editorGroupModel);
    }

    public void AddGroup(EditorGroupModel editorGroupModel, int index)
    {
        Groups.Insert(index, editorGroupModel);
    }
}
