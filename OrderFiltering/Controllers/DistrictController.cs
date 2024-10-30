using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderFiltering.Data;
using OrderFiltering.Models;
using OrderFiltering.ViewModels;

namespace OrderFiltering.Controllers;

[Route("api/district/[controller]")]
[ApiController]
public class DistrictController: ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<OrderController> _logger;
    
    public DistrictController(AppDbContext context, ILogger<OrderController> logger, IConfiguration configuration)
    {
        _context = context;
        _logger = logger;
    }
    
    /// <summary>
    /// Получение всех районов и передача их пользователю через API
    /// </summary>
    /// <returns>Заказы из БД</returns>
    [HttpGet("districts")]
    public async Task<ActionResult<IEnumerable<DistrictModel>>> GetDistricts()
    {
        try
        {
            var districts = await _context.Districts.ToListAsync();
            _logger.LogInformation("Получены все районы из БД");
            return Ok(districts);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Ошибка при получении районов: {ex.Message}");
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Создание нового района и сохранение его в БД
    /// </summary>
    /// <param name="model"></param>
    [HttpPost("create")]
    public async Task<IActionResult> AddDistrict([FromBody] DistrictViewModel model)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Некорректные входные данные для создания района");
            return BadRequest(ModelState);
        }
        
        try
        {
            var district = new DistrictModel()
            {
                Id = Guid.NewGuid(),
                Name = model.Name
            };

            _context.Districts.Add(district);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Создан новый район с ID: {district.Id}");
            return Ok(district);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Ошибка при создании района: {ex.Message}");
            return StatusCode(500, $"Iternal server error: {ex}");
        }
    }
    
    /// <summary>
    /// Изменение района по указанному идентификатору
    /// </summary>
    /// <param name="id">Уникальный идентификатор района</param>
    /// <param name="model">Изменённые данные</param>
    [HttpPut("change")]
    public async Task<IActionResult> PutDistrict(Guid id, [FromBody] DistrictViewModel model)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Некорректные входные данные для изменения района");
            return BadRequest(ModelState);
        }
        
        try
        {
            var district = await _context.Districts.AsNoTracking().FirstOrDefaultAsync(o => o.Id == id);

            if (district == null)
            {
                _logger.LogWarning($"Район с ID: {id} не найден");
                throw new Exception("Район по указанному идентификатору не найден");
            }

            district.Name = model.Name;

            _context.Districts.Update(district);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Изменен район с ID: {id}");
            return Ok(district);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Ошибка при изменении района: {ex.Message}");
            return StatusCode(500, $"Iternal server error: {ex}");
        }
    }
    
    /// <summary>
    /// Удаление района по указанному идентификатору
    /// </summary>
    /// <param name="id">Уникальный идентификатор района</param>
    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteDistrict(Guid id)
    {
        try
        {
            var district = await _context.Districts.FirstOrDefaultAsync(d => d.Id == id);

            if (district == null)
            {
                _logger.LogWarning($"Район с ID: {id} не найден");
                return NotFound();
            }

            _context.Districts.Remove(district);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Район с ID: {id} удален");
            return Ok(new { Message = "Район успешно удален" });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Ошибка при удалении района: {ex.Message}");
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}