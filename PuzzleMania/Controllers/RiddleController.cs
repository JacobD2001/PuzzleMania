using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PuzzleMania.Interfaces;
using PuzzleMania.Models;

namespace PuzzleMania.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RiddleController : ControllerBase
    {
        //TODO - fix REST API its because of the gameId might do it nullable and than see
        private readonly IRiddleRepository _riddleRepository;
        public RiddleController(IRiddleRepository riddleRepository)
        {
            _riddleRepository = riddleRepository;
        }

        //getall
        //https://localhost:7013/api/Riddle
        //works
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var riddles = await _riddleRepository.GetAll();

            if (riddles.Any())
            {
                return Ok(riddles);
            }
            else
            {
                return Ok("No riddles available.");
            }
        }


        //getbyid
        //https://localhost:7013/api/Riddle/1
        //works
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int gameId, int id)
        {
            Riddle riddle = await _riddleRepository.GetByIdAsync(gameId, id);
            if (riddle == null)
                return NotFound();

            return Ok(riddle);
        }

        //Create/edit
        //https://localhost:7013/api/Riddle + body
        //works
        [HttpPost]
        public async Task<IActionResult> CreateEdit(Riddle riddle)
        {
            if (riddle.RiddleId == 0)
            {
                _riddleRepository.Add(riddle);
                return Ok(riddle);
            }
            else
            {
                _riddleRepository.Update(riddle);
                return Ok(riddle);
            }
        }

        //Delete
        //https://localhost:7013/api/Riddle/1
        //works
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int gameId, int id)
        {
            var result = await _riddleRepository.GetByIdAsync(gameId, id);

            if (result == null)
                return NotFound();

            _riddleRepository.Delete(result);
            return Ok("Success Deletion");
        }


    }


}