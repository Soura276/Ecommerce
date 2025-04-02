using Catalog.Application.Commands;
using Catalog.Application.Mappers;
using Catalog.Application.Responses;
using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using MediatR;

namespace Catalog.Application.Handlers
{
    internal class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, bool>
    {
        private readonly IProductRepository _productRepository;

        public UpdateProductCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var productEntity = new Product()
            {
                Id = request.Id,
                Name = request.Name,
                Description = request.Description,
                ImageFile = request.ImageFile,
                Summary = request.Summary,
                Price = request.Price,
                Brands = request.Brands,
                Types = request.Types,
            };
            return await _productRepository.UpdateProduct(productEntity);
        }
    }
}
