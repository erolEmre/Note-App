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
            return note == null ? NotFound() : Ok(note);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Note note)
        {
            var validationResult = _noteValidator.Validate(note);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            else
            {
                await _noteService.Add(note, UserId);
                return CreatedAtAction(nameof(Get), new { id = note.Id }, note);
            }
            
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Note updated)
        {            
            var validationResult = _noteValidator.Validate(updated);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            else
            {
                await _noteService.Update(updated,UserId);
            }
            return NoContent();
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
            return Ok(notebookNotes.Notes);
        }
    }
}


