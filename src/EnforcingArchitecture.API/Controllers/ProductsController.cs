using EnforcingArchitecture.API.Application.Messages.Commands;
using EnforcingArchitecture.API.Application.Messages.Queries;
using EnforcingArchitecture.API.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EnforcingArchitecture.API.Controllers;

[Route("api/products")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET api/products/
    /// <summary>
    /// Obtém todas os produtos 
    /// </summary>   
    /// <response code="200">Sucesso</response>               
    /// <response code="204">Nenhum registro localizado</response>             
    [HttpGet]
    [ProducesResponseType(typeof(List<ProductModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllProducts()
    {
        var result = await _mediator.Send(new FindAllProductsQuery());

        return result == null ? NoContent() : Ok(result);
    }

    // POST api/products/
    /// <summary>
    /// Grava o produto
    /// </summary>   
    /// <remarks>
    /// Exemplo request:
    ///
    ///     POST / Produto
    ///     {
    ///         "title": "Sandalia",
    ///         "description": "Sandália Preta Couro Salto Fino",
    ///         "price": 249.50,
    ///         "quantity": 100       
    ///     }
    /// </remarks>        
    /// <returns>Retorna objeto criado da classe Produto</returns>                
    /// <response code="201">O produto foi incluído corretamente</response>                
    /// <response code="400">Falha na requisição</response>         
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ActionName("NewProduct")]
    public async Task<IActionResult> PostAsync([FromBody] CreateProductCommand command)
    {
        var result = await _mediator.Send(command);

        return result.IsValid() ? CreatedAtAction("NewProduct", new { id = result.Response }, command) : BadRequest(result.Errors);
    }

}
