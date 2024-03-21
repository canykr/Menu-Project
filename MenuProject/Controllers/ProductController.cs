using MenuProject.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using System.Data.SqlClient;

namespace MenuProject.Controllers
{
    public class ProductsController : Controller
    {
        SqlConnection connection = new SqlConnection();

        public ProductsController(IConfiguration configuration)
        {
            connection.ConnectionString = configuration.GetConnectionString("MenuContext");
        }

        [HttpGet]
        public IActionResult Index(int categoryId)
        {
            List<Products> products = new List<Products>();
            SqlCommand command = new SqlCommand(@"select p.*, c.CategoryName from Products p inner join Categories c ON p.CategoryId = c.Id where c.Id = @CategoryId", connection);
            command.Parameters.AddWithValue("@CategoryId", categoryId);

            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            foreach (DataRow row in dataTable.Rows)
            {
                products.Add(new Products
                {
                    Id = Convert.ToInt32(row["Id"]),
                    CategoryId = Convert.ToInt32(row["CategoryId"]),
                    ProductName = Convert.ToString(row["ProductName"]),
                    ProductDetails = Convert.ToString(row["ProductDetails"]),
                    Price = Convert.ToDecimal(row["Price"]),
                    ImageUrl = Convert.ToString(row["ImageUrl"]),
                    CategoryName = Convert.ToString(row["CategoryName"])
                });
            }
            return View("Index", products);
        }


        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(GetCategories(), "Id", "CategoryName");
            return View();
        }
        public List<Categories> GetCategories()
        {
            List<Categories> categories = new List<Categories>();
            SqlCommand command = new SqlCommand("SELECT Id, CategoryName FROM Categories", connection);
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataTable dataTable = new DataTable();
            connection.Open();
            adapter.Fill(dataTable);

            foreach (DataRow row in dataTable.Rows)
            {
                categories.Add(new Categories
                {
                    Id = Convert.ToInt32(row["Id"]),
                    CategoryName = Convert.ToString(row["CategoryName"])
                });
            }
            return categories;
        }
        [HttpPost]
        public IActionResult Create(Products product)
        {
            SqlCommand command = new SqlCommand("insert into Products (ProductName, ProductDetails, Price, ImageUrl, CategoryId) values (@ProductName, @ProductDetails, @Price, @ImageUrl, @CategoryId)", connection);
            command.Parameters.AddWithValue("@ProductName", product.ProductName);
            command.Parameters.AddWithValue("@ProductDetails", (object)product.ProductDetails ?? DBNull.Value);
            command.Parameters.AddWithValue("@Price", product.Price);
            command.Parameters.AddWithValue("@ImageUrl", product.ImageUrl);
            command.Parameters.AddWithValue("@CategoryId", product.CategoryId);

            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();

            return RedirectToAction("Index", new { categoryId = product.CategoryId });
        }

        public IActionResult Edit(int id)
        {
            Products product = new Products();
            SqlCommand command = new SqlCommand(@"select p.Id, p.ProductName, p.ProductDetails, p.Price, p.ImageUrl, p.CategoryId, c.CategoryName from Products p join Categories c ON p.CategoryId = c.Id where p.Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);

            DataRow row = dataTable.Rows[0];
            product = new Products
            {
                Id = Convert.ToInt32(row["Id"]),
                CategoryId = Convert.ToInt32(row["CategoryId"]),
                ProductName = Convert.ToString(row["ProductName"]),
                ProductDetails = Convert.ToString(row["ProductDetails"]),
                Price = Convert.ToDecimal(row["Price"]),
                ImageUrl = Convert.ToString(row["ImageUrl"]),
                CategoryName = Convert.ToString(row["CategoryName"])
            };

            ViewBag.Categories = new SelectList(GetCategories(), "Id", "CategoryName", product.CategoryId);
            return View(product);

        }

        [HttpPost]
        public IActionResult Edit(Products product)
        {
            SqlCommand command = new SqlCommand("update Products set ProductName = @ProductName, ProductDetails = @ProductDetails, Price = @Price, ImageUrl = @ImageUrl, CategoryId =@CategoryId where Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", product.Id);
            command.Parameters.AddWithValue("@ProductName", product.ProductName);
            command.Parameters.AddWithValue("@ProductDetails", (object)product.ProductDetails ?? DBNull.Value);
            command.Parameters.AddWithValue("@Price", product.Price);
            command.Parameters.AddWithValue("@ImageUrl", product.ImageUrl);
            command.Parameters.AddWithValue("@CategoryId", product.CategoryId);

            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();

            return RedirectToAction("Index", new { categoryId = product.CategoryId });
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            int categoryId = 0;
            SqlCommand command = new SqlCommand("select CategoryId from Products where Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);
            DataTable dataTable = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(dataTable);

            categoryId = Convert.ToInt32(dataTable.Rows[0]["CategoryId"]);
            SqlCommand cmd = new SqlCommand("delete from Products where Id = @Id", connection);
            cmd.Parameters.AddWithValue("@Id", id);
            connection.Open();
            cmd.ExecuteNonQuery();
            connection.Close();

            return RedirectToAction("Index", new { categoryId = categoryId });
        }
    }
}
