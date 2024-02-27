using System.Data;
using System.Data.SqlClient;

namespace BlogSite.Data
{
    public class BlogDb
    {
        private string _connectionString;

        public BlogDb(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<BlogPost> GetPosts(int skip, int amount)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = "GetPosts";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@skip", skip);
            command.Parameters.AddWithValue("@amount", amount);
            connection.Open();
            List<BlogPost> posts = new List<BlogPost>();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                BlogPost post = GetFromReader(reader);
                posts.Add(post);
            }

            return posts;
        }

        public int GetPostsCount()
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT Count(*) FROM Posts";
            connection.Open();
            return (int)command.ExecuteScalar();
        }

        public void AddPost(BlogPost post)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Posts (Title, BlogContent, DateCreated) " +
                                  "VALUES (@title, @content, @date) SELECT SCOPE_IDENTITY()";
            command.Parameters.AddWithValue("@title", post.Title);
            command.Parameters.AddWithValue("@content", post.Content);
            command.Parameters.AddWithValue("@date", post.DateCreated);
            connection.Open();
            post.Id = (int)(decimal)command.ExecuteScalar();
        }

        public BlogPost GetPost(int postId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Posts WHERE Id = @id";
            command.Parameters.AddWithValue("@id", postId);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            return GetFromReader(reader);
        }

        private BlogPost GetFromReader(SqlDataReader reader)
        {
            var post = new BlogPost();
            post.Id = (int)reader["Id"];
            post.Title = (string)reader["Title"];
            post.Content = (string)reader["BlogContent"];
            post.DateCreated = (DateTime)reader["DateCreated"];
            return post;
        }

        public void AddComment(Comment comment)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Comments (Name, Comment, DateCreated, PostId) " +
                                  "VALUES (@name, @comment, @date, @postId)";
            command.Parameters.AddWithValue("@name", comment.Name);
            command.Parameters.AddWithValue("@comment", comment.Content);
            command.Parameters.AddWithValue("@date", comment.DateCreated);
            command.Parameters.AddWithValue("@postId", comment.PostId);
            connection.Open();
            command.ExecuteNonQuery();
        }

        public List<Comment> GetComments(int postId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Comments WHERE PostId = @postId";
            command.Parameters.AddWithValue("@postId", postId);
            connection.Open();
            List<Comment> comments = new List<Comment>();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                Comment comment = new Comment();
                comment.Id = (int)reader["Id"];
                comment.Name = (string)reader["Name"];
                comment.Content = (string)reader["Comment"];
                comment.DateCreated = (DateTime)reader["DateCreated"];
                comment.PostId = (int)reader["PostId"];
                comments.Add(comment);
            }

            return comments;
        }

        public int GetMostRecentId()
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT Top 1 Id FROM Posts ORDER BY DateCreated DESC";
            connection.Open();
            return (int)command.ExecuteScalar();
        }
    }
}