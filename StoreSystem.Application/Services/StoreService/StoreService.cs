using StoreSystem.Core.Interfaces;
using StoreSystem.Core.common;
using AutoMapper;
using StoreSystem.Core.Entities;
using StoreSystem.Application.Contract.Common;
using StoreSystem.Application.Contract.StoreContract.req;
using StoreSystem.Application.Contract.StoreContract.res;
using StoreSystem.Application.Contract.StoreContract.validator;
using StoreSystem.Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using StoreSystem.Application.Interfaces.Auth;
using System.Collections.Generic;

namespace StoreSystem.Application.Services.StoreService
{
    public class StoreService: IStoreService
    {
        private readonly IRepository<Store> _repo;
        private readonly IMapper _mapper;
        private readonly StoreValidator _validator;
        private readonly ICurrentUserService _CurrentUserService;
        private readonly IToken _tokenService;
        private readonly IUserClaimsExtension _userClaimsExtension;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<Inventory> _inventoryRepo;

        public StoreService(
            IRepository<Store> repo, 
            IMapper mapper, 
            StoreValidator validator, 
            ICurrentUserService currentUserService,
            IToken tokenService,
            IUserClaimsExtension userClaimsExtension,
            UserManager<ApplicationUser> userManager,
            IRepository<Inventory> inventoryRepo)
        {
            _repo = repo;
            _mapper = mapper;
            _validator = validator;
            _CurrentUserService = currentUserService;
            _tokenService = tokenService;
            _userClaimsExtension = userClaimsExtension;
            _userManager = userManager;
            _inventoryRepo = inventoryRepo;
        }

        public async Task<GeneralResponse<StoreRes>> AddAsync(StoreReq entity)
        {
            if (_CurrentUserService.UserId == null)
                return GeneralResponse<StoreRes>.Failure("Create An Account First", 400);

            if (entity == null)
                return GeneralResponse<StoreRes>.Failure("Invalid Data");
            try
            {
                var result = _validator.Validate(entity);

                if (!result.IsValid)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.ErrorMessage));

                    return GeneralResponse<StoreRes>.Failure(string.Join(", ", errors));
                }


                Store store = _mapper.Map<Store>(entity);
                store.CreateByUserId =store.UserId = store.UpdateByUserId = _CurrentUserService.UserId!;
                await _repo.AddAsync(store);
                await _repo.SaveAsync();

                var inventory = new Inventory 
                { 
                    Name = "Main Inventory", 
                    StoreId = store.Id,
                    Location = store.Address ?? "Main Location"
                };
                await _inventoryRepo.AddAsync(inventory);
                await _inventoryRepo.SaveAsync();

                var user = await _userManager.FindByIdAsync(_CurrentUserService.UserId!);
                var claims = await _userClaimsExtension.GetClaimsAsync(user!);
                
                var authClaims = new List<Claim>(claims);
                authClaims.RemoveAll(c => c.Type == "StoreId" || c.Type == "InventoryId");
                
                authClaims.Add(new Claim("StoreId", store.Id.ToString()));
                authClaims.Add(new Claim("InventoryId", inventory.Id.ToString()));
                
                foreach (var role in await _userManager.GetRolesAsync(user!))
                {
                    if (!authClaims.Any(c => c.Type == ClaimTypes.Role && c.Value == role))
                        authClaims.Add(new Claim(ClaimTypes.Role, role));
                }

                var newToken = _tokenService.GenerateAccessToken(authClaims, isTemporary: false);

                var response = _mapper.Map<StoreRes>(store);
                response.NewToken = newToken;

                return GeneralResponse<StoreRes>.Success(response, "Store Added Successfully", 201);
                
            }catch(Exception ex)
            {
                    return GeneralResponse<StoreRes>.Failure(ex.Message);
                
            }
        }

        public async Task<GeneralResponse<bool?>> DeleteAsync(int id)
        {

            if (id < 1)
                return GeneralResponse<bool?>.Failure("Invalid Data");

            var result = await _repo.FindAsync(x => x.Id == id && x.UserId == _CurrentUserService.UserId);
            if (result == null)
                return GeneralResponse<bool?>.Failure($"store With Id {id} Not Found", 404);
            
            _repo.DeleteAsync(result);
            await _repo.SaveAsync();
            return GeneralResponse<bool?>.Success(null, "store deleted Successfully",201);
        }
        public async Task<GeneralResponse<PagedResult<StoreRes>>> GetAllAsync(GetStoreReq entity)
        {
             if (!_CurrentUserService.IsAuthenticated)
                return GeneralResponse<PagedResult<StoreRes>>.Failure("Unauthorized",401);

            if (entity == null)
                return GeneralResponse<PagedResult<StoreRes>>.Failure("Invalid Data");

           

            try
            {
                PagedResult<Store> pagedResult = await _repo.GetAllAsync(entity.PageNumber, entity.PageSize);

                if (pagedResult != null)
                {
                    PagedResult<StoreRes> result = new()
                    {
                        Items = pagedResult.Items.Select(x => _mapper.Map<StoreRes>(x)).ToList(),
                        PageNumber = pagedResult.PageNumber,
                        PageSize = pagedResult.PageSize
                    };
                    return GeneralResponse<PagedResult<StoreRes>>.Success(result, "Success", 200);
                }
                return GeneralResponse<PagedResult<StoreRes>>.Failure("there is no products yet!", 404);
            }
            catch (Exception ex)
            {
                return GeneralResponse<PagedResult<StoreRes>>.Failure("Error Happend : " + ex.Message);
            }
        }
        public async Task<GeneralResponse<StoreRes?>> GetByIdAsync(int id)
        {

             if (id < 1)
                return GeneralResponse<StoreRes?>.Failure("Invalid Data");

            Store? store = await _repo.FindAsync(x => x.Id == id && x.UserId == _CurrentUserService.UserId);
            if (store != null)
                return GeneralResponse<StoreRes?>.Success(_mapper.Map<StoreRes>(store), "success", 200);
            
            return GeneralResponse<StoreRes?>.Failure($"there is no store with that Id {id} ");
            
        }
        public async Task<GeneralResponse<PagedResult<StoreRes>?>> GetStoreByJwtAsync()
        {
            if (!_CurrentUserService.IsAuthenticated)
                return GeneralResponse<PagedResult<StoreRes>?>.Failure("Unauthorized",401);

            var result = await _repo.GetAllAsync(1,10, x => x.UserId == _CurrentUserService.UserId);
            if (result != null)
            {
                PagedResult<StoreRes> res = new()
                {
                    Items = result.Items.Select(x => _mapper.Map<StoreRes>(x)).ToList(),
                    PageNumber = result.PageNumber,
                    PageSize = result.PageSize
                };
                return GeneralResponse<PagedResult<StoreRes>?>.Success(res , "success", 200);
            }
            return GeneralResponse<PagedResult<StoreRes>?>.Failure($"there is no store, Create Your Store First ");
        }
        public async Task<GeneralResponse<bool?>> Update(StoreReq entity, int Id)
        {

            if (Id < 1 || entity == null)
                return GeneralResponse<bool?>.Failure("Invalid Data");


            var result = _validator.Validate(entity);
            if (!result.IsValid)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.ErrorMessage));

                return GeneralResponse<bool?>.Failure(string.Join(", ", errors));
            }

            Store? store = await _repo.FindAsync(x => x.Id == Id && x.UserId == _CurrentUserService.UserId);

            if (store == null)
                return GeneralResponse<bool?>.Failure($"there is no Store with that Id : {Id}");

            _mapper.Map(entity, store);
            store.UpdateByUserId = _CurrentUserService.UserId;
            await _repo.SaveAsync();
            return GeneralResponse<bool?>.Success(null, "Store Updated Successfully",200);
            
        }
    }
}