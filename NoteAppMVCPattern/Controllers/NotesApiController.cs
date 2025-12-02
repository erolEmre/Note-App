using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoteApp.WebUI.Models;
using NoteApp.WebUI.Models.ViewModel;
using System;
using System.Security.Claims;
using NoteApp.Core.Entities;
using NoteApp.Infrastructure.Models;
using NoteApp.Application.Repo.Notes;
using NoteApp.Application.Services.Notes;
using NoteApp.Application.Services.Notebooks;
using NoteApp.Application.NewFolder.DTOs;
using NoteApp.Application.Models.DTOs;

namespace NoteApp.WebUI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/[controller]")]
    public class NotesApiController : BaseController
    {
        private readonly INoteService _noteService;
        private readonly NoteValidator _noteValidator;
        private readonly INotebookService _notebookService;

        public NotesApiController(INoteService noteService, NoteValidator noteValidator,INotebookService notebookService)
        {
            _noteService = noteService;
            _noteValidator = noteValidator;
            _notebookService = notebookService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var notes = await _noteService.GetAllNotes();
            return Ok(notes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var note = await _noteService.GetNoteById(id, UserId);
            return note == null ? NotFound() : Ok(new NoteDto
            {
                Id         = note.Id,
                Title      = note.Title,
                Content    = note.Content,
                CreateDate = note.CreateDate.Date,
                NotebookId = note.NotebookId,
                Tags = note.Tags.Select(t => new TagDto
                {
                    Id = t.Id,
                    Name = t.TagName,
                    Color = t.TagColor,
                    TagUsageCount = t.TagUsageCount

                }).ToList()
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] NoteDto noteDto)
        {
            Note note = new Note
            {
                Title = noteDto.Title,
                Content = noteDto.Content,
                CreateDate = DateTime.Now,
                NotebookId = noteDto.NotebookId
            };
            var validationResult = _noteValidator.Validate(note);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            else
            {
                await _noteService.Add(note, UserId);
                return CreatedAtAction(nameof(Get), new { id = noteDto.Id }, noteDto);
            }
            
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] NoteDto updated)
        {
            Note note = new Note
            {
                Id = id,
                Title = updated.Title,
                Content = updated.Content,
                CreateDate = updated.CreateDate,
                NotebookId = updated.NotebookId
            };
            var validationResult = _noteValidator.Validate(note);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            else
            {
                await _noteService.Update(note,UserId);
            }
            var updatedNote = await _noteService.GetNoteById(id, UserId);
            NoteDto updatedNoteDto = new NoteDto
            {
                Id = updatedNote.Id,
                Title = updatedNote.Title,
                Content = updatedNote.Content,
                CreateDate = updatedNote.CreateDate,
                NotebookId = updatedNote.NotebookId,
                Tags = updatedNote.Tags.Select(t => new TagDto
                {
                    Id = t.Id,
                    Name = t.TagName,
                    Color = t.TagColor,
                    TagUsageCount = t.TagUsageCount
                }).ToList()
            };
            return Ok(updatedNoteDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if(_noteService.GetNoteById(id,UserId) == null)
            {
                return NotFound();
            } else 
            { 
                await _noteService.Delete(id, UserId);
            }
            return NoContent();
        }
        
        [Authorize]
        [HttpGet("notebooks/{id}/notes")]
        public async Task<IActionResult> GetMyNotes(int id)
        {
            
            var notebookNotes = await _notebookService.Get(id);
            if(notebookNotes == null)
            {
                return Forbid();
            }
            return Ok(new NotebookDTO
            {
                Id = notebookNotes.Id,
                Name = notebookNotes.Name,
                NoteList = notebookNotes.Notes.Select(n => new NoteDto
                {
                    Id = n.Id,
                    Title = n.Title,
                    Content = n.Content,
                    CreateDate = n.CreateDate, 
                    NotebookId = n.NotebookId,
                    Tags = n.Tags.Select(t => new TagDto
                    {
                        Id = t.Id,
                        Name = t.TagName,
                        Color = t.TagColor,
                        TagUsageCount = t.TagUsageCount
                    }).ToList()
                }).ToList()
            });
        }
    }
}


