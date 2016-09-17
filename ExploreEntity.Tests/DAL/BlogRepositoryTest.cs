using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ExploreEntity.DAL;
using ExploreEntity.Models;
using System.Collections.Generic;
using Moq;
using System.Data.Entity;
using System.Linq;

namespace ExploreEntity.Tests.DAL
{
    [TestClass]
    public class BlogRepositoryTest
    { //set these as getters and setters so youd on't have to type this out every damn time
        Mock<BlogContext> mock_context { get; set; }
        Mock<DbSet<Author>> mock_author_table { get; set; }
        List<Author> author_list { get; set; }


    public void ConnectMocksToDatastore()
        {
            var queryable_list = author_list.AsQueryable(); //casts list as a query
            //Lie to LINQ make it think that our new Queryable list is a Database Table
            mock_author_table.As<IQueryable<Author>>().Setup(m => m.Provider).Returns(queryable_list.Provider);
            mock_author_table.As<IQueryable<Author>>().Setup(m => m.Expression).Returns(queryable_list.Expression);
            mock_author_table.As<IQueryable<Author>>().Setup(m => m.ElementType).Returns(queryable_list.ElementType);
            mock_author_table.As<IQueryable<Author>>().Setup(m => m.GetEnumerator()).Returns(() => queryable_list.GetEnumerator());

            // Have our Author property return our Aueryable List AKA Fake Database table
            mock_context.Setup(c => c.Authors).Returns(mock_author_table.Object);
            //How to define a callback in response to a called method
            mock_author_table.Setup(t => t.Add(It.IsAny<Author>())).Callback((Author a) => author_list.Add(a));//when dbset is called listen for the Add mehtod to be used ...callback
            mock_author_table.Setup(t => t.Remove(It.IsAny<Author>())).Callback((Author a) => author_list.Remove(a));//when dbset is called listen for the Add mehtod to be used ...callback

        }


        [TestInitialize] //Initialize will run all of this before running any tests//
        public void Initialize()
        {
            mock_context = new Mock<BlogContext>();
            mock_author_table = new Mock<DbSet<Author>>();
            author_list = new List<Author>(); 
        }
        [TestCleanup] //cleans up any "shared resources" after a test
        public void 

        [TestMethod]
        public void RepoEnsureCanCreateInstance()
        {
            BlogRepository repo = new BlogRepository();
            Assert.IsNotNull(repo);
        }
        [TestMethod]
        public void RepoEnsureRepoHasContext()
        {
            BlogRepository repo = new BlogRepository();
            BlogContext actual_context = repo.Context;

            Assert.IsInstanceOfType(actual_context, typeof(BlogContext));
        }


        [TestMethod]
        public void RepoEnsureWeHaveNoAuthors()
        {
            //Arrange
            //How to create a new Author first

            // Create Mock BlogContet
            //Mock<BlogContext> mock_context = new Mock<BlogContext>();
            //Mock<DbSet<Author>> mock_author_table = new Mock<DbSet<Author>>(); //connection between c# objects and database set
            //List<Author> author_list = new List<Author>(); //fake in-memory database created only for this test
            //change mock so that its type changes so it can talk to Linq
            //Lie to LINQ make it think that our new Queryable list is a Database Table
            //mock_author_table.As<IQueryable<Author>>().Setup(m => m.Provider).Returns(queryable_list.Provider);
            //mock_author_table.As<IQueryable<Author>>().Setup(m => m.Expression).Returns(queryable_list.Expression);
            //mock_author_table.As<IQueryable<Author>>().Setup(m => m.ElementType).Returns(queryable_list.ElementType);
            //mock_author_table.As<IQueryable<Author>>().Setup(m => m.GetEnumerator()).Returns(queryable_list.GetEnumerator());
            ConnectMocksToDatastore();
            //Have our Author property return our Queryable List AKA Fake Database Table
            BlogRepository repo = new BlogRepository(mock_context.Object);
            //Act
            List<Author> some_authors = repo.GetAuthors();
            int expectedAuthors_count = 0;
            int actualAuthors_count = some_authors.Count;
            //Assert
            Assert.AreEqual(expectedAuthors_count, actualAuthors_count);
        }
        [TestMethod]
        public void ReponsureAddAuthorToDatabase()
        {
            //Arrange
            ConnectMocksToDatastore();
            BlogRepository repo = new BlogRepository(mock_context.Object);
            Author my_author = new Author { FirstName = "Sally", LastName = "Mae", PenName = "Voldemort" }; //Property initializer//
            //Act
            repo.AddAuthor(my_author);
            int actual_author_count = repo.GetAuthors().Count;
            int expected_author_count = 1;

            //Assert
            Assert.AreEqual(expected_author_count, actual_author_count);
        }
        //[TestMethod]
        //public void ReponsureAddAuthorToDatabaseWhyDoesThisPass()
        //{
        //    //Arrange
        //    ConnectMocksToDatastore();
        //    BlogRepository repo = new BlogRepository(mock_context.Object);
        //    Author my_author = new Author { FirstName = "Sally", LastName = "Mae", PenName = "Voldemort" }; //Property initializer//
        //    //Act
        //    repo.AddAuthor(my_author);
        //    ConnectMocksToDatastore();
        //    int actual_author_count = repo.GetAuthors().Count;
        //    int expected_author_count = 1;

        //    //Assert
        //    Assert.AreEqual(expected_author_count, actual_author_count);
        //}
        [TestMethod]
        public void RepoEnsureAddAuthorWithArgs()
        {
            //Arrange
            BlogRepository repo = new BlogRepository(mock_context.Object);
            ConnectMocksToDatastore();
            //Act
            repo.AddAuthor("Terry", "Pratchett", "Pterry");
            //Assert
            List<Author> actual_authors = repo.GetAuthors();
            string actual_author_penname = actual_authors.First().PenName;
            string expected_author_penname = "Pterry";
            Assert.AreEqual(expected_author_penname, actual_author_penname);
        }
        [TestMethod]
        public void RepoEnsureFindAuthorByPenName() //need 2 tests - one to find and one to remove
        {
            //Arrange
            author_list.Add(new Author { AuthorId = 1, FirstName = "Sally", LastName = "Mae", PenName = "Voldemort" }); //without setting this up first, connect mock to databse will have nothing in it
            author_list.Add(new Author { AuthorId = 2, FirstName = "Bob", LastName = "Smith", PenName = "Bosmith" }); 
            author_list.Add(new Author { AuthorId = 3, FirstName = "Jed", LastName = "Clampett", PenName = "Texas Tea" }); 

            BlogRepository repo = new BlogRepository(mock_context.Object);
            ConnectMocksToDatastore();
            //Act
            string pen_name = "voldemort";
            Author actual_author = repo.FindAuthorByPenName(pen_name);
            //Assert
            int expected_author_id = 1;
            int actual_author_id = actual_author.AuthorId;
            Assert.AreEqual(expected_author_id, actual_author_id);
        }
        [TestMethod]
        public void RepoEnsureDeleteAuthorFromDatabase() //will need callback for remove  in test initialize
        {
            author_list.Add(new Author { AuthorId = 1, FirstName = "Sally", LastName = "Mae", PenName = "Voldemort" }); //without setting this up first, connect mock to databse will have nothing in it
            author_list.Add(new Author { AuthorId = 2, FirstName = "Bob", LastName = "Smith", PenName = "Bosmith" });
            author_list.Add(new Author { AuthorId = 3, FirstName = "Jed", LastName = "Clampett", PenName = "Texas Tea" });

            BlogRepository repo = new BlogRepository(mock_context.Object);
            ConnectMocksToDatastore();
            string pen_name = "Bob";
            Author removed_author = repo.RemoveAuthor(pen_name);
            int expected_author_count = 2;
            int actual_author_count = repo.GetAuthors().Count;
            int expected_author_id = 2;
            int actual_author_id = removed_author.AuthorId;
            Assert.AreEqual(expected_author_count, actual_author_count);
            Assert.AreEqual(expected_author_id, actual_author_id);
        }
        [TestMethod]
        public void RepoEnsureICanNotRemoveThingsNotThere()
        {
            author_list.Add(new Author { AuthorId = 1, FirstName = "Sally", LastName = "Mae", PenName = "Voldemort" }); //without setting this up first, connect mock to databse will have nothing in it
            author_list.Add(new Author { AuthorId = 2, FirstName = "Bob", LastName = "Smith", PenName = "Bosmith" });
            author_list.Add(new Author { AuthorId = 3, FirstName = "Jed", LastName = "Clampett", PenName = "Texas Tea" });

            BlogRepository repo = new BlogRepository(mock_context.Object);
            ConnectMocksToDatastore();
            string pen_name = "Harry";
            Author removed_author = repo.RemoveAuthor(pen_name);
            int expected_author_count = 2;
            int actual_author_count = repo.GetAuthors().Count;
            int expected_author_id = 2;
            int actual_author_id = removed_author.AuthorId;
            //Assert
            Assert.IsNull(removed_author);
                  
        }
    }
}
