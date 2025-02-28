using Globomantics.Domain.Models;
using Globomatics.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Globomatics.Web.Components
{
    public class ProductListViewComponent : ViewComponent
    {

        private readonly IRepository<Product> productRepository;
        private readonly ILogger<ProductListViewComponent> logger;


        public ProductListViewComponent(IRepository<Product> productRepository, ILogger<ProductListViewComponent> logger)
        {
            this.productRepository = productRepository;
            this.logger = logger;  
        }
        public Task<IViewComponentResult> InvokeAsync()
        {
            // This component will be incharge of listing the products and returning the view
            // to do it, use a constructor injection, requesting the repository and, alogger to be injected
            // in  to the constructor when this view component is being instantiated.

            // could have used async operation to load all data, which for scaliblty is a good idea,
            var products = productRepository.All();

            logger.LogInformation($"Found a total of {products.Count()} products");

            

            //since the method is task with running async operation, it is also needed to be wrapped with completed task.
            // can also tag the method async, but since we are not doing async opration, not needed.
            return Task.FromResult<IViewComponentResult>(View(products));
            /* when this component is being invoked, it will look for a dcorresponding view in a very specific path,
             the view discovery is slightly different from a normal actions invocation to the view method*/
        }
    }
}
