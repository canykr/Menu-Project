using MenuProject.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace MenuProject.Controllers
{
    public class HomeController : Controller
    {
        SqlConnection connection = new SqlConnection();

        public HomeController(IConfiguration configuration)
        {
            connection.ConnectionString = configuration.GetConnectionString("MenuContext");
        }
        public IActionResult Index()
        {
            List<Categories> categories = new List<Categories>();
            SqlCommand cmd = new SqlCommand("select * from dbo.Categories", connection);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            foreach (DataRow rowCategory in dt.Rows)
            {
                Categories category = new Categories
                {
                    Id = Convert.ToInt32(rowCategory["Id"]),
                    CategoryName = rowCategory["CategoryName"].ToString(),
                    Products = new List<Products>()
                };

                SqlCommand command = new SqlCommand("select * from dbo.Products where CategoryId = @CategoryId order by ProductName", connection);

                command.Parameters.AddWithValue("@CategoryId", category.Id);
                SqlDataAdapter daProducts = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                daProducts.Fill(dataTable);
                foreach (DataRow rowProduct in dataTable.Rows)
                {
                    Products product = new Products
                    {
                        Id = Convert.ToInt32(rowProduct["Id"]),
                        CategoryId = category.Id,
                        ProductName = rowProduct["ProductName"].ToString(),
                        ProductDetails = rowProduct["ProductDetails"].ToString(),
                        Price = Convert.ToDecimal(rowProduct["Price"]),
                        ImageUrl = rowProduct["ImageUrl"].ToString()
                    };
                    category.Products.Add(product);
                }
                categories.Add(category);

            }
            return View(categories);
        }
    }
}

