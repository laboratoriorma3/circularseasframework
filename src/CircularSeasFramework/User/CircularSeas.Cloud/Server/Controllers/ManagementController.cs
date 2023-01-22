using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CircularSeas.Adapters;
using CircularSeas.Models;
using CircularSeas.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CircularSeas.Cloud.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ManagementController : Controller
    {
        private readonly IDbService dbService;

        public ManagementController(IDbService dbService)
        {
            this.dbService = dbService;
        }

        #region GETs
        /// <summary>
        /// Get a list of the materials with the data of their properties
        /// </summary>
        /// <param name="includeProperties">Boolean to include data of non-visible properties</param>
        /// <param name="nodeStock">GUID of the stock of the node</param>
        /// <param name="forUsers">Boolean to indicate if it requested by an user or a service provider</param>
        /// <returns></returns>
        [HttpGet("materials")]
        public async Task<IActionResult> GetMaterials([FromQuery] bool includeProperties = true, [FromQuery] Guid nodeStock = default(Guid), bool forUsers = false)
        {
            try
            {
                var result = await dbService.GetMaterials(includeProperties, nodeStock, forUsers);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Get the data of an specific material
        /// </summary>
        /// <param name="materialId">GUID Identifier of the material</param>
        /// <returns></returns>
        [HttpGet("material/detail/{materialId}")]
        public async Task<IActionResult> GetMaterialDetail([FromRoute] Guid materialId)
        {
            try
            {
                var result = await dbService.GetMaterialDetail(materialId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
                throw;
            }

        }

        /// <summary>
        /// Get a list of the values of each propertie
        /// </summary>
        /// <returns></returns>
        [HttpGet("properties")]
        public async Task<IActionResult> GetProperties()
        {
            try
            {
                var result = await dbService.GetProperties();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Get a list of the values of a specific property
        /// </summary>
        /// <param name="propertyId">GUID of the property</param>
        /// <returns></returns>
        [HttpGet("property/detail/{propertyId}")]
        public async Task<IActionResult> GetPropertyDetail([FromRoute] Guid propertyId)
        {
            try
            {
                var result = await dbService.GetPropertyDetail(propertyId);
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
                throw;
            }
        }

        /// <summary>
        /// Get a list of materials with their related properties
        /// </summary>
        /// <returns></returns>
        [HttpGet("material/schema")]
        public async Task<IActionResult> GetMaterialSchema()
        {
            try
            {
                var schema = await dbService.GetMaterialSchema();
                return Ok(schema);
            }
            catch (Exception ex)
            {
                BadRequest(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Get a list of orders
        /// </summary>
        /// <param name="status">0: all, 1:pending, 2: delivering, 3:finished</param>
        /// <param name="nodeId">GUID of the node that request the information</param>
        /// <returns></returns>
        [HttpGet("order/list")]
        public async Task<IActionResult> GetOrders([FromQuery] int status = 0, [FromQuery] Guid nodeId = default(Guid))
        {
            //Status: 0: all, 1:pending, 2: delivering, 3:finished
            try
            {
                var orders = await dbService.GetOrders(status, nodeId);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                BadRequest(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Get a list with the data of each node
        /// </summary>
        /// <returns></returns>
        [HttpGet("nodes/list")]
        public async Task<IActionResult> GetNodes()
        {
            try
            {
                var nodes = await dbService.GetNodes();
                return Ok(nodes);
            }
            catch (Exception ex)
            {
                BadRequest(ex.Message);
                throw;
            }
        }
        #endregion


        #region POSTs
        /// <summary>
        /// Create a new property to consider in the Material-Helping section of the user application
        /// </summary>
        /// <param name="property">Object that represents a property</param>
        /// <returns></returns>
        [HttpPost("property/new")]
        public async Task<IActionResult> PostProperty([FromBody] Models.Property property)
        {
            try
            {
                var created = await dbService.CreateProperty(property);
                return Ok(created);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
                throw;
            }
        }

        /// <summary>
        /// Create a new material
        /// </summary>
        /// <param name="material">Object that represents a property</param>
        /// <returns></returns>
        [HttpPost("material/new")]
        public async Task<IActionResult> PostMaterial([FromBody] Models.Material material)
        {
            try
            {
                var created = await dbService.CreateMaterial(material);
                return Ok(created);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Create a new order
        /// </summary>
        /// <param name="order">Object that represents an order</param>
        /// <returns></returns>
        [HttpPost("order/new")]
        public async Task<IActionResult> PostOrder([FromBody] Models.Order order)
        {
            try
            {
                var created = await dbService.CreateOrder(order);
                return Ok(created);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Add new settings of the Cloud CAM
        /// </summary>
        /// <param name="file">Bundle file with the whole configuration of materials, profiles, printers and compatibilities (.INI) </param>
        /// <returns></returns>
        [HttpPost("settings/bundle-file")]
        public async Task<IActionResult> PostSettingsBundle([FromForm] IFormFile file)
        {
            try
            {
                using (var ms = new MemoryStream())
                {
                    await file.CopyToAsync(ms);
                    await dbService.ProcessSettingsBundle(ms);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest();
                throw;
            }
        }

        /// <summary>
        /// Add new settings of the Cloud CAM
        /// </summary>
        /// <param name="bundle">List that represents the materials, profiles, printers and compatibilities (.INI) </param>
        /// <returns></returns>
        [HttpPost("settings/bundle-lines")]
        public async Task<IActionResult> PostSettingsLines([FromBody] ConfigDTO bundle)
        {
            try
            {
                await dbService.ProcessSettingsBundle(bundle.Lines, bundle.Matching);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest();
                throw;
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("settings/match-materials")]
        public async Task<IActionResult> GetFilamentsCandidates([FromBody] Dictionary<Guid, Guid> matching)
        {
            try
            {
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
                throw;
            }
        }
        #endregion

        #region PUTs
        /// <summary>
        /// Update the visibility of a property for an user in the Material-Helping section
        /// </summary>
        /// <param name="propertyId">GUID of the selected property</param>
        /// <param name="visible">Value of the visibility of the selected property</param>
        /// <returns></returns>
        [HttpPut("property/visibility/{propertyId}/{visible}")]
        public async Task<IActionResult> PutPropertyVisibility([FromRoute] Guid propertyId, [FromRoute] bool visible)
        {
            try
            {
                var incorrectMaterials = await dbService.CheckBadMaterialsVisible(propertyId);
                incorrectMaterials = null; //TODO
                if (incorrectMaterials == null)
                {
                    await dbService.UpdateVisibility(propertyId, visible);
                    return Ok("Bieeenn");
                }
                else
                {
                    return Conflict(incorrectMaterials);
                }
            }
            catch (Exception ex)
            {
                throw;
                return BadRequest(ex.Message);

            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPut("material/update-properties")]
        public async Task<IActionResult> PutChangeMaterialProperties([FromBody] Models.Material material)
        {
            try
            {
                await dbService.UpdateMaterialEvaluations(material);
                return NoContent();
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
                throw;
            }

            return NoContent();
        }

        /// <summary>
        /// Update the value of any property of a material (evaluation)
        /// </summary>
        /// <param name="material">Object of a material with the updated data of the evaluation of its properties</param>
        /// <returns></returns>
        [HttpPut("material/update-material")]
        public async Task<IActionResult> PutUpdateMaterial([FromBody] Models.Material material)
        {
            try
            {
                await dbService.UpdateMaterial(material);
                return NoContent();
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
                throw;
            }
        }

        /// <summary>
        ///  Update the value of any property of a material (evaluation)
        /// </summary>
        /// <param name="property">Object with the updated values of a property</param>
        /// <returns></returns>
        [HttpPut("property/update-property")]
        public async Task<IActionResult> PutUpdateProperty([FromBody] Models.Property property)
        {
            try
            {
                await dbService.UpdateProperty(property);
                return NoContent();
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Update the value of any property of an order 
        /// </summary>
        /// <param name="order">Object with the updated values of an order</param>
        /// <returns></returns>
        [HttpPut("order/update")]
        public async Task<IActionResult> PutUpdateOrder([FromBody] Models.Order order)
        {
            try
            {
                var updated = await dbService.UpdateOrder(order);
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Mark an order as received or not
        /// </summary>
        /// <param name="orderId">GUID of an order</param>
        /// <returns></returns>
        [HttpPut("order/mark-received/{orderId}")]
        public async Task<IActionResult> PutReceiveOrder([FromRoute] Guid orderId)
        {
            try
            {
                var order = await dbService.GetOrder(orderId);
                if (order.FinishedDate != null)
                {
                    return BadRequest("Order is already mark as finished");
                }
                else if (order.ShippingDate == null)
                {
                    return BadRequest("Can't mark as received a order which hasn't already sended");
                }
                order.FinishedDate = DateTime.Now;
                var updated = await dbService.UpdateOrder(order);
                var stockUpdated = await dbService.UpdateStock(order);

                return Ok(stockUpdated);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Mark a filament spool as finished
        /// </summary>
        /// <param name="nodeId">GUID of the node</param>
        /// <param name="materialId">GUID of the material</param>
        /// <param name="amount">Value of the new amount of filament spools</param>
        /// <returns></returns>
        [HttpPut("order/mark-spended/{nodeId}/{materialId}/{amount}")]
        public async Task<IActionResult> PutSpendSpool([FromRoute] Guid nodeId, [FromRoute] Guid materialId, [FromRoute] int amount)
        {
            try
            {
                var stockUpdated = await dbService.UpdateStock(nodeId, materialId, amount);
                if (stockUpdated == null) return NotFound();

                return Ok(stockUpdated);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
                throw;
            }
        }
        #endregion

        #region DELETE
        /// <summary>
        /// Delete a property of the BBDD
        /// </summary>
        /// <param name="propertyId">GUID of a property</param>
        /// <returns></returns>
        [HttpDelete("property/delete/{propertyId}")]
        public async Task<IActionResult> DeleteProperty([FromRoute] Guid propertyId)
        {
            try
            {
                await dbService.DeleteProperty(propertyId);
                return NoContent();
            }
            catch (Exception ex)
            {
                BadRequest(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Delete a material of the BBDD
        /// </summary>
        /// <param name="materialId">GUID of a material</param>
        /// <returns></returns>
        [HttpDelete("material/delete/{materialId}")]
        public async Task<IActionResult> DeleteMaterial([FromRoute] Guid materialId)
        {
            try
            {
                await dbService.DeleteMaterial(materialId);
                return NoContent();
            }
            catch (Exception ex)
            {
                BadRequest(ex.Message);
                throw;
            }
        }
        #endregion
    }
}
