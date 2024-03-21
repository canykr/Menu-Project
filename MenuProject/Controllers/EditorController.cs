using MenuProject.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace MenuProject.Controllers
{
    public class EditorController : Controller
    {
        SqlConnection connection = new SqlConnection();

        public EditorController(IConfiguration configuration)
        {
            connection.ConnectionString = configuration.GetConnectionString("MenuContext");
        }
        public IActionResult Index()
        {
            List<Categories> categories = new List<Categories>();
            SqlCommand command = new SqlCommand("select Id, CategoryName from Categories", connection);
            SqlDataAdapter da = new SqlDataAdapter(command);
            DataTable dt = new DataTable();
            da.Fill(dt);

            foreach (DataRow row in dt.Rows)
            {
                categories.Add(new Categories
                {
                    Id = Convert.ToInt32(row["Id"]),
                    CategoryName = Convert.ToString(row["CategoryName"])
                });
            }
            return View(categories);
        }

        [HttpPost]
        public IActionResult Create(string categoryName)
        {
            SqlCommand command = new SqlCommand("insert into Categories (CategoryName) values (@CategoryName)", connection);
            command.Parameters.AddWithValue("@CategoryName", categoryName);
            connection.Open();
            command.ExecuteNonQuery();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Edit(int id, string categoryName)
        {
            SqlCommand command = new SqlCommand("update Categories set CategoryName = @CategoryName where Id = @Id", connection);
            command.Parameters.AddWithValue("@CategoryName", categoryName);
            command.Parameters.AddWithValue("@Id", id);
            connection.Open();
            command.ExecuteNonQuery();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {

            SqlCommand cmd = new SqlCommand("delete from Products where CategoryId = @CategoryId", connection);
            connection.Open();
            cmd.Parameters.AddWithValue("@CategoryId", id);
            cmd.ExecuteNonQuery();

            SqlCommand command = new SqlCommand("delete from Categories where Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);
            command.ExecuteNonQuery();
            connection.Close();

            return RedirectToAction("Index");
        }
    }
}
