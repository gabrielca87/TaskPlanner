using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskPlanner.Api.Extensions;
using TaskPlanner.Application.DTOs;
using TaskPlanner.Application.Interfaces.Services;
using TaskPlanner.Application.Requests.Tasks;

namespace TaskPlanner.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/task-items")]
public sealed class TaskItemsController : ControllerBase
{
    private readonly ITaskItemService _taskItemService;

    public TaskItemsController(ITaskItemService taskItemService)
    {
        _taskItemService = taskItemService;
    }

    [HttpGet]
    [ProducesResponseType<IReadOnlyList<TaskItemDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<TaskItemDto>>> GetByCurrentUserAsync(CancellationToken cancellationToken)
    {
        var userId = User.GetCurrentUserId();
        var tasks = await _taskItemService.GetByUserIdAsync(userId, cancellationToken);
        return Ok(tasks);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<TaskItemDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskItemDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var taskItem = await _taskItemService.GetByIdAsync(id, cancellationToken);
        return Ok(taskItem);
    }

    [HttpPost]
    [ProducesResponseType<TaskItemDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskItemDto>> CreateAsync([FromBody] CreateTaskItemRequest request, CancellationToken cancellationToken)
    {
        var createdTask = await _taskItemService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = createdTask.Id }, createdTask);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType<TaskItemDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskItemDto>> UpdateAsync(Guid id, [FromBody] UpdateTaskItemRequest request, CancellationToken cancellationToken)
    {
        var updatedTask = await _taskItemService.UpdateAsync(request, cancellationToken);
        return Ok(updatedTask);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        await _taskItemService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}