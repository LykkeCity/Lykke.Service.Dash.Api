using Lykke.Service.BlockchainApi.Contract;
using Lykke.Service.BlockchainApi.Contract.Assets;
using Lykke.Service.Dash.Api.Core.Domain;
using Lykke.Service.Dash.Api.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Lykke.Service.Dash.Api.Controllers
{
    [Route("api/assets")]
    public class AssetsController : Controller
    {
        [HttpGet]
        public IActionResult Get([Required, FromQuery] string take, [FromQuery] string continuation)
        {
            if (!int.TryParse(take, out var takeInt))
            {
                ModelState.AddModelError(nameof(take), "Should be integer.");

                return BadRequest(ModelState);
            }

            var assets = new AssetResponse[] { Asset.Dash.ToAssetResponse() };

            return Ok(PaginationResponse.From("", assets));
        }

        [HttpGet("{assetId}")]
        [ProducesResponseType(typeof(AssetResponse), StatusCodes.Status200OK)]
        public IActionResult GetAsset([Required] string assetId)
        {
            if(Asset.Dash.Id != assetId)
            {
                return NoContent();
            }

            return Ok(Asset.Dash.ToAssetResponse());
        }
    }
}
