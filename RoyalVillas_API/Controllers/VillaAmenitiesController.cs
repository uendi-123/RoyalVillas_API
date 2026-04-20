using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoyalVillas_API.Data;
using RoyalVillas_API.Models;
using RoyalVillas_API.Models.DTO;

namespace RoyalVilla_API.Controllers
{
    [Route("api/villa-amenities")]
    [ApiController]
    //[Authorize(Roles = "Customer,Admin")]
    public class VillaAmenitiesController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public VillaAmenitiesController(ApplicationDbContext db,IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [HttpGet]
        //[Authorize(Roles ="Admin")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<VillaAmenitiesDTO>>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<IEnumerable<VillaAmenitiesDTO>>>> GetAmenitiesVillas()
        {
            var villas = await _db.VillaAmenities.ToListAsync();
            var dtoResponseVillaAmenities= _mapper.Map<List<VillaAmenitiesDTO>>(villas);
            var response=ApiResponse<IEnumerable<VillaAmenitiesDTO>>.Ok(dtoResponseVillaAmenities, 
                "Villa Amenities retrieved Successfully");

            return Ok(response); 
        }
        [HttpGet("{id:int}")]
        //[AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<VillaAmenitiesDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<VillaAmenitiesDTO>>> GetVillaAmenitiesById (int id)
        {
            try
            {
                if (id <= 0) 
                {
                    return NotFound(ApiResponse<object>.NotFound("Villa Amenities ID must be greater than 0"));
                }
                var villaAmenities = await _db.VillaAmenities.FirstOrDefaultAsync(u => u.Id == id);
                if (villaAmenities == null)
                {
                    return NotFound(ApiResponse<object>.NotFound($"Villa Amenities with ID {id} was not found"));
                }


                return Ok(ApiResponse<VillaAmenitiesDTO>.Ok(_mapper.Map<VillaAmenitiesDTO>(villaAmenities), "Villa Amenities retrieved Successfully"));
            }
            catch(Exception ex)
            {
                var errorResponse = ApiResponse<object>.Error(500, "$An error occured while retrieving villa amenities", ex.Message);
                return StatusCode(500, errorResponse);
            }
        }


        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<VillaAmenitiesDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]

        public async Task<ActionResult<ApiResponse<VillaAmenitiesDTO>>> CreateVillaAmenities(VillaAmenitiesCreateDTO villaAmenitiesDTO)
        {
            try
            {
                if (villaAmenitiesDTO == null)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Villa Amenities data is required"));
                }

                var villaExist = await _db.Villa.FirstOrDefaultAsync(u => u.Id==villaAmenitiesDTO.VillaId);

                if (villaExist == null)
                {
                    return Conflict(ApiResponse<object>.Conflict($"A villa with the ID'{villaAmenitiesDTO.VillaId}' does not exists"));
                }

                VillaAmenities villaAmenities = _mapper.Map<VillaAmenities>(villaAmenitiesDTO);
                villaAmenities.CreatedDate = DateTime.Now;

                await _db.VillaAmenities.AddAsync(villaAmenities);

                await _db.SaveChangesAsync();

                var response = ApiResponse<VillaAmenitiesDTO>.CreateAt(_mapper.Map<VillaAmenitiesDTO>(villaAmenities), "Villa Amenities created successfully");
                return CreatedAtAction(nameof(CreateVillaAmenities),new {id=villaAmenities.Id},response);

            }
            catch (Exception ex)
            {
                var errorResponse = ApiResponse<object>.Error(500, "$An error occured while creating villa amenities", ex.Message);
                return StatusCode(500,errorResponse);
            }
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<VillaAmenitiesDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<VillaAmenitiesDTO>>> UpdateVillaAmenities(int id,VillaAmenitiesUpdateDTO villaAmenitiesDTO)
        {
            try
            {
                if (villaAmenitiesDTO == null)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Villa Amenities data is required"));
                    
                }
                if (id != villaAmenitiesDTO.Id)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Villa Amenities ID in URL does not match Villa Amenities ID in request body "));
                }

                var villaExist = await _db.Villa.FirstOrDefaultAsync(u => u.Id == villaAmenitiesDTO.VillaId);
                if (villaExist == null)
                {
                    return Conflict(ApiResponse<object>.Conflict($"Villa with the ID'{villaAmenitiesDTO.VillaId}' does not exists"));
                }

                var existingVillaAmenities = await _db.VillaAmenities.FirstOrDefaultAsync(u => u.Id == id);

                if (existingVillaAmenities == null)
                {
                    return NotFound(ApiResponse<object>.NotFound("Villa Amenities with ID {id} was not found")); ;
                }

             


                _mapper.Map(villaAmenitiesDTO, existingVillaAmenities);
                existingVillaAmenities.UpdatedDate = DateTime.Now;
                await _db.SaveChangesAsync();
                var response = ApiResponse<VillaAmenitiesDTO>.Ok(_mapper.Map<VillaAmenitiesDTO>(existingVillaAmenities), "Villa Amenities updated successfully");
                return Ok(response);

            }
            catch (Exception ex)
            {
                var errorResponse = ApiResponse<object>.Error(500, "$An error occured while updating villa amenities", ex.Message);
                return StatusCode(500, errorResponse);
            }
        }
        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteVillaAmenities(int id)
        {
            try
            {
  
                var existingVillaAmenities = await _db.VillaAmenities.FirstOrDefaultAsync(u => u.Id == id);

                if (existingVillaAmenities == null)
                {
                    return NotFound(ApiResponse<object>.NotFound("Villa Amenities with ID {id} was not found"));
                }

                _db.VillaAmenities.Remove(existingVillaAmenities);

                await _db.SaveChangesAsync();

                var response = ApiResponse<object>.NoContent("Villa Amenities deleted successfully");
                return Ok(response);

            }
            catch (Exception ex)
            {
                var errorResponse = ApiResponse<object>.Error(500, "$An error occured while creating villa", ex.Message);
                return StatusCode(500, errorResponse);
            }
        }
    }
}