using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

using AutoMapper;
using MagicVilla_VillaAPI.Repository.IRepository;
using System.Net;

namespace MagicVilla_VillaAPI.Controllers
{
    //[Route("api/[controller]")]
    [Route("VillaNumberApi")]
    [ApiController]
    public class VillaNumberAPIController : ControllerBase
    {
        private ILogger<VillaAPIController> _logger;
        private readonly IVillaNumberRepository _dbVillaNumber;
        private readonly ApplicationDbContext _db;

        private readonly IMapper _mapper;
        protected APIResponse _apiResponse;
        private readonly IVillaRepository _dbVilla;

        public VillaNumberAPIController(ILogger<VillaAPIController> logger, IVillaNumberRepository dbVillaNumber, IMapper mapper, IVillaRepository dbVilla)
        {
            _logger = logger;
            _dbVillaNumber = dbVillaNumber;
            _mapper = mapper;
            this._apiResponse = new();
            _dbVilla = dbVilla;

        }

        [HttpGet]
        public async Task<ActionResult<APIResponse>> GetVillaNumbers()
        {
            try
            {
                _logger.LogInformation("Getting All villas");
                //return Ok(VillaStore.villaList); // this is via VillaStore
                //return Ok(await _db.Villas.ToListAsync());

                //using automapper
                // IEnumerable<VillaNumber> villaNumberList = await _db.VillaNumber.ToListAsync();
                IEnumerable<VillaNumber> villaNumberList = await _dbVillaNumber.GetAllAsync();

                //return Ok(_mapper.Map<List<VillaDTO>>(villaList));
                // with API response , IEn to Apiresp in return type
                _apiResponse.Result = _mapper.Map<List<VillaNumberDTO>>(villaNumberList);
                _apiResponse.StatusCode = HttpStatusCode.OK;
                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessage = new List<string> { ex.ToString() };

            }
            return _apiResponse;

        }


        //[HttpGet("id")]
        [HttpGet("{id:int}", Name = "GetVillaNumber")]
        //[ProducesResponseTypeAttribute(200, Type=typeof(VillaDTO) )] // then we can remove the return type in endpoint method
        //[ProducesResponseTypeAttribute(404)]
        //[ProducesResponseTypeAttribute(400)]
        [ProducesResponseTypeAttribute(StatusCodes.Status200OK)]
        [ProducesResponseTypeAttribute(StatusCodes.Status400BadRequest)]
        [ProducesResponseTypeAttribute(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetVillaNumber(int id)
        {
            try
            {
                if (id == 0)
                {
                    // _logger.LogError("Get villa error with id " + id);
                    // return BadRequest();

                    //with api resp
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_apiResponse);
                }

                //var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
                var villaNumber = await _dbVillaNumber.GetAsync(u => u.VillaNo == id);
                if (villaNumber == null)
                {
                    //return NotFound();
                    //with api resp
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_apiResponse);
                }
                //return Ok(villa);
                //
                //return Ok(_mapper.Map<VillaDTO>(villa));
                //replace above with apiresp return type

                _apiResponse.Result = _mapper.Map<VillaNumberDTO>(villaNumber);
                _apiResponse.StatusCode = HttpStatusCode.OK;
                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessage = new List<string> { ex.ToString() };

            }
            return _apiResponse;
        }

        [HttpPost]
        public async Task<ActionResult<APIResponse>> CreateVillaNumber([FromBody] VillaNumberCreateDTO createdto)
        {
            try
            {
                //Custome model state validation via ModelState.AddModelError("", "msg");

                //if (VillaStore.villaList.FirstOrDefault(u => u.Name.ToLower() == createdto.Name.ToLower()) != null)
                                if (await _dbVillaNumber.GetAsync(u => u.VillaNo == createdto.VillaNo) != null)
                {
                    ModelState.AddModelError("Custom error", "Villa number already exists");
                    return BadRequest(ModelState);
                }

                // foriegn key added to villanumber, so check if the villaid exists in villa table,so inject Villa class and check
                if (await _dbVilla.GetAsync(u => u.Id == createdto.VillaId) == null){
                    return BadRequest(ModelState);
                }



                if (createdto == null)
                {
                    return BadRequest(createdto);
                }
                //when creating id should be 0
                //if (createdto.Id > 0) // no need now , we use CreateVillaDto
                //{
                //    return StatusCode(StatusCodes.Status500InternalServerError);
                //}
                ////get increment id and add to that
                //createdto.Id = VillaStore.villaList.OrderByDescending(u => u.Id).FirstOrDefault().Id + 1;
                ////finally add villa obj to store
                //VillaStore.villaList.Add(createdto);

                //replace abvoe with EF core
                // convert createdto to villa first and then add, ie manually map createdto obj to villa model
                //Villa model = new Villa()
                //{
                //    Amenity = createdto.Amenity,
                //    Details = createdto.Details,
                //                  ImageUrl = createdto.ImageUrl,
                //    Name = createdto.Name,
                //    Occupancy = createdto.Occupancy,
                //    Rate = createdto.Rate,
                //    Sqft = createdto.Sqft
                //};
                //using Auto mapper
                VillaNumber villaNumber = _mapper.Map<VillaNumber>(createdto);

                //await _db.Villas.AddAsync(villa); // to add call save changes
                //await _db.SaveChangesAsync();

                await _dbVillaNumber.CreateAsync(villaNumber);


                //finally 
                //return Ok(createdto);
                // use CreatedAtRoute
                //return CreatedAtRoute("GetVilla", new { id = createdto.Id }, createdto);
                // return CreatedAtRoute("GetVilla", new { id = villa.Id }, villa);

                //above line with apiresp
                _apiResponse.Result = _mapper.Map<VillaNumberDTO>(villaNumber);
                _apiResponse.StatusCode = HttpStatusCode.Created;
                return CreatedAtRoute("GetVilla", new { id = villaNumber.VillaNo }, _apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessage = new List<string> { ex.ToString() };

            }
            return _apiResponse;
        }


        [ProducesResponseType(StatusCodes.Status204NoContent)]
        //[ProducesResponseTypeAttribute(404)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("{id:int}", Name = "DeleteVillaNumber")]
        public async Task<ActionResult<APIResponse>> DeleteVillaNumber(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }
                var villaNumber = await _dbVillaNumber.GetAsync(u => u.VillaNo == id);
                if (villaNumber == null)
                {
                    return NotFound();
                }
                //VillaStore.villaList.Remove(villa);
                //await _db.SaveChangesAsync();

                await _dbVillaNumber.RemoveAsync(villaNumber);
                //return NoContent();
                //with apiresponse

                _apiResponse.StatusCode = HttpStatusCode.NoContent;
                _apiResponse.IsSuccess = true;
                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessage = new List<string> { ex.ToString() };

            }
            return _apiResponse;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseTypeAttribute(204)]        
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut("{id:int}", Name = "UpdateVillaNumber")]
        public async Task<ActionResult<APIResponse>> UpdateVillaNumber(int id, [FromBody] VillaNumberUpdateDTO updateDTO)
        {
            try
            {
                if (updateDTO == null || id != updateDTO.VillaNo)
                {
                    return BadRequest(updateDTO);
                }


                // foriegn key added to villanumber, so check if the villaid exists in villa table,so inject Villa class and check
                if (await _dbVilla.GetAsync(u => u.Id == updateDTO.VillaId) == null)
                {
                    return BadRequest(ModelState);
                }

                //based on id get the villa and then update
                //var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
                //villa.Name = updateDTO.Name;
                //villa.Occupancy = updateDTO.Occupancy;
                //villa.Sqft = updateDTO.Sqft;


                // Replace above to use db ctxt
                // convert updateDTO to villa first and then add, ie manually map updateDTO obj to villa model
                //Villa model = new Villa()
                //{
                //    Amenity = updateDTO.Amenity,
                //    Details = updateDTO.Details,
                //    Id = updateDTO.Id,
                //    ImageUrl = updateDTO.ImageUrl,
                //    Name = updateDTO.Name,
                //    Occupancy = vdto.Occupancy,
                //    Rate = vupdateDTOdto.Rate,
                //    Sqft = updateDTO.Sqft
                //};
                // update dto to villa model using Auto Mapper
                VillaNumber model = _mapper.Map<VillaNumber>(updateDTO);

                //EF is smart based on id it will figure out what record to udpate
                // var villa =  _db.Villas.Update(model);
                //await _db.SaveChangesAsync();
                // with repository
                //var villa = _dbVilla.UpdateAsync(model);
                await _dbVillaNumber.UpdateAsync(model);
                //return NoContent();
                _apiResponse.StatusCode = HttpStatusCode.NoContent;
                _apiResponse.IsSuccess = true;
                return Ok(_apiResponse);

            }
            catch (Exception ex)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessage = new List<string> { ex.ToString() };
            }
            return _apiResponse;
        }

        ////gi ti jsonpatch .com for detaisl
        //[HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        //[ProducesResponseType(StatusCodes.Status204NoContent)]
        ////[ProducesResponseTypeAttribute(204)]        
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //public IActionResult UpdatePartialVilla(int id, JsonPatchDocument<VillaDTO> patchDto)
        //{
        //    if (patchDto == null || id == 0)
        //    {
        //        return BadRequest();
        //    }

        //    var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
        //    if (villa == null)
        //    {
        //        return BadRequest(); // or NotFound()
        //    }

        //    patchDto.ApplyTo(villa, ModelState);
        //    //if model state is not valid
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }
        //    return NoContent();
        //}
        //above is converted using db context



        //gi ti jsonpatch .com for detaisl
        [HttpPatch("{id:int}", Name = "UpdatePartialVillaNumber")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseTypeAttribute(204)]        
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePartialVillaNumber(int id, JsonPatchDocument<VillaNumberUpdateDTO> patchDto)
        {
            if (patchDto == null || id == 0)
            {
                return BadRequest();
            }

            //var villa = await _db.Villas.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
            //above using repository
            var villa = await _dbVillaNumber.GetAsync(u => u.VillaNo == id, tracked: false);

            //convert villa to vdto
            //VillaUpdateDTO villaDTO = new()
            //{
            //    Amenity = villa.Amenity,
            //    Details = villa.Details,
            //    Id = villa.Id,
            //    ImageUrl = villa.ImageUrl,
            //    Name = villa.Name,
            //    Occupancy = villa.Occupancy,
            //    Rate = villa.Rate,
            //    Sqft = villa.Sqft
            //};
            // using automapper convert villa to VillaUpdateDTO/destination
            VillaNumberUpdateDTO villaNumberDTO = _mapper.Map<VillaNumberUpdateDTO>(villa);

            if (villa == null)
            {
                return BadRequest(); // or NotFound()
            }

            patchDto.ApplyTo(villaNumberDTO, ModelState);
            //convert to Villadto back to villa , bcz db is of villa type
            //Villa model = new Villa()
            //{
            //    Amenity = villaDTO.Amenity,
            //    Details = villaDTO.Details,
            //    Id = villaDTO.Id,
            //    ImageUrl = villaDTO.ImageUrl,
            //    Name = villaDTO.Name,
            //    Occupancy = villaDTO.Occupancy,
            //    Rate = villaDTO.Rate,
            //    Sqft = villaDTO.Sqft
            //};
            //VillaUpdateDTO to villa mode;using automapper
            VillaNumber model = _mapper.Map<VillaNumber>(villaNumberDTO);

            //_db.Villas.Update(model);
            //await _db.SaveChangesAsync();
            // above using repo

            await _dbVillaNumber.UpdateAsync(model);

            //if model state is not valid
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return NoContent();
        }
    }
}
