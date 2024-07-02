using Application.Transactions.Commands;
using Application.Transactions.Queries;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TransactionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTransaction([FromBody] Transaction transaction)
        {
            await _mediator.Send(new CreateTransactionCommand { Transaction = transaction });
            return Ok();
        }

        [HttpGet("id")]
        public async Task<IActionResult> GetTransactionById([FromBody] GetTransactionByIdQuery id)
        {
            var transaction = await _mediator.Send(id );
            if (transaction == null)
                return NotFound();

            return Ok(transaction);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllTransactions()
        {
            var transactions = await _mediator.Send(new GetAllTransactionsQuery());
            return Ok(transactions);
        }
    }
}
