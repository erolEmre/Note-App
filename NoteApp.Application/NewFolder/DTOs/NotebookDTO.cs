using NoteApp.Application.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteApp.Application.NewFolder.DTOs
{
    public class NotebookDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<NoteDto> NoteList { get; set; } = new();
    }
}
