using Microsoft.AspNetCore.Mvc;
using OnlineShop.Common.Services;
using OnlineShop.Infrastructure.Models;
using OnlineShop.REST.Models;

namespace OnlineShop.REST.Controllers;

[ApiController]
[Route("api/customers")]
[Produces("application/json")]
public sealed class CustomersController : ControllerBase
{
    private readonly ICrudServiceAsync<CustomerModel> _customers;

    public CustomersController(ICrudServiceAsync<CustomerModel> customers)
    {
        _customers = customers;
    }

    /// <summary>Returns one page of customers.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CustomerResponseModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<CustomerResponseModel>>> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        if (page < 1 || pageSize < 1 || pageSize > 100)
            return BadRequest(new { message = "page must be >= 1 and pageSize must be between 1 and 100." });

        var customers = await _customers.ReadAllAsync(page, pageSize);
        return Ok(customers.Select(ToResponse));
    }

    /// <summary>Returns a customer by its GUID.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CustomerResponseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerResponseModel>> GetById(Guid id)
    {
        var customer = await _customers.ReadAsync(id);
        return customer is null ? NotFound() : Ok(ToResponse(customer));
    }

    /// <summary>Creates a customer.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(CustomerResponseModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CustomerResponseModel>> Create(CustomerCreateModel model)
    {
        var customer = new CustomerModel
        {
            Id = Guid.NewGuid(),
            Name = model.Name.Trim(),
            Email = model.Email.Trim(),
            Age = model.Age
        };

        if (!await _customers.CreateAsync(customer))
            return Conflict(new { message = "A customer with this ID already exists." });

        return CreatedAtAction(nameof(GetById), new { id = customer.Id }, ToResponse(customer));
    }

    /// <summary>Updates an existing customer.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, CustomerUpdateModel model)
    {
        var customer = await _customers.ReadAsync(id);
        if (customer is null)
            return NotFound();

        customer.Name = model.Name.Trim();
        customer.Email = model.Email.Trim();
        customer.Age = model.Age;
        return await _customers.UpdateAsync(customer) ? NoContent() : NotFound();
    }

    /// <summary>Deletes an existing customer.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var customer = await _customers.ReadAsync(id);
        if (customer is null)
            return NotFound();

        return await _customers.RemoveAsync(customer) ? NoContent() : NotFound();
    }

    private static CustomerResponseModel ToResponse(CustomerModel customer) => new()
    {
        Id = customer.Id,
        Name = customer.Name,
        Email = customer.Email,
        Age = customer.Age
    };
}
