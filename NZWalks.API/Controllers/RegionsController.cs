using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.CustomActionFilters;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;
using System.Text.Json;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegionsController : ControllerBase
    {
        private readonly NZWalksDbContext dbContext;
        private readonly IRegionRepository regionRepository;
        private readonly IMapper mapper;
        private readonly ILogger<RegionsController> logger;

        public RegionsController(NZWalksDbContext dbContext, IRegionRepository regionRepository, IMapper mapper) //ILogger<RegionsController> logger)
        {
            this.dbContext = dbContext;
            this.regionRepository = regionRepository;
            this.mapper = mapper;
            //this.logger = logger;
        }
        // Get All Regions
        [HttpGet]
        [Authorize(Roles ="Reader")]
        public async Task<IActionResult> GetAll()
        {
            //logger.LogInformation("GetAllRegions Action ethod was invoked");
           
            //Get Data From Database - Domain models
            //var regionsDomain = await dbContext.Regions.ToListAsync();

            var regionsDomain = await regionRepository.GetAllAsync();

            //Map Domain Models to DTOs
            //var regionsDto = new List<RegionDto>();
            //foreach (var regionDomain in regionsDomain)
            //{
            //    regionsDto.Add(new RegionDto()
            //    {
            //        Id = regionDomain.Id,
            //        Name = regionDomain.Name,
            //        Code = regionDomain.Code,
            //        RegionImageUrl = regionDomain.RegionImageUrl
            //    });
            //}


            ////Map Domain Models to DTOs by AutoMapper in Return 

            //logger.LogInformation($"Finished GetAllRegions request with data: {JsonSerializer.Serialize(regionsDomain)}");
            return Ok(mapper.Map<List<RegionDto>>(regionsDomain));
        }

        //Get Single Region
        [HttpGet]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Reader")]
        public async Task<IActionResult> GetById([FromRoute] Guid id) 
        {
            //var region = dbContext.Regions.Find(id);
            //Get Region Domain model from Database
            //var regionDomain = await dbContext.Regions.FirstOrDefaultAsync(x => x.Id == id);

            var regionDomain = await regionRepository.GetByIdAsync(id);
            if (regionDomain == null)
            {
                return NotFound();  
            }
            //Map or Convert Region Domain Model to Region DTO
            //var regionDto = new RegionDto
            //{
            //    Id = regionDomain.Id,
            //    Name = regionDomain.Name,
            //    Code = regionDomain.Code,
            //    RegionImageUrl = regionDomain.RegionImageUrl
            //};
            //Returen DTO back to client
            return Ok(mapper.Map<RegionDto>(regionDomain));
        }

        //Post To Create New region
        [HttpPost]
        [ValidateModel]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Create([FromBody] AddRegionRequestDto addRegionRequestDto) 
        {
            //Map or Convert DTO to Domain Model
            //var regionDomainModel = new Region
            //{
            //    Code = addRegionRequestDto.Code,
            //    Name = addRegionRequestDto.Name,
            //    RegionImageUrl = addRegionRequestDto.RegionImageUrl
            //};

                //Map or Convert DTO to Domain Model by AutoMapper
                var regionDomainModel = mapper.Map<Region>(addRegionRequestDto);


                //Use Domain Model to create region
                regionDomainModel = await regionRepository.CreateAsync(regionDomainModel);

                //Map Domain Model back to DTO
                //var regionDto = new RegionDto
                //{
                //    Id = regionDomainModel.Id,
                //    Name = regionDomainModel.Name,
                //    Code = regionDomainModel.Code,
                //    RegionImageUrl = regionDomainModel.RegionImageUrl
                //};

                var regionDto = mapper.Map<RegionDto>(regionDomainModel);

                return CreatedAtAction(nameof(GetById), new { id = regionDto.Id }, regionDto);

            
        }



        //Update Region
        [HttpPut]
        [ValidateModel]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateRegionRequestDto updateRegionRequestDto)
        {

            //Map DTO to Domain Model
            //var regionDomainModel = new Region
            //{
            //    Code = updateRegionRequestDto.Code,
            //    Name = updateRegionRequestDto.Name,
            //    RegionImageUrl = updateRegionRequestDto.RegionImageUrl
            //};
            
                //Map DTO to Domain Model by automapper
                var regionDomainModel = mapper.Map<Region>(updateRegionRequestDto);

                //Check if Region exists
                await regionRepository.UpdateAsync(id, regionDomainModel);

                if (regionDomainModel == null)
                {
                    return NotFound();
                }

                //Map Dto to Domain Model
                //regionDomainModel.Code = updateRegionRequestDto.Code;
                //regionDomainModel.Name = updateRegionRequestDto.Name;
                //regionDomainModel.RegionImageUrl = updateRegionRequestDto.RegionImageUrl;

                //await dbContext.SaveChangesAsync();

                //Convert Domain Model to DTO
                //var regionDto = new RegionDto
                //{
                //    Id = regionDomainModel.Id,
                //    Name = regionDomainModel.Name,
                //    Code = regionDomainModel.Code,
                //    RegionImageUrl = regionDomainModel.RegionImageUrl
                //};
                return Ok(mapper.Map<RegionDto>(regionDomainModel));
        }

        //Delete a Region
        [HttpDelete]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var regionDomainModel = await regionRepository.DeleteAsync(id);
            if(regionDomainModel == null)
            {
                return NotFound();  
            }
            //Region delete (we use it in repository folder)
            //dbContext.Regions.Remove(regionDomainModel);
            //await dbContext.SaveChangesAsync();

            //Return deleted Region Back
            // Map Domain Model to DTO
            //var regionDto = new RegionDto
            //{
            //    Id = regionDomainModel.Id,
            //    Name = regionDomainModel.Name,
            //    Code = regionDomainModel.Code,
            //    RegionImageUrl = regionDomainModel.RegionImageUrl
            //};


            return Ok(mapper.Map<RegionDto>(regionDomainModel));
        }
    }
}