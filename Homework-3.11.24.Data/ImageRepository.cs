using System.Data.SqlClient;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace Homework_3._11._24.Data
{
    public class ImageRepository
    {
        private string _connectionString = @"Data Source=.\sqlexpress; Initial Catalog=HomeWork; Integrated Security=true;";

        public Image GetImage(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = $"SELECT * FROM Images WHERE Id = @id";
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            var reader = command.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            return new Image
            {
                Id = id,
                Password = (string)reader["Password"],
                Views = (int)reader["Views"],
                FileName = (string)reader["FileName"]
            };
        }

        public int Add(Image image)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = $"INSERT INTO Images (Password, Views, FileName) VALUES (@password, @views, @fileName) SELECT SCOPE_IDENTITY()";
            command.Parameters.AddWithValue("@password", image.Password);
            command.Parameters.AddWithValue("@views", 0);
            command.Parameters.AddWithValue("@fileName", image.FileName);


            connection.Open();
            return (int)(decimal)command.ExecuteScalar();
        }

        public void UpdateImageViews(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = $"UPDATE Images SET Views = Views + 1 WHERE Id = @id";
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            command.ExecuteNonQuery();
        }
    }
}
