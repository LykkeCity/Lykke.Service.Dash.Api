﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Common;
using Common.Log;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.BlockchainApi.Contract;
using Lykke.Service.BlockchainApi.Contract.Balances;
using Lykke.Service.Dash.Api.Core.Repositories;
using Lykke.Service.Dash.Api.Helpers;
using Lykke.Service.Dash.Api.Services;
using Lykke.Common.Log;

namespace Lykke.Service.Dash.Api.Controllers
{
    [Route("api/balances")]
    public class BalancesController : Controller
    {
        private readonly ILog _log;
        private readonly IDashService _dashService;
        private readonly IBalanceRepository _balanceRepository;
        private readonly IBalancePositiveRepository _balancePositiveRepository;

        public BalancesController(ILogFactory logFactory,
            IDashService dashService,
            IBalanceRepository balanceRepository,
            IBalancePositiveRepository balancePositiveRepository)
        {
            _log = logFactory.CreateLog(this);
            _dashService = dashService;
            _balanceRepository = balanceRepository;
            _balancePositiveRepository = balancePositiveRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get([Required, FromQuery] string take, [FromQuery] string continuation)
        {
            if (!int.TryParse(take, out var takeInt))
            {
                ModelState.AddModelError(nameof(take), "Should be integer.");

                return BadRequest(ModelState);
            }

            var result = await _balancePositiveRepository.GetAsync(takeInt, continuation);
            
            return Ok(PaginationResponse.From(
                result.ContinuationToken, 
                result.Entities.Select(f => f.ToWalletBalanceContract()).ToArray()));
        }

        [HttpPost("{address}/observation")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> AddToObservations([Required] string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                return BadRequest(ErrorResponse.Create($"{nameof(address)} is null or empty"));
            }

            var validAddress = _dashService.GetBitcoinAddress(address) != null;
            if (!validAddress)
            {
                return BadRequest(ErrorResponse.Create($"{nameof(address)} is not valid"));
            }

            var balance = await _balanceRepository.GetAsync(address);
            if (balance != null)
            {
                return new StatusCodeResult(StatusCodes.Status409Conflict);
            }

            _log.Info("Add address to observations", new { address = address });

            await _balanceRepository.AddAsync(address);

            return Ok();
        }

        [HttpDelete("{address}/observation")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteFromObservations([Required] string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                return BadRequest(ErrorResponse.Create($"{nameof(address)} is null or empty"));
            }

            var balance = await _balanceRepository.GetAsync(address);
            if (balance == null)
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }

            _log.Info("Delete address from observations", new { address = address });

            await _balancePositiveRepository.DeleteAsync(address);
            await _balanceRepository.DeleteAsync(address);

            return Ok();
        }
    }
}
