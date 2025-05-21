using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace bike_matrix_tech_int.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BikesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BikesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bike>>> Get()
        {
            return await _context.Bikes.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Bike>> GetById(int id)
        {
            var bike = await _context.Bikes.FindAsync(id);
            return bike == null ? NotFound() : Ok(bike);
        }

        [HttpPost]
        public async Task<ActionResult<Bike>> Post(Bike bike)
        {
            // Validate bike data
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bike.Id = 0;
            _context.Bikes.Add(bike);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = bike.Id }, bike);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Bike bike)
        {
            if (id != bike.Id)
            {
                return BadRequest("Id mismatch");
            }

            // Validate bike data
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // If the Id is not in the database
            if (!_context.Bikes.Any(bikeInDatabase => bikeInDatabase.Id == id))
            {
                return NotFound();
            }

            _context.Entry(bike).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var bike = await _context.Bikes.FindAsync(id);

            // If id is not in database
            if (bike == null)
            {
                return NotFound();
            }

            _context.Bikes.Remove(bike);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
