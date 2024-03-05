using BlazorBlocks.BlockEditor.Internals;
using BlazorBlocks.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorBlocks.Support;
internal class DragService
{
    public BlazorBlocksEditorBlockBaseModel DraggedBlock { get; set; }

    public EditorRow DraggedRow { get; set; }
}
