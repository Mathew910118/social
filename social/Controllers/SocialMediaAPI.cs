using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;

namespace SocialMediaAPI
{
    public class Post
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string ImageUrl { get; set; }
        public string Link { get; set; }
        public List<string> Reactions { get; set; }
        public List<Comment> Comments { get; set; }
    }

    public class Comment
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string Author { get; set; }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            // Define the API routes
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "api/{controller}/{action}/{id?}");
            });

            app.Run();
        }
    }

    [ApiController]
    [Route("api/posts")]
    public class PostsController : ControllerBase
    {
        private static List<Post> posts = new List<Post>();
        private static string instructionFilePath = "Instruction.txt";

        [HttpGet]
        public IActionResult GetPosts(string filter = null, string sortBy = null)
        {
            // Apply filtering if specified
            if (!string.IsNullOrEmpty(filter))
            {
                // Implement filtering logic based on the provided parameters
                // Example: filter by author, date, keywords, etc.
                // FilteredPosts = posts.Where(...)   
                // return FilteredPosts;
            }

            // Apply sorting if specified
            if (!string.IsNullOrEmpty(sortBy))
            {
                // Implement sorting logic based on the provided parameter
                // Example: sort by date, popularity, etc.
                // SortedPosts = FilteredPosts.OrderBy(...)
                // return SortedPosts;
            }

            return Ok(posts);
        }

        [HttpGet("{id}")]
        public IActionResult GetPost(int id)
        {
            var post = posts.Find(p => p.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return Ok(post);
        }

        [HttpPost]
        public IActionResult CreatePost(Post post)
        {
            post.Id = GetNewPostId();
            posts.Add(post);
            return CreatedAtAction(nameof(GetPost), new { id = post.Id }, post);
        }

        [HttpPut("{id}")]
        public IActionResult UpdatePost(int id, Post updatedPost)
        {
            var post = posts.Find(p => p.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            post.Text = updatedPost.Text;
            post.ImageUrl = updatedPost.ImageUrl;
            post.Link = updatedPost.Link;
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeletePost(int id)
        {
            var post = posts.Find(p => p.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            posts.Remove(post);
            return NoContent();
        }

        [HttpPost("{id}/reactions")]
        public IActionResult AddReactionToPost(int id, string reaction)
        {
            var post = posts.Find(p => p.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            post.Reactions.Add(reaction);
            return NoContent();
        }

        private int GetNewPostId()
        {
            // Generate a new unique ID for a new post
            return posts.Count + 1;
        }

        [HttpGet("instructions")]
        public ActionResult<string> GetInstructions()
        {
            if (System.IO.File.Exists(instructionFilePath))
            {
                var instructions = System.IO.File.ReadAllText(instructionFilePath);
                return Ok(instructions);
            }
            else
            {
                return NotFound("Návod nebyl nalezen.");
            }
        }
    }
}
