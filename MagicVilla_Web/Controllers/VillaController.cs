using AutoMapper;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MagicVilla_Web.Controllers
{
    public class VillaController : Controller
    {// for injection first we need villaservice to call api and automapper
        private readonly IVillaService _villaService;
        private readonly IMapper _mapper;
        public VillaController(IMapper mapper, IVillaService villaService)
        {
            _mapper = mapper;
            _villaService = villaService;
        }
        public async Task<IActionResult> IndexVilla()
        {
            List<VillaDTO> list = new();
            //use villa service and call method inside there
            var response = await _villaService.GetAllAsync<APIResponse>();
            if (response != null && response.IsSuccess)
            {
                //convert to string and then dese obj to list<VillaDTO>
                list = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(response.Result));
            }

            return View(list);
        }
    }
}
