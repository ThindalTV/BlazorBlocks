using System.Text;
using System.Text.Json;
using BlazorBlocks.Internals.Models;

namespace BlazorBlocks;

/// <summary>
/// The model used for the BlazorBlocks editor.
/// It contains properties and methods for managing the editor rows and serializing/deserializing the model to JSON/HTML.
/// </summary>
public class BlazorBlocksModel : BaseModel
{
    /// <summary>
    /// Gets or sets the list of editor group models.
    /// </summary>
    public List<EditorGroupModel> Groups { get; set; }

    private readonly JsonSerializerOptions _jsonSerializerOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="BlazorBlocksModel"/> class.
    /// </summary>
    public BlazorBlocksModel()
    {
        Groups = [];
        _jsonSerializerOptions = new JsonSerializerOptions() { TypeInfoResolver = new JsonSerializerTypeResolver(BlockRegistrationService.Blocks) };
    }

    /// <summary>
    /// Gets the HTML representation of the editor model.
    /// </summary>
    /// <returns>The HTML string.</returns>
    public string GetHtml()
    {
        var sb = new StringBuilder();
        foreach (var group in Groups)
        {
            sb.Append(group.Render());
        }
        return sb.ToString();
    }

    /// <summary>
    /// Gets the JSON representation of the editor model.
    /// </summary>
    /// <returns>The JSON string.</returns>
    public string GetJson()
    {
        return JsonSerializer.Serialize(this, _jsonSerializerOptions);
    }

    /// <summary>
    /// Loads the editor model from the specified JSON string.
    /// </summary>
    /// <param name="data">The JSON string representing the editor model.</param>
    public void Load(string data)
    {
        Console.WriteLine("Loading");
        if (string.IsNullOrWhiteSpace(data))
        {
            Groups = [];
            return;
        }

        Console.WriteLine("Validated, deserializing");
        var model = JsonSerializer.Deserialize<BlazorBlocksModel>(data, _jsonSerializerOptions);


        Console.WriteLine("Model is " + model == null ? "null" : "not null" + ", setting groups");

        Groups = model?.Groups ?? [];

        Console.WriteLine("Setting parents");
        SetGroupParents(Groups);
    }

    private void SetGroupParents(List<EditorGroupModel> groups)
    {
        Console.WriteLine("Number of groups is " + groups.Count);
        foreach (var group in groups)
        {
            group.ParentModel = this;
            SetColumnParents(group, group.Columns);
        }
    }

    private static void SetColumnParents(EditorGroupModel parent, List<EditorColumnModel> groupColumns)
    {
        Console.WriteLine("Setting column parents. columncount is " + groupColumns.Count);
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
