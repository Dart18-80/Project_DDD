using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CQRS.Core.Infrastructure;
using CQRS.Core.Exceptions;
using Post.Cmd.Api.Commands;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]

    public class EditMessageController : ControllerBase
    {
        private readonly ILogger<NewPostController> _logger;
        private readonly ICommandDispacher _commandDispatcher;
        public EditMessageController( ILogger<NewPostController> logger, ICommandDispacher commandDispatcher )
        {
            _logger = logger;
            _commandDispatcher = commandDispatcher;
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> EditMessageAsync (Guid id, EditMessageCommand command)
        {
            try
            {
                command.Id = id;
                await _commandDispatcher.SendAsync(command);

                return Ok( new BaseResponse 
                {
                    Message = "Edit message request completed successfully!"
                });
            }
            catch(InvalidOperationException ex)
            {
                _logger.Log(LogLevel.Warning, ex, "Client made a bad request!");
                return BadRequest( new BaseResponse
                {
                    Message = ex.Message
                });
            }
            catch(AggregateNotFoundException ex)
            {
                _logger.Log(LogLevel.Warning, ex, "Could not retrive aggregate, client passed an incorrect post ID targetting the aggregate!");
                return BadRequest( new BaseResponse
                {
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                const string SAFE_ERROR_MESSAGE = "Error while processing request to edit the message of a post!";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);
                
                return StatusCode( StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }
    }
}