using System.Text;
using System.Text.Json;

namespace BlazorBlocks.Model;

/// <summary>
/// The model used for the BlazorBlocks editor.
/// It contains properties and methods for managing the editor rows and serializing/deserializing the model to JSON/HTML.
/// </summary>
public class BlazorBlocksEditorModel : EditorModel
{
    /// <summary>
    /// Gets or sets the list of editor group models.
    /// </summary>
    public List<EditorGroupModel> Groups { get; set; }

    private readonly JsonSerializerOptions _jsonSerializerOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="BlazorBlocksEditorModel"/> class.
    /// </summary>
    public BlazorBlocksEditorModel()
    {
        Groups = new List<EditorGroupModel>();

        _jsonSerializerOptions = new JsonSerializerOptions() { TypeInfoResolver = new JsonSerializerTypeResolver(BlockRegistrationService.RegisteredBlocks) };
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
        var model = JsonSerializer.Deserialize<BlazorBlocksEditorModel>(data, _jsonSerializerOptions);
        Groups = model?.Groups ?? new List<EditorGroupModel>();

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

    private void SetColumnParents(EditorGroupModel parent, List<EditorColumnModel> groupColumns)
    {
        foreach (var column in groupColumns)
        {
            column.ParentModel = parent;
            SetBlockParents(column, column.BlocksModels);
        }
    }

    private void SetBlockParents(EditorColumnModel column, List<EditorBlockModel> columnBlocksModels)
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
