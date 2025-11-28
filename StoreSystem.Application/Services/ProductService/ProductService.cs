using StoreSystem.Core.Interfaces;
using StoreSystem.Core.common;
using StoreSystem.Core.Entities;
using StoreSystem.Application.Contract.Common;
using StoreSystem.Application.Contract.ProductContract.Req;
using StoreSystem.Application.Contract.ProductContract.Res;
using StoreSystem.Application.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using StoreSystem.Core.Events.ProductEvent;
using AutoMapper;
using FluentValidation;

namespace StoreSystem.Application.Services.ProductService
{
    /// <summary>
    /// Product service implementation following Clean Architecture (application layer).
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly IRepository<Product> _productRepo;
        private readonly IUniteOfWork _uow;
        private readonly IEventBus _mediator;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;
        private readonly IValidator<ProductReq> _validator;
        private const string ProductsCacheKey = "products_list";
        private readonly ICurrentUserService _CurrentUserService;

        public ProductService(ICurrentUserService currentUserService,IValidator<ProductReq> Validator,IRepository<Product> productRepo, IUniteOfWork uow, IEventBus mediator, IMapper mapper, IMemoryCache cache)
        {
            _productRepo = productRepo;
            _uow = uow;
            _mediator = mediator;
            _mapper = mapper;
            _validator = Validator;
            _cache = cache;
            _CurrentUserService = currentUserService;
        }

        /// <inheritdoc />
        public async Task<GeneralResponse<int>> CreateAsync(ProductReq req)
        {
            if (!_CurrentUserService.IsAuthenticated)
                return GeneralResponse<int>.Failure("Unauthorized", 401);

            if (req == null) return GeneralResponse<int>.Failure("Invalid payload", 400);

            var validationResult = await _validator.ValidateAsync(req);
            if (!validationResult.IsValid)
            {
                var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                return GeneralResponse<int>.Failure(errors, 400);
            }

            Product entity = _mapper.Map<Product>(req);
            entity.UpdateAt = DateTime.UtcNow;
            entity.StockQuantity = req.StockQuantity;
            entity.CreateByUserId = _CurrentUserService.UserId;
            entity.UpdateByUserId = _CurrentUserService.UserId;
    
            await _productRepo.AddAsync(entity);
            await _uow.CompleteAsync();
            await _mediator.PublishAsync(new ProductCreatedEvent(entity.Id, entity.UpdateAt, entity.StockQuantity, entity.StoreId,_CurrentUserService.UserId!));
            _cache.Remove(ProductsCacheKey);

            return GeneralResponse<int>.Success(entity.Id, "Product created", 201);
        }

        /// <inheritdoc />
        public async Task<GeneralResponse<bool?>> UpdateAsync(int id, ProductReq req)
        {
            if (!_CurrentUserService.IsAuthenticated)
                return GeneralResponse<bool?>.Failure("Unauthorized", 401);

            if (id < 1 || req == null) return GeneralResponse<bool?>.Failure("Invalid data", 400);

            var validationResult = await _validator.ValidateAsync(req);
            if (!validationResult.IsValid)
            {
                var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                return GeneralResponse<bool?>.Failure(errors, 400);
            }
            
            var product = await _productRepo.FindAsync(p => p.Id == id && p.StoreId == _CurrentUserService.StoreId!.Value);
            if (product == null) return GeneralResponse<bool?>.Failure("Product not found", 404);

            _mapper.Map(req, product);
            product.UpdateAt = DateTime.UtcNow;
            product.UpdateByUserId = _CurrentUserService.UserId;

            var ok = await _productRepo.UpdateAsync(product);
            if (!ok) return GeneralResponse<bool?>.Failure("Could not update product", 500);

            await _uow.CompleteAsync();
            await _mediator.PublishAsync(new ProductUpdatedEvent(product.Id, product.UpdateAt));

            _cache.Remove(ProductsCacheKey);
            return GeneralResponse<bool?>.Success(null, "Product updated", 200);
        }

        /// <inheritdoc />
        public async Task<GeneralResponse<bool?>> DeleteAsync(int id)
        {
            if (!_CurrentUserService.IsAuthenticated)
                return GeneralResponse<bool?>.Failure("Unauthorized", 401);

            if (id < 1) return GeneralResponse<bool?>.Failure("Invalid id", 400);
            var product = await _productRepo.FindAsync(p => p.Id == id && p.StoreId == _CurrentUserService.StoreId!.Value);
            if (product == null) return GeneralResponse<bool?>.Failure("Product not found", 404);

            _productRepo.DeleteAsync(product);
            await _uow.CompleteAsync();

            await _mediator.PublishAsync(new ProductDeletedEvent(product.Id));

            _cache.Remove(ProductsCacheKey);

            return GeneralResponse<bool?>.Success(true, "Deleted", 200);
        }
        /// <inheritdoc />
        public async Task<GeneralResponse<ProductRes?>> GetByIdAsync(int id)
        {
            if (!_CurrentUserService.IsAuthenticated)
                return GeneralResponse<ProductRes?>.Failure("Unauthorized", 401);

            if (id < 1) return GeneralResponse<ProductRes?>.Failure("Invalid id", 400);

            var product = await _productRepo.FindAsync(p => p.Id == id && p.StoreId == _CurrentUserService.StoreId!.Value);
            if (product == null) return GeneralResponse<ProductRes?>.Failure("Not found", 404);

            var res = _mapper.Map<ProductRes>(product);
            return GeneralResponse<ProductRes?>.Success(res, "Ok", 200);
        }

        /// <inheritdoc />
        public async Task<GeneralResponse<PagedResult<ProductRes>>> GetAllAsync(GetProductReq req)
        {
            if (!_CurrentUserService.IsAuthenticated)
                return GeneralResponse<PagedResult<ProductRes>>.Failure("Unauthorized", 401);

            var page = await _productRepo.GetAllAsync(req.PageNumber, req.PageSize,x => x.StoreId == _CurrentUserService.StoreId!.Value );
            var mapped = new PagedResult<ProductRes>
            {
                Items = page.Items.Select(p => _mapper.Map<ProductRes>(p)),
                PageNumber = page.PageNumber,
                PageSize = page.PageSize,
                TotalItems = page.TotalItems
            };

            return GeneralResponse<PagedResult<ProductRes>>.Success(mapped, "Ok", 200);
        }

       
    }
}
