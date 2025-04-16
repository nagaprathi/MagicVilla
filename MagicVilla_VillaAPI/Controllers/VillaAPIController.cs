using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

using MagicVilla_VillaAPI.Repository.IRepository;
using System.Net;

namespace MagicVilla_VillaAPI.Controllers
{
    //[Route("api/[controller]")]
    [Route("VillaApi")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        private ILogger<VillaAPIController> _logger;
        private readonly IVillaRepository _dbVilla;
        private readonly ApplicationDbContext _db;
        
        private readonly IMapper _mapper;
        protected APIResponse _apiResponse;

        public VillaAPIController(ILogger<VillaAPIController> logger, IVillaRepository dbVilla, IMapper mapper)
        {
            _logger = logger;
            _dbVilla = dbVilla;
            _mapper = mapper;
            this._apiResponse = new();

        }

        [HttpGet]
        public async Task<ActionResult<APIResponse>> GetVillas()
        {
            try
            {
                _logger.LogInformation("Getting All villas");
                //return Ok(VillaStore.villaList); // this is via VillaStore
                //return Ok(await _db.Villas.ToListAsync());

                //using automapper
                // IEnumerable<Villa> villaList = await _db.Villas.ToListAsync();
                IEnumerable<Villa> villaList = await _dbVilla.GetAllAsync();

                //return Ok(_mapper.Map<List<VillaDTO>>(villaList));
                // with API response , IEn to Apiresp in return type
                _apiResponse.Result = _mapper.Map<List<VillaDTO>>(villaList);
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
        [HttpGet("{id:int}", Name = "GetVilla")]
        //[ProducesResponseTypeAttribute(200, Type=typeof(VillaDTO) )] // then we can remove the return type in endpoint method
        //[ProducesResponseTypeAttribute(404)]
        //[ProducesResponseTypeAttribute(400)]
        [ProducesResponseTypeAttribute(StatusCodes.Status200OK)]
        [ProducesResponseTypeAttribute(StatusCodes.Status400BadRequest)]
        [ProducesResponseTypeAttribute(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetVilla(int id)
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
                var villa = await _dbVilla.GetAsync(u => u.Id == id);
                if (villa == null)
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

                _apiResponse.Result = _mapper.Map<VillaDTO>(villa);
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
        public async Task<ActionResult<APIResponse>> CreateVilla([FromBody] VillaCreateDTO createdto)
        {
            try
            {
                //Custome model state validation via ModelState.AddModelError("", "msg");

                //if (VillaStore.villaList.FirstOrDefault(u => u.Name.ToLower() == createdto.Name.ToLower()) != null)
                if (await _dbVilla.GetAsync(u => u.Name.ToLower() == createdto.Name.ToLower()) != null)
                {
                    ModelState.AddModelError("", "Villa already exists");
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
                Villa villa = _mapper.Map<Villa>(createdto);

                //await _db.Villas.AddAsync(villa); // to add call save changes
                //await _db.SaveChangesAsync();

                await _dbVilla.CreateAsync(villa);


                //finally 
                //return Ok(createdto);
                // use CreatedAtRoute
                //return CreatedAtRoute("GetVilla", new { id = createdto.Id }, createdto);
                // return CreatedAtRoute("GetVilla", new { id = villa.Id }, villa);

                //above line with apiresp
                _apiResponse.Result = _mapper.Map<VillaDTO>(villa);
                _apiResponse.StatusCode = HttpStatusCode.Created;
                return CreatedAtRoute("GetVilla", new { id = villa.Id }, _apiResponse);
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
        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        public async Task<ActionResult<APIResponse>> DeleteVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }
                var villa = await _dbVilla.GetAsync(u => u.Id == id);
                if (villa == null)
                {
                    return NotFound();
                }
                //VillaStore.villaList.Remove(villa);
                //await _db.SaveChangesAsync();

                await _dbVilla.RemoveAsync(villa);
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

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        //[ProducesResponseTypeAttribute(204)]        
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut("{id:int}", Name = "UpdateVilla")]
        public async Task<ActionResult<APIResponse>> UpdateVilla(int id, [FromBody] VillaUpdateDTO vdto)
        {
            try
            {
                if (vdto == null || id != vdto.Id)
                {
                    return BadRequest(vdto);
                }

                //based on id get the villa and then update
                //var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
                //villa.Name = vdto.Name;
                //villa.Occupancy = vdto.Occupancy;
                //villa.Sqft = vdto.Sqft;


                // Replace above to use db ctxt
                // convert vdto to villa first and then add, ie manually map vdto obj to villa model
                //Villa model = new Villa()
                //{
                //    Amenity = vdto.Amenity,
                //    Details = vdto.Details,
                //    Id = vdto.Id,
                //    ImageUrl = vdto.ImageUrl,
                //    Name = vdto.Name,
                //    Occupancy = vdto.Occupancy,
                //    Rate = vdto.Rate,
                //    Sqft = vdto.Sqft
                //};
                // update dto to villa model using Auto Mapper
                Villa model = _mapper.Map<Villa>(vdto);

                //EF is smart based on id it will figure out what record to udpate
                // var villa =  _db.Villas.Update(model);
                //await _db.SaveChangesAsync();
                // with repository
                var villa = _dbVilla.UpdateAsync(model);

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
        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        //[ProducesResponseTypeAttribute(204)]        
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> patchDto)
        {
            if (patchDto == null || id == 0)
            {
                return BadRequest();
            }

            //var villa = await _db.Villas.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
            //above using repository
            var villa = await _dbVilla.GetAsync(u => u.Id == id, tracked: false);

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
            VillaUpdateDTO villaDTO = _mapper.Map<VillaUpdateDTO>(villa);

            if (villa == null)
            {
                return BadRequest(); // or NotFound()
            }

            patchDto.ApplyTo(villaDTO, ModelState);
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
            Villa model = _mapper.Map<Villa>(villaDTO);

            //_db.Villas.Update(model);
            //await _db.SaveChangesAsync();
            // above using repo

            await _dbVilla.UpdateAsync(model);

            //if model state is not valid
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return NoContent();
        }
    }
}
