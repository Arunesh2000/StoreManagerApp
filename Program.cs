using System;
using Microsoft.Data.SqlClient;


class Program
{
    static string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=StoreDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";


    static void Main()
    {
        while (true)
        {
            Console.WriteLine("\n--- Store Manager ---");
            Console.WriteLine("1. Add Product");
            Console.WriteLine("2. Update Stock");
            Console.WriteLine("3. Delete Product");
            Console.WriteLine("4. View Products");
            Console.WriteLine("5. Check Stock Alerts");
            Console.WriteLine("0. Exit");
            Console.Write("Choose an option: ");
            string input = Console.ReadLine();

            switch (input)
            {
                case "1": AddProduct(); break;
                case "2": UpdateStock(); break;
                case "3": DeleteProduct(); break;
                case "4": ViewProducts(); break;
                case "5": CheckAlerts(); break;
                case "0": return;
                default: Console.WriteLine("Invalid option."); break;
            }
        }
    }

    static void AddProduct()
    {
        Console.Write("Enter product name: ");
        string name = Console.ReadLine();

        Console.Write("Enter quantity: ");
        if (!int.TryParse(Console.ReadLine(), out int qty))
        {
            Console.WriteLine("Invalid quantity!");
            return;
        }

        using (SqlConnection con = new SqlConnection(connectionString))
        {
            string query = "INSERT INTO Products (Name, Quantity, MinThreshold, MaxThreshold, LastUpdated) VALUES (@name, @qty, 5, 100, GETDATE())";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@qty", qty);
            con.Open();
            cmd.ExecuteNonQuery();
            Console.WriteLine("Product added.");
        }
    }

    static void UpdateStock()
    {
        Console.Write("Enter product ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid product ID!");
            return;
        }

        Console.Write("Enter new quantity: ");
        if (!int.TryParse(Console.ReadLine(), out int qty))
        {
            Console.WriteLine("Invalid quantity!");
            return;
        }

        using (SqlConnection con = new SqlConnection(connectionString))
        {
            string query = "UPDATE Products SET Quantity = @qty, LastUpdated = GETDATE() WHERE ProductID = @id";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@qty", qty);
            cmd.Parameters.AddWithValue("@id", id);
            con.Open();
            cmd.ExecuteNonQuery();
            Console.WriteLine("Stock updated.");
        }
    }

    static void DeleteProduct()
    {
        Console.Write("Enter product ID to delete: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID!");
            return;
        }

        using (SqlConnection con = new SqlConnection(connectionString))
        {
            string query = "DELETE FROM Products WHERE ProductID = @id";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@id", id);
            con.Open();
            cmd.ExecuteNonQuery();
            Console.WriteLine("Product deleted.");
        }
    }

    static void ViewProducts()
    {
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            string query = "SELECT * FROM Products";
            SqlCommand cmd = new SqlCommand(query, con);
            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            Console.WriteLine("\nID | Name         | Qty | Min | Max | LastUpdated");
            while (reader.Read())
            {
                Console.WriteLine($"{reader["ProductID"],-2} | {reader["Name"],-12} | {reader["Quantity"],-3} | {reader["MinThreshold"],-3} | {reader["MaxThreshold"],-3} | {reader["LastUpdated"]}");
            }
        }
    }

    static void CheckAlerts()
    {
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            string query = "SELECT Name, Quantity, MinThreshold, MaxThreshold FROM Products";
            SqlCommand cmd = new SqlCommand(query, con);
            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                string name = reader["Name"].ToString();
                int qty = (int)reader["Quantity"];
                int min = (int)reader["MinThreshold"];
                int max = (int)reader["MaxThreshold"];

                if (qty == 0)
                    Console.WriteLine($"{name} is OUT OF STOCK!");
                else if (qty < min)
                    Console.WriteLine($"{name} is LOW in stock!");
                else if (qty > max)
                    Console.WriteLine($"{name} exceeds MAX stock!");
            }
        }
    }
}
