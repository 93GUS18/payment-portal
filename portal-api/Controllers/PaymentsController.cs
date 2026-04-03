using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using portal_api.Data;
using portal_api.Models.DbModel;
using portal_api.Models.Model;

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
        public async Task<ActionResult<Response>> GetPaymentsDBModel()
        {
            try
            {
                var payments = await _context.PaymentsDBModel.Where(p => !p.IsDeleted).ToListAsync();

                return new Response(StatusCodes.Status200OK, "", payments, "");
            }
            catch (Exception ex)
            {
                return new Response(StatusCodes.Status500InternalServerError, "An error occurred while retrieving payments", null, ex.Message);
            }
        }

        [HttpGet("next-id")]
        public async Task<ActionResult<Response>> GetNextId()
        {
            try
            {
                var maxId = await _context.PaymentsDBModel.AsNoTracking().MaxAsync(p => (int?)p.Id) ?? 0;
                var nextId = maxId + 1;

                return new Response(StatusCodes.Status200OK, "", nextId, "");
            }
            catch (Exception ex)
            {
                return new Response(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the next ID", null, ex.Message);
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
                    return new Response(StatusCodes.Status404NotFound, "Payment record not found", null, "");
                }

                return new Response(StatusCodes.Status200OK, "", paymentsDBModel, "");
            }
            catch (Exception ex)
            {
                return new Response(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the payment", null, ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Response>> PutPaymentsDBModel(int id, PaymentsDBModel paymentsDBModel)
        {
            try
            {
                if (id != paymentsDBModel.Id)
                {
                    return new Response(StatusCodes.Status400BadRequest, "Inccorect payment record", null, "");
                }

                if (paymentsDBModel.Amount <= 0)
                {
                    return new Response(StatusCodes.Status400BadRequest, "Payment amount must be greater than 0", null, "");
                }

                var existingPayment = await _context.PaymentsDBModel.FindAsync(id);

                if (existingPayment == null)
                {
                    return new Response(StatusCodes.Status404NotFound, "Payment record not found", null, "");
                }

                if (existingPayment.IsDeleted)
                {
                    return new Response(StatusCodes.Status400BadRequest, "Cannot update a deleted payment record", null, "");
                }

                paymentsDBModel.UpdatedAt = DateTime.UtcNow;
                paymentsDBModel.CreatedAt = existingPayment.CreatedAt;
                paymentsDBModel.IsDeleted = existingPayment.IsDeleted;

                _context.Entry(existingPayment).State = EntityState.Detached;
                _context.Entry(paymentsDBModel).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                return new Response(StatusCodes.Status200OK, "Payment updated successfully", paymentsDBModel, "");
            }
            catch (Exception ex)
            {
                return new Response(StatusCodes.Status500InternalServerError, "An error occurred while updating the payment", null, ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<Response>> PostPaymentsDBModel(PaymentsDBModel paymentsDBModel)
        {
            try
            {
                if (paymentsDBModel == null)
                {
                    return new Response(StatusCodes.Status400BadRequest, "Payment data is required", null, "");
                }

                if (paymentsDBModel.Amount <= 0)
                {
                    return new Response(StatusCodes.Status400BadRequest, "Payment amount must be greater than 0", null, "");
                }

                paymentsDBModel.CreatedAt = DateTime.UtcNow;
                paymentsDBModel.UpdatedAt = DateTime.UtcNow;
                paymentsDBModel.IsDeleted = false;

                _context.PaymentsDBModel.Add(paymentsDBModel);
                await _context.SaveChangesAsync();

                return new Response(StatusCodes.Status201Created, "Payment created successfully", paymentsDBModel, "");
            }
            catch (DbUpdateException ex)
            {
                return new Response(StatusCodes.Status400BadRequest, "Invalid payment data provided", null, ex.InnerException?.Message ?? ex.Message);
            }
            catch (Exception ex)
            {
                return new Response(StatusCodes.Status500InternalServerError, "An error occurred while creating the payment", null, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Response>> DeletePaymentsDBModel(int id)
        {
            try
            {
                var paymentsDBModel = await _context.PaymentsDBModel.Where(p => p.Id == id && !p.IsDeleted).FirstOrDefaultAsync();

                if (paymentsDBModel == null)
                {
                    return new Response(StatusCodes.Status404NotFound, "Payment record not found or already deleted", null, "");
                }

                paymentsDBModel.IsDeleted = true;
                paymentsDBModel.UpdatedAt = DateTime.UtcNow;
                _context.Entry(paymentsDBModel).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                return new Response(StatusCodes.Status200OK, "Payment deleted successfully", null, "");
            }
            catch (Exception ex)
            {
                return new Response(StatusCodes.Status500InternalServerError, "An error occurred while deleting the payment", null, ex.Message);
            }
        }

        private bool PaymentsDBModelExists(int id)
        {
            return _context.PaymentsDBModel.Any(e => e.Id == id && !e.IsDeleted);
        }
    }
}