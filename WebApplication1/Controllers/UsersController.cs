using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;

namespace WebApplication1.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    [HttpGet]
    public IEnumerable<User> GetUsers() => users;

    [HttpPost]
    public IActionResult AddUser(User user)
    {
        user.Id = nextUserId++;
        users.Add(user);
        return CreatedAtAction(nameof(GetUsers), new { id = user.Id }, user);
    }

    [HttpDelete("{id:int}")]
    public IActionResult DeleteUser(int id)
    {   var userToRemove = users.FirstOrDefault(u => u.Id == id);
        if (userToRemove == null)
        {
            return NotFound();
        }
        var updatedUsers = users.Where(u => u.Id != id).ToList();
        users = new ConcurrentBag<User>(updatedUsers);
        return NoContent();
    }

    [HttpDelete("{name:alpha}")]
    public IActionResult DeleteUserByName(string name)
    {
        var userToRemove = users.FirstOrDefault(u => u.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (userToRemove == null)
        {
            return NotFound("Who is that?!");
        }
        var updatedUsers = users.Where(u => !u.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).ToList();
        users = new ConcurrentBag<User>(updatedUsers);
        return NoContent();
    }


    private static int nextUserId = 4;
    private static ConcurrentBag<User> users = new ConcurrentBag<User>
    {
        new User { Id = 1, Name = "Alice", FavoriteFruit = "Apple", Age = 30 },
        new User { Id = 2, Name = "Bob", FavoriteFruit = "Banana", Age = 25 },
        new User { Id = 3, Name = "Charlie", FavoriteFruit = "Cherry", Age = 35 }
    };
}

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? FavoriteFruit { get; set; }
    public int Age { get; set; }
}
