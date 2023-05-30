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
        private readonly IRiddleRepository _riddleRepository;
        public RiddleController(IRiddleRepository riddleRepository)
        {
            _riddleRepository = riddleRepository;
        }

        //getall
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
        //works
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            Riddle riddle = await _riddleRepository.GetByIdAsync(id);
            if (riddle == null)
                return NotFound();

            return Ok(riddle);
        }

        //Create/edit
        //works
        [HttpPost]
        public async Task<IActionResult> CreateEdit(Riddle riddle)
        {
            if(riddle.RiddleId == 0)
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
        //works
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _riddleRepository.GetByIdAsync(id);

            if(result == null)
                return NotFound();

            _riddleRepository.Delete(result);
            return Ok("Success Deletion");
        }


    }

    
}
