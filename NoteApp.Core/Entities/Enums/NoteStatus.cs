using System.ComponentModel.DataAnnotations;

namespace NoteApp.Core.Entities.Enums
{
    public enum NoteStatus
    {
        None = 0,
        Done = 1,
        Todo = 2,
        Planned = 3,
    }
}