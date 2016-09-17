using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ExploreEntity.Models;

namespace ExploreEntity.DAL
{
    public class BlogRepository
    {
        public BlogContext Context { get; set; }
        public BlogRepository()
        {
            Context = new BlogContext();
        }

        public BlogRepository(BlogContext _context) //dependency injection
        {
            Context = _context;
        }

        public List<Author> GetAuthors() //not the same as DbSet<Authors> so we have to leverage other DbSet methods
        {
            return Context.Authors.ToList(); //Context.Authors.ToList<Author>(); Same thing
        }
        public void AddAuthor(Author my_author)
        {
            Context.Authors.Add(my_author);
            Context.SaveChanges();
        }
        public void AddAuthor(string first_name, string last_name, string penname)
        {
            Author author = new Author { FirstName = first_name, LastName = last_name, PenName = penname };
            Context.Authors.Add(author);
            Context.SaveChanges();
        }

        public Author FindAuthorByPenName(string pen_name)
        {
            //inefficient!!!!!!!!!!!!! See note below
        //much faster to use LINQ to generate something like:
        //SELECT * FROM Authors WHERE PenName == pen_name;
        //SQL databases are designed to find and iterate over these rows very efficiently
            List<Author> found_authors = Context.Authors.ToList();
            foreach (var author in found_authors)
            {
                if (author.PenName.ToLower() == pen_name.ToLower())
                {
                    return author;
                }
            }
            return null;
        }

        public Author RemoveAuthor(string pen_name)
        {
            Author found_author = FindAuthorByPenName(pen_name); //re-using method from above to find the instance of the author before we remove it
            if (found_author != null)
            {
                Context.Authors.Remove(found_author);
                Context.SaveChanges();
            }
            return found_author;
        }
    }
}