using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoteAppMVCPattern.Models;
using NoteAppMVCPattern.Models.ViewModel;
using System;
using System.Security.Claims;


namespace NoteAppMVCPattern.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/[controller]")]
    public class NotesApiController : ControllerBase
    {
        private readonly AppDBContext _context;

        public NotesApiController(AppDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var notes = _context.Notes.ToList();
            return Ok(notes);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var note = _context.Notes.Find(id);
            return note == null ? NotFound() : Ok(note);
        }

        [HttpPost]
        public IActionResult Create(Note note)
        {
            _context.Notes.Add(note);
            _context.SaveChanges();
            return CreatedAtAction(nameof(Get), new { id = note.Id }, note);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, Note updated)
        {
            var note = _context.Notes.Find(id);
            if (note == null) return NotFound();

            note.Title = updated.Title;
            note.Content = updated.Content;
            note.Tag = updated.Tag;
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var note = _context.Notes.Find(id);
            if (note == null) return NotFound();

            _context.Notes.Remove(note);
            _context.SaveChanges();
            return NoContent();
        }
        
        [Authorize]
        [HttpGet("my")]
        public async Task<IActionResult> GetMyNotes()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var notes = await _context.Notes
                .Where(n => n.UserId == userId)
                .ToListAsync();

            return Ok(notes);
        }
    }
}


