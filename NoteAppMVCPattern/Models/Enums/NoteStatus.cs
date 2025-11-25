using System.ComponentModel.DataAnnotations;

namespace NoteAppMVCPattern.Models.Enums
{
    public enum NoteStatus
    {
        None = 0,
        Done = 1,
        Todo = 2,
        Planned = 3,
    }
}