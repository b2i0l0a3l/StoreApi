using StoreSystem.Core.Interfaces;
using StoreSystem.Core.common;
using StoreSystem.Application.Contract.Categories.req;
using StoreSystem.Application.Contract.Categories.res;
using StoreSystem.Application.Contract.Categories.Validator;
using StoreSystem.Application.Interfaces;
using StoreSystem.Core.Entities;
using StoreSystem.Application.Contract.Common;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace StoreSystem.Application.Services.CategoryService
{
    public class CategoryService : ICategoryService
    {
        private readonly IRepository<Category> _repo;
        private CategoryValidator _validator;
        private ICurrentUserService _CurrentUserService;

        public CategoryService(ICurrentUserService currentUserService,IRepository<Category> repo, CategoryValidator validations)
        {
            _repo = repo;
            _validator = validations;
            _CurrentUserService = currentUserService;
        }

        public async Task<GeneralResponse<int>> AddAsync(CategoryReq entity)
        {
            if (entity == null )
                return GeneralResponse<int>.Failure("Invalid Data", 400);

            var result = IsEntityValid(entity);

            if (!result.Item1)
            {
                return GeneralResponse<int>.Failure(string.Join(" ,", result.Item2));

            }            
       
            Category category = new Category { Name = entity.Name, InventoryId = _CurrentUserService.InventoryId ?? 0, CreateByUserId = _CurrentUserService.UserId, UpdateByUserId = _CurrentUserService.UserId };
            try
            {
                await _repo.AddAsync(category);

                await _repo.SaveAsync();
                return  GeneralResponse<int>.Success(category.Id , "Category Added Successfully", 201);

            }
            catch(Exception ex)
            {
                return GeneralResponse<int>.Failure($"Error while Adding Category : {ex.Message}",500);
                
            }

        } 

        public async Task<GeneralResponse<bool?>> DeleteAsync(int id)
        {

            if (id < 1)
            {
                return GeneralResponse<bool?>.Failure("Invalid Data", 400);
            }

            Category? category = await _repo.FindAsync(x => x.Id == id && (x.InventoryId == _CurrentUserService.InventoryId || x.Inventory!.StoreId == _CurrentUserService.StoreId));
            if (category == null)
                return GeneralResponse<bool?>.Failure($"Category with Id : {id} Not Found", 404);

            _repo.DeleteAsync(category);
            await _repo.SaveAsync();
            return  GeneralResponse<bool?>.Success(null , "Category deleted Successfully", 200);
        }  
        public async Task<GeneralResponse<PagedResult<CategoryRes>>> GetAllAsync(GetCategoryReq entity)
        {





            PagedResult<Category> r = await _repo.GetAllAsync(entity.PageNumber, entity.PageSize);
            if (r != null && r.Items.Any())
            {
                PagedResult<CategoryRes> result = new()
                {
                    Items = r.Items.Select(x => new CategoryRes { Name = x.Name, Id = x.Id, CreateAt = x.CreatedAt }),
                    PageNumber = r.PageNumber,
                    PageSize = r.PageSize,
                    TotalItems = r.TotalItems
                };
                return GeneralResponse<PagedResult<CategoryRes>>.Success(result, "success", 200);
            }
            return GeneralResponse<PagedResult<CategoryRes>>.Failure("There is no Category!", 404);

        }
        public async Task<GeneralResponse<PagedResult<CategoryRes>>> GetAllForStoreAsync(GetCategoryReq entity)
        {

            if (entity == null || entity.PageNumber < 1  || entity.PageSize < 1)
                return GeneralResponse<PagedResult<CategoryRes>>.Failure("Invalid Data", 400);


            PagedResult<Category> r = await _repo.GetAllAsync(entity.PageNumber, entity.PageSize, x=>x.InventoryId == _CurrentUserService.InventoryId || x.Inventory!.StoreId == _CurrentUserService.StoreId);
            if (r != null && r.Items.Any())
            {
                PagedResult<CategoryRes> result = new()
                {
                    Items = r.Items.Select(x => new CategoryRes { Name = x.Name, Id = x.Id, CreateAt = x.CreatedAt }),
                    PageNumber = r.PageNumber,
                    PageSize = r.PageSize,
                    TotalItems = r.TotalItems

                };
                return GeneralResponse<PagedResult<CategoryRes>>.Success(result, "success", 200);
            }
            return GeneralResponse<PagedResult<CategoryRes>>.Failure("There is no Category!", 404);

        }
        public async Task<GeneralResponse<CategoryRes?>> GetByIdAsync(int id)
        {

             if (id < 1)
                return GeneralResponse<CategoryRes?>.Failure("Invalid Data", 400);

            Category? category = await _repo.FindAsync(x => x.Id == id && (x.InventoryId == _CurrentUserService.InventoryId || x.Inventory!.StoreId == _CurrentUserService.StoreId)); ;
            if (category == null)
                return GeneralResponse<CategoryRes?>.Failure($"Category with Id : {id} Not Found", 404);
            return  GeneralResponse<CategoryRes?>.Success(new CategoryRes{Name = category.Name, Id = category.Id} , "success", 200);
        }
        public async Task<GeneralResponse<bool?>> Update(CategoryReq entity, int Id)
        {
            if (Id < 1 || entity == null)
                return GeneralResponse<bool?>.Failure("Invalid Data", 400);
           
            var result = IsEntityValid(entity);

            if (!result.Item1)
            {
                return GeneralResponse<bool?>.Failure(string.Join(" ,", result.Item2));
            }

            Category? category = await _repo.FindAsync(x => x.Id == Id && (x.InventoryId == _CurrentUserService.InventoryId || x.Inventory!.StoreId == _CurrentUserService.StoreId));
            if (category == null)
                return GeneralResponse<bool?>.Failure($"Category with Id : {Id} Not Found", 404);

            category.Name = entity.Name;
            category.UpdateByUserId = _CurrentUserService.UserId;
            var r = await _repo.UpdateAsync(category);
            if (!r)
                return GeneralResponse<bool?>.Failure($"internal server error", 500);
            await _repo.SaveAsync();
            return GeneralResponse<bool?>.Success(null, "success", 201);

        }
        /*
        private async Task<int?> GetInventoryId()
        {
            int? StoreId = _CurrentUserService.StoreId!.Value;
            if (StoreId == null) return null;

            var r =  await _repo.FindAsync(x => x.Inventory!.StoreId == StoreId);
            return null; // Placeholder
        }
        */
   
        private Tuple<bool, string> IsEntityValid(CategoryReq entity)
        {
            var result = _validator.Validate(entity);
            if (!result.IsValid)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.ErrorMessage));
                return new Tuple<bool, string>(false, errors);
            }
            return new Tuple<bool, string>(true, "");
        }
        
    }
}