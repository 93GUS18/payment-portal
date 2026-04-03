using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using portal_api.Data;
using portal_api.Models.DbModel;

namespace portal_api.Controllers
{
    [Route("api/Payments")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly portal_apiContext _context;

        public PaymentsController(portal_apiContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<object>> GetPaymentsDBModel()
        {
            try
            {
                var payments = await _context.PaymentsDBModel.Where(p => !p.IsDeleted).ToListAsync();

                return Ok(
                    new
                    {
                        statusCode = StatusCodes.Status200OK,
                        message = "",
                        data = payments
                    }
                );
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        statusCode = StatusCodes.Status500InternalServerError,
                        message = "An error occurred while retrieving payments",
                        error = ex.Message
                    }
                );
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetPaymentsDBModel(int id)
        {
            try
            {
                var paymentsDBModel = await _context.PaymentsDBModel.Where(p => p.Id == id && !p.IsDeleted).FirstOrDefaultAsync();

                if (paymentsDBModel == null)
                {
                    return NotFound(
                        new
                        {
                            statusCode = StatusCodes.Status404NotFound,
                            message = "Payment record not found"
                        }
                    );
                }

                return Ok(
                    new
                    {
                        statusCode = StatusCodes.Status200OK,
                        message = "",
                        data = paymentsDBModel
                    }
                 );
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        statusCode = StatusCodes.Status500InternalServerError,
                        message = "An error occurred while retrieving the payment",
                        error = ex.Message
                    }
                 );
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPaymentsDBModel(int id, PaymentsDBModel paymentsDBModel)
        {
            try
            {
                if (id != paymentsDBModel.Id)
                {
                    return BadRequest(new
                    {
                        statusCode = StatusCodes.Status400BadRequest,
                        message = "Inccorect payment record"
                    });
                }

                var existingPayment = await _context.PaymentsDBModel.FindAsync(id);

                if (existingPayment == null)
                {
                    return NotFound(new
                    {
                        statusCode = StatusCodes.Status404NotFound,
                        message = "Payment record not found"
                    });
                }

                if (existingPayment.IsDeleted)
                {
                    return BadRequest(new
                    {
                        statusCode = StatusCodes.Status400BadRequest,
                        message = "Cannot update a deleted payment record"
                    });
                }

                paymentsDBModel.UpdatedAt = DateTime.UtcNow;
                paymentsDBModel.CreatedAt = existingPayment.CreatedAt;
                paymentsDBModel.IsDeleted = existingPayment.IsDeleted;

                _context.Entry(existingPayment).State = EntityState.Detached;
                _context.Entry(paymentsDBModel).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    statusCode = StatusCodes.Status200OK,
                    message = "Payment updated successfully",
                    data = paymentsDBModel
                });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PaymentsDBModelExists(id))
                {
                    return NotFound(new
                    {
                        statusCode = StatusCodes.Status404NotFound,
                        message = "Payment record not found"
                    });
                }

                return StatusCode(
                    StatusCodes.Status409Conflict, 
                    new
                    {
                        statusCode = StatusCodes.Status409Conflict,
                        message = "Concurrency conflict: the record has been modified by another user"
                    }
                );
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError, 
                    new
                    {
                        statusCode = StatusCodes.Status500InternalServerError,
                        message = "An error occurred while updating the payment",
                        error = ex.Message
                    }
                );
            }
        }

        [HttpPost]
        public async Task<ActionResult<object>> PostPaymentsDBModel(PaymentsDBModel paymentsDBModel)
        {
            try
            {
                if (paymentsDBModel == null)
                {
                    return BadRequest(new
                    {
                        statusCode = StatusCodes.Status400BadRequest,
                        message = "Payment data is required"
                    });
                }

                paymentsDBModel.CreatedAt = DateTime.UtcNow;
                paymentsDBModel.UpdatedAt = DateTime.UtcNow;
                paymentsDBModel.IsDeleted = false;

                _context.PaymentsDBModel.Add(paymentsDBModel);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetPaymentsDBModel", new { id = paymentsDBModel.Id }, new
                {
                    statusCode = StatusCodes.Status201Created,
                    message = "Payment created successfully",
                    data = paymentsDBModel
                });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new
                {
                    statusCode = StatusCodes.Status400BadRequest,
                    message = "Invalid payment data provided",
                    error = ex.InnerException?.Message ?? ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    statusCode = StatusCodes.Status500InternalServerError,
                    message = "An error occurred while creating the payment",
                    error = ex.Message
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePaymentsDBModel(int id)
        {
            try
            {
                var paymentsDBModel = await _context.PaymentsDBModel
                    .Where(p => p.Id == id && !p.IsDeleted)
                    .FirstOrDefaultAsync();

                if (paymentsDBModel == null)
                {
                    return NotFound(new
                    {
                        statusCode = StatusCodes.Status404NotFound,
                        message = "Payment record not found or already deleted"
                    });
                }

                paymentsDBModel.IsDeleted = true;
                paymentsDBModel.UpdatedAt = DateTime.UtcNow;
                _context.Entry(paymentsDBModel).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    statusCode = StatusCodes.Status200OK,
                    message = "Payment deleted successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    statusCode = StatusCodes.Status500InternalServerError,
                    message = "An error occurred while deleting the payment",
                    error = ex.Message
                });
            }
        }

        private bool PaymentsDBModelExists(int id)
        {
            return _context.PaymentsDBModel.Any(e => e.Id == id && !e.IsDeleted);
        }
    }
}