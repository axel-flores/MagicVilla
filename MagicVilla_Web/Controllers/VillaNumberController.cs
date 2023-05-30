using AutoMapper;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Models.VM;
using MagicVilla_Web.Services;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MagicVilla_Web.Controllers
{
    public class VillaNumberController : Controller
    {
        private readonly IMapper _mapper;
        private IVillaNumberService _villaNumberService;
        private IVillaService _villaService;
        public VillaNumberController(IVillaNumberService villaNumberService, IMapper mapper, IVillaService villaService)
        {
            _villaNumberService = villaNumberService;
            _mapper = mapper;
            _villaService = villaService;
        }

        public async Task<IActionResult> IndexVillaNumber()
        {
            List<VillaNumberDTO> list = new();

            var response = await _villaNumberService.GetAllAsync<APIResponse>();
            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<VillaNumberDTO>>(Convert.ToString(response.Result));
            }
            return View(list);
        }

        public async Task<IActionResult> CreateVillaNumber()
        {
            VillaNumberCreateVM villaNumberCreateVM = new();

            var response = await _villaService.GetAllAsync<APIResponse>();
            if (response != null && response.IsSuccess )
            {
                villaNumberCreateVM.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>
                    (Convert.ToString(response.Result)).Select(i => new SelectListItem
                    {
                        Text = $"{i.Id} - {i.Name}",
                        Value = i.Id.ToString(),
                    });
            }
            return View(villaNumberCreateVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateVillaNumber(VillaNumberCreateVM model)
        {
            if (ModelState.IsValid)
            {
                var response = await _villaNumberService.CreateAsync<APIResponse>(model.VillaNumber);
                if (response != null && response.IsSuccess)
                {
                    return RedirectToAction(nameof(IndexVillaNumber));
                }
                else
                {
                    if (response.ErrorMessages.Count > 0)
                    {
                        ModelState.AddModelError("ErrorMessages", response.ErrorMessages.FirstOrDefault()); ;

                    }
                }

            }

            var dropdownList = await _villaService.GetAllAsync<APIResponse>();
            if (dropdownList != null && dropdownList.IsSuccess)
            {
                model.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>
                    (Convert.ToString(dropdownList.Result)).Select(i => new SelectListItem
                    {
                        Text = $"{i.Id} - {i.Name}",
                        Value = i.Id.ToString(),
                    });
            }

            return View(model);
        }

        public async Task<IActionResult> UpdateVillaNumber(int villaNum)
        {
            VillaNumberUpdateVM villaNumberVM = new();

            var response = await _villaNumberService.GetAsync<APIResponse>(villaNum);
            if (response != null && response.IsSuccess)
            {
                VillaNumberDTO model = JsonConvert.DeserializeObject<VillaNumberDTO>(Convert.ToString(response.Result));
                villaNumberVM.VillaNumber = _mapper.Map<VillaNumberUpdateDTO>(model);
            }

            response = await _villaService.GetAllAsync<APIResponse>();
            if (response != null && response.IsSuccess)
            {
                villaNumberVM.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>
                    (Convert.ToString(response.Result)).Select(i => new SelectListItem
                    {
                        Text = $"{i.Id} - {i.Name}",
                        Value = i.Id.ToString(),
                    });
                return View(villaNumberVM);
            }

            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateVillaNumber(VillaNumberUpdateVM model)
        {
            if (ModelState.IsValid)
            {
                var response = await _villaNumberService.UpdateAsync<APIResponse>(model.VillaNumber);
                if (response != null && response.IsSuccess)
                {
                    return RedirectToAction(nameof(IndexVillaNumber));
                }
                else
                {
                    if (response.ErrorMessages.Count > 0)
                    {
                        ModelState.AddModelError("ErrorMessages", response.ErrorMessages.FirstOrDefault()); ;

                    }
                }

            }

            var dropdownList = await _villaService.GetAllAsync<APIResponse>();
            if (dropdownList != null && dropdownList.IsSuccess)
            {
                model.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>
                    (Convert.ToString(dropdownList.Result)).Select(i => new SelectListItem
                    {
                        Text = $"{i.Id} - {i.Name}",
                        Value = i.Id.ToString(),
                    });
            }
            return View(model);
        }

        public async Task<IActionResult> DeleteVillaNumber(int villaNum)
        {
            VillaNumberDeleteVM villaNumberVM = new();
            var response = await _villaNumberService.GetAsync<APIResponse>(villaNum);
            if (response != null && response.IsSuccess)
            {
                VillaNumberDTO model = JsonConvert.DeserializeObject<VillaNumberDTO>(Convert.ToString(response.Result));
                villaNumberVM.VillaNumber = model;
            }

            response = await _villaService.GetAllAsync<APIResponse>();

            if(response != null && response.IsSuccess)
            {
                villaNumberVM.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>
                    (Convert.ToString(response.Result)).Select(i => new SelectListItem
                    {
                        Text = i.Name,
                        Value = i.Id.ToString(),
                    });
                return View(villaNumberVM);
            }

            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteVillaNumber(VillaNumberDeleteVM model)
        {

            var response = await _villaNumberService.DeleteAsync<APIResponse>(model.VillaNumber.VillaNum);
            if (response != null && response.IsSuccess)
            {
                return RedirectToAction(nameof(IndexVillaNumber));
            }

            return View(model);
        }
    }
}
