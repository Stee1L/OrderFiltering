using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderFiltering.Data;
using OrderFiltering.Models;
using OrderFiltering.ViewModels;

namespace OrderFiltering.Controllers;

[Route("api/order/[controller]")]
[ApiController]
public class OrderController: ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<OrderController> _logger;
    private readonly IConfiguration _configuration;

    public OrderController(AppDbContext context, ILogger<OrderController> logger, IConfiguration configuration)
    {
        _context = context;
        _logger = logger;
        _configuration = configuration;
    }

    /// <summary>
    /// Получение всех заказов и передача их пользователю через API
    /// </summary>
    /// <returns>Заказы из БД</returns>
    [HttpGet("orders")]
    public async Task<ActionResult<IEnumerable<OrderModel>>> GetOrders()
    {
        try
        {
            var orders = await _context.Orders.ToListAsync();
            _logger.LogInformation("Получены все заказы из БД");
            return Ok(orders);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Ошибка при получении заказов: {ex.Message}");
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Создание нового заказа и сохранение его в БД
    /// </summary>
    /// <param name="model">Входные данные нового заказа</param>
    [HttpPost("create")]
    public async Task<IActionResult> AddOrder([FromBody] OrderViewModel model)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Некорректные входные данные для создания заказа");
            return BadRequest(ModelState);
        }
        
        try
        {
            var order = new OrderModel
            {
                Id = Guid.NewGuid(),
                OrderWeight = model.OrderWeight,
                DistrictId = model.DistrictId,
                DeliveryTime = model.DeliveryTime
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Создан новый заказ с ID: {order.Id}");
            return Ok(order);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Ошибка при создании заказа: {ex.Message}");
            return StatusCode(500, $"Iternal server error: {ex}");
        }
    }

    /// <summary>
    /// Изменение заказа по указанному идентификатору
    /// </summary>
    /// <param name="id">Уникальный идентификатор заказа</param>
    /// <param name="model">Изменённые данные</param>
    [HttpPut("change")]
    public async Task<IActionResult> PutOrder(Guid id, [FromBody] OrderViewModel model)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Некорректные входные данные для изменения заказа");
            return BadRequest(ModelState);
        }
        
        try
        {
            var order = await _context.Orders.AsNoTracking().FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                _logger.LogWarning($"Заказ с ID: {id} не найден");
                throw new Exception("Заказ по указанному идентификатору не найден");
            }

            order.OrderWeight = model.OrderWeight;
            order.DistrictId = model.DistrictId;
            order.DeliveryTime = model.DeliveryTime;

            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Изменен заказ с ID: {id}");
            return Ok(order);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Ошибка при изменении заказа: {ex.Message}");
            return StatusCode(500, $"Iternal server error: {ex}");
        }
    }
    
    /// <summary>
    /// Удаление заказа по указанному идентификатору
    /// </summary>
    /// <param name="id">Уникальный идентификатор заказа</param>
    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteOrder(Guid id)
    {
        try
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                _logger.LogWarning($"Заказ с ID: {id} не найден");
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Заказ с ID: {id} удален");
            return Ok(new { Message = "Заказ успешно удален" });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Ошибка во время удаления заказа: {ex.Message}");
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Фильтрация заказов по времени
    /// </summary>
    /// <param name="model">Входные данные для фильтрации</param>
    [HttpPost("filter")]
    public async Task<IActionResult> FilterOrders([FromBody] FilterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Некорректные входные данные для фильтрации заказов");
            return BadRequest(ModelState);
        }
        
        try
        {
            var filteredOrders = await _context.Orders
                .Where(o => o.DistrictId == model.DistrictId &&
                            o.DeliveryTime >= model.DeliveryTime &&
                            o.DeliveryTime <= model.DeliveryTime.AddMinutes(30))
                .ToListAsync();

            var district = await _context.Districts.FirstOrDefaultAsync(d => d.Id == model.DistrictId);
            if (district == null)
            {
                _logger.LogWarning($"Район с ID: {model.DistrictId} не найден");
                throw new Exception("Указанный район не найден");
            }

            _logger.LogInformation($"Отфильтровано {filteredOrders.Count} заказов для района {district.Name}" +
                                   $"с {model.DeliveryTime} по {model.DeliveryTime.AddMinutes(30)}");

            switch (model.FilterResultOutputType)
            {
                case "Database":
                    await WriteOrdersToDb(filteredOrders, district.Name);
                    return Ok(new { FilteredOrders = filteredOrders });
                case "File":
                {
                    var resultFilePath = _configuration["ResultFilePath"];
                    if (resultFilePath == null)
                    {
                        _logger.LogError("Ошибка в файле конфигурации! Проверьте поле \"ResultFilePath\"");
                        return BadRequest("Ошибка в файле конфигурации! Проверьте поле \"ResultFilePath\"");
                    }

                    await WriteOrdersToFile(filteredOrders, resultFilePath, district.Name);
                    return Ok(new { FilteredOrders = filteredOrders, ResultFilePath = resultFilePath });
                }
                default:
                    _logger.LogWarning("Некорректный результирующий тип");
                    return BadRequest("Некорректный результирующий тип");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Ошибка при выполнении фильтрации заказов: {ex.Message}");
            return StatusCode(500, $"Iternal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Сохранение результатов в файл
    /// </summary>
    /// <param name="filteredOrders">Выборка заказов</param>
    /// <param name="resultFilePath">Результирующий файл</param>
    /// <param name="districtName">Наименование района</param>
    private async Task WriteOrdersToFile(List<OrderModel> filteredOrders, string resultFilePath, string districtName)
    {
        try
        {
            var writer = new StreamWriter(resultFilePath);
            await writer.WriteLineAsync($"Отфильтрованные заказы для района {districtName}:");
            foreach (var order in filteredOrders)
            {
                await writer.WriteLineAsync(
                    $"Id: {order.Id}, Вес: {order.OrderWeight}, Время доставки: {order.DeliveryTime}");
            }

            _logger.LogInformation($"Результаты фильтрации записаны в файл: {resultFilePath}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Ошибка при записи результатов фильтрации в файл: {ex.Message}");
        }
    }

    /// <summary>
    /// Сохранение результатов в базу данных
    /// </summary>
    /// <param name="filteredOrders">Выборка заказов</param>
    /// <param name="districtName">Наименование района</param>
    private async Task WriteOrdersToDb(List<OrderModel> filteredOrders, string districtName)
    {
        try
        {
            var orders = filteredOrders.Select(o => new FilteredOrderModel
            {
                OrderId = o.Id,
                OrderWeight = o.OrderWeight,
                DistrictId = o.DistrictId,
                DisctictName = districtName,
                DeliveryTime = o.DeliveryTime
            }).ToList();

            _context.FilteredOrders.AddRange(orders);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Результаты фильтрации записаны в базу данных");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Ошибка при записи результатов фильтрации в базу данных: {ex.Message}");
        }
    }
    
}