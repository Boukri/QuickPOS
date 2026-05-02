using Ardalis.Specification;
using QuickPOS.Models.Entities;

namespace QuickPOS.Data.Specifications;

public class ProductsByCategorySpec : Specification<Product>
{
    public ProductsByCategorySpec(int? categoryId = null)
    {
        Query.Include(p => p.Category)
             .OrderBy(p => p.Name);
        if (categoryId.HasValue)
            Query.Where(p => p.CategoryId == categoryId.Value);
    }
}

/// <summary>
/// Products for the POS selling view: includes Category and Batches so
/// stock counts and costing-method unit costs can be derived client-side.
/// </summary>
public class ProductsForSellingSpec : Specification<Product>
{
    public ProductsForSellingSpec(int? categoryId = null)
    {
        Query.Include(p => p.Category)
             .Include(p => p.Batches)
             .OrderBy(p => p.Name);
        if (categoryId.HasValue)
            Query.Where(p => p.CategoryId == categoryId.Value);
    }
}

public class AllCategoriesSpec : Specification<CategoryModel>
{
    public AllCategoriesSpec()
    {
        Query.OrderBy(c => c.Name);
    }
}

public class AllIdentityUsersSpec : Specification<AppIdentityUser>
{
    public AllIdentityUsersSpec()
    {
        Query.OrderBy(u => u.FullName);
    }
}

public class TransactionsForDateSpec : Specification<Transaction>
{
    public TransactionsForDateSpec(DateTime date)
    {
        Query.Where(t => t.CreatedAt.Date == date.Date)
             .Include(t => t.Items)
             .ThenInclude(i => i.Product)
             .OrderByDescending(t => t.CreatedAt);
    }
}

public class RecentTransactionsSpec : Specification<Transaction>
{
    public RecentTransactionsSpec(int count = 10)
    {
        Query.OrderByDescending(t => t.CreatedAt)
             .Take(count)
             .Include(t => t.Items);
    }
}

/// <summary>All batches for a product ordered oldest-first (FIFO).</summary>
public class StockBatchesByProductSpec : Specification<StockBatch>
{
    public StockBatchesByProductSpec(int? productId)
    {
        if (productId.HasValue)
        {
            Query.Where(b => b.ProductId == productId.Value)
                 .OrderBy(b => b.ReceivedAt);
        }
        else
        {
            Query.OrderBy(b => b.ReceivedAt);
        }
    }
}

/// <summary>Only batches that still have remaining stock (for FIFO consumption).</summary>
public class ActiveStockBatchesByProductSpec : Specification<StockBatch>
{
    public ActiveStockBatchesByProductSpec(int productId)
    {
        Query.Where(b => b.ProductId == productId && b.QuantityRemaining > 0)
             .OrderBy(b => b.ReceivedAt);
    }
}

/// <summary>All products with their batch collections pre-loaded.</summary>
public class ProductsWithBatchesSpec : Specification<Product>
{
    public ProductsWithBatchesSpec()
    {
        Query.Include(p => p.Category)
             .Include(p => p.Batches)
             .OrderBy(p => p.Name);
    }
}

/// <summary>All batches across every product, including their Product for display.</summary>
public class AllStockBatchesSpec : Specification<StockBatch>
{
    public AllStockBatchesSpec()
    {
        Query.Include(b => b.Product)
             .OrderByDescending(b => b.ReceivedAt);
    }
}

/// <summary>Loads a specific set of products by their IDs (used at checkout to read costing method).</summary>
public class ProductsByIdsSpec : Specification<Product>
{
    public ProductsByIdsSpec(IEnumerable<int> ids)
    {
        Query.Where(p => ids.Contains(p.Id));
    }
}
