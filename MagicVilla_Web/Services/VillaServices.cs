using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Services.IServices;
using System.Runtime.CompilerServices;

namespace MagicVilla_Web.Services
{
    public class VillaService : BaseService, IVillaService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly string? villaUrl;

        public VillaService(IHttpClientFactory clientFactory, IConfiguration config) : base(clientFactory)
        {
            _clientFactory = clientFactory;
            villaUrl = config.GetValue<string>("ServiceUrls:VillaAPI");
        }
        public Task<T> CreateAsync<T>(VillaCreateDTO dto)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = dto,
                Url=villaUrl+"/api/VillaApi" // api/VillaApi is the Route name of villaAPI controller in Villa project
            });


        }

        public Task<T> DeleteAsync<T>(int id)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.DELETE,
               
                Url = villaUrl + "/api/VillaApi/"+id // api/VillaApi is the Route name of villaAPI controller in Villa project
            });
        }

        public Task<T> GetAllAsync<T>()
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.GET,
               
                Url = villaUrl + "/api/VillaApi" // api/VillaApi is the Route name of villaAPI controller in Villa project
            });
        }

        public Task<T> GetAsync<T>(int id)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.GET,
                
                Url = villaUrl + "/api/VillaApi/"+id // api/VillaApi is the Route name of villaAPI controller in Villa project
            });
        }

        public Task<T> UpdateAsync<T>(VillaUpdateDTO dto)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.PUT,
                Data = dto,
                Url = villaUrl + "/api/VillaApi/"+dto.Id // api/VillaApi is the Route name of villaAPI controller in Villa project
            });
        }
    }
}
