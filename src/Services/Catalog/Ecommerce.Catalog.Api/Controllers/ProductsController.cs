using Ecommerce.Catalog.Api.Contracts.Products;
using Ecommerce.Catalog.Application.Products.Create;
using Ecommerce.Catalog.Application.Products.GetById;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Catalog.Api.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController(ISender sender) : ControllerBase
    {

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductRequest request, CancellationToken cancellationToken)
        {

            var command = new CreateProductCommand(
                 request.Name,
                 request.Price,
                 request.Stock,
                 request.CategoryId
            );

            var result = await sender.Send(command, cancellationToken);

            //handle errors
            if (!result.IsSuccess) BadRequest(result.Error);

            //Return the action
            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Value },
                result
                );
        }


        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var query = new GetProductByIdQuery(id);

            var result = await sender.Send(
                query,
                cancellationToken);

            if (!result.IsSuccess)
            {
                return NotFound(result.Error);
            }

            return Ok(result.Value);

        }

    }
}
