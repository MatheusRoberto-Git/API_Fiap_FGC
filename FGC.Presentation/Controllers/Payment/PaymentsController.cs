using FGC.Application.PaymentManagement.DTOs;
using FGC.Application.PaymentManagement.UseCases;
using FGC.Presentation.Models.Requests.Payment;
using FGC.Presentation.Models.Responses.Payment;
using FGC.Presentation.Models.Responses.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FGC.Presentation.Controllers.Payment
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        #region [Contructor]

        private readonly CreatePaymentUseCase _createPaymentUseCase;
        private readonly ProcessPaymentUseCase _processPaymentUseCase;
        private readonly GetPaymentStatusUseCase _getPaymentStatusUseCase;
        private readonly GetPaymentByIdUseCase _getPaymentByIdUseCase;
        private readonly GetUserPaymentsUseCase _getUserPaymentsUseCase;
        private readonly RefundPaymentUseCase _refundPaymentUseCase;

        public PaymentsController(CreatePaymentUseCase createPaymentUseCase, ProcessPaymentUseCase processPaymentUseCase, GetPaymentStatusUseCase getPaymentStatusUseCase, GetPaymentByIdUseCase getPaymentByIdUseCase, GetUserPaymentsUseCase getUserPaymentsUseCase, RefundPaymentUseCase refundPaymentUseCase)
        {
            _createPaymentUseCase = createPaymentUseCase;
            _processPaymentUseCase = processPaymentUseCase;
            _getPaymentStatusUseCase = getPaymentStatusUseCase;
            _getPaymentByIdUseCase = getPaymentByIdUseCase;
            _getUserPaymentsUseCase = getUserPaymentsUseCase;
            _refundPaymentUseCase = refundPaymentUseCase;
        }

        #endregion

        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<PaymentResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<PaymentResponse>>> Create([FromBody] CreatePaymentRequest request)
        {
            try
            {
                var validationError = ValidateCreateRequest(request);
                if (validationError != null) return validationError;

                var dto = new CreatePaymentDTO
                {
                    UserId = request.UserId,
                    GameId = request.GameId,
                    Amount = request.Amount,
                    PaymentMethod = request.PaymentMethod
                };

                var result = await _createPaymentUseCase.ExecuteAsync(dto);
                var response = MapToResponse(result);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = result.Id },
                    ApiResponse<PaymentResponse>.SuccessMethod(response, "Pagamento criado com sucesso")
                );
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorMethod(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorMethod($"Erro interno do servidor - {ex.Message}"));
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<PaymentResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<PaymentResponse>>> GetById(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return BadRequest(ApiResponse<object>.ErrorMethod("ID inválido"));

                var payment = await _getPaymentByIdUseCase.ExecuteAsync(id);
                var response = MapToResponse(payment);

                return Ok(ApiResponse<PaymentResponse>.SuccessMethod(response));
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ApiResponse<object>.ErrorMethod(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorMethod($"Erro interno do servidor - {ex.Message}"));
            }
        }

        [HttpGet("{id}/status")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<PaymentStatusResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<PaymentStatusResponse>>> GetStatus(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return BadRequest(ApiResponse<object>.ErrorMethod("ID inválido"));

                var status = await _getPaymentStatusUseCase.ExecuteAsync(id);
                var response = new PaymentStatusResponse
                {
                    PaymentId = status.PaymentId,
                    TransactionId = status.TransactionId,
                    Status = status.Status,
                    LastUpdated = status.LastUpdated
                };

                return Ok(ApiResponse<PaymentStatusResponse>.SuccessMethod(response));
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ApiResponse<object>.ErrorMethod(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorMethod($"Erro interno do servidor - {ex.Message}"));
            }
        }

        [HttpGet("user/{userId}")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<PaymentResponse>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<IEnumerable<PaymentResponse>>>> GetUserPayments(Guid userId)
        {
            try
            {
                if (userId == Guid.Empty)
                    return BadRequest(ApiResponse<object>.ErrorMethod("UserId inválido"));

                var payments = await _getUserPaymentsUseCase.ExecuteAsync(userId);
                var response = payments.Select(MapToResponse);

                return Ok(ApiResponse<IEnumerable<PaymentResponse>>.SuccessMethod(response, $"Encontrados {response.Count()} pagamentos"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorMethod(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorMethod($"Erro interno do servidor - {ex.Message}"));
            }
        }

        [HttpPost("{id}/process")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<PaymentResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<PaymentResponse>>> Process(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return BadRequest(ApiResponse<object>.ErrorMethod("ID inválido"));

                var dto = new ProcessPaymentDTO { PaymentId = id };
                var result = await _processPaymentUseCase.ExecuteAsync(dto);
                var response = MapToResponse(result);

                var message = result.Status == "Completed"
                    ? "Pagamento processado com sucesso"
                    : $"Pagamento falhou: {result.FailureReason}";

                return Ok(ApiResponse<PaymentResponse>.SuccessMethod(response, message));
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ApiResponse<object>.ErrorMethod(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorMethod($"Erro interno do servidor - {ex.Message}"));
            }
        }

        [HttpPost("{id}/refund")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<PaymentResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<PaymentResponse>>> Refund(Guid id, [FromBody] RefundPaymentRequest request)
        {
            try
            {
                if (id == Guid.Empty)
                    return BadRequest(ApiResponse<object>.ErrorMethod("ID inválido"));

                var dto = new RefundPaymentDTO
                {
                    PaymentId = id,
                    Reason = request?.Reason ?? string.Empty
                };

                var result = await _refundPaymentUseCase.ExecuteAsync(dto);
                var response = MapToResponse(result);

                return Ok(ApiResponse<PaymentResponse>.SuccessMethod(response, "Reembolso processado com sucesso"));
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ApiResponse<object>.ErrorMethod(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorMethod($"Erro interno do servidor - {ex.Message}"));
            }
        }

        #region [Private Helpers]

        private ActionResult<ApiResponse<PaymentResponse>> ValidateCreateRequest(CreatePaymentRequest request)
        {
            if (request == null)
                return BadRequest(ApiResponse<object>.ErrorMethod("Dados obrigatórios"));

            if (request.UserId == Guid.Empty)
                return BadRequest(ApiResponse<object>.ErrorMethod("UserId é obrigatório"));

            if (request.GameId == Guid.Empty)
                return BadRequest(ApiResponse<object>.ErrorMethod("GameId é obrigatório"));

            if (request.Amount <= 0)
                return BadRequest(ApiResponse<object>.ErrorMethod("Valor deve ser maior que zero"));

            return null;
        }

        private static PaymentResponse MapToResponse(PaymentResponseDTO dto)
        {
            return new PaymentResponse
            {
                Id = dto.Id,
                UserId = dto.UserId,
                GameId = dto.GameId,
                Amount = dto.Amount,
                Status = dto.Status,
                Method = dto.Method,
                TransactionId = dto.TransactionId,
                CreatedAt = dto.CreatedAt,
                ProcessedAt = dto.ProcessedAt,
                CompletedAt = dto.CompletedAt,
                FailureReason = dto.FailureReason
            };
        }

        #endregion
    }
}
