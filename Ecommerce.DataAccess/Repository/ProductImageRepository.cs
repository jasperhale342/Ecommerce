using Ecommerce.DataAccess.Data;
using Ecommerce.DataAccess.Repository.IRepository;
using Ecommerce.Models;

namespace Ecommerce.DataAccess.Repository
{
    public class ProductImageRepository : Repository<ProductImage>, IProductImageRepository
    {

        private ApplicationDbContext _db;
        public ProductImageRepository(ApplicationDbContext db) : base(db) 
        {

            _db = db;
        }
       
        public void Update(ProductImage obj)
        {
            _db.ProductImages.Update(obj);
        }
    }
}
