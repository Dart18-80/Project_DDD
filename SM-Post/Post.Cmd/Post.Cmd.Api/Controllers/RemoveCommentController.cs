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
    public class RemoveCommentController : ControllerBase
    {
        private readonly ILogger<RemoveCommentController> _logger;
        private readonly ICommandDispacher _commandDispatcher;
        public RemoveCommentController( ILogger<RemoveCommentController> logger, ICommandDispacher commandDispatcher )
        {
            _logger = logger;
            _commandDispatcher = commandDispatcher;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> RemoveCommentAsync(Guid id, RemoveCommentCommand command)
        {
            try
            {
                command.Id = id;
                await _commandDispatcher.SendAsync(command);

                return Ok( new BaseResponse 
                {
                    Message = "Remove Comment request completed successfully!"
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
                const string SAFE_ERROR_MESSAGE = "Error while processing request to remove a comment from a post!";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);
                
                return StatusCode( StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }
    }
}