using ExploreEntity.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ExploreEntity.DAL
{
    public class BlogContext : DbContext
    {

        //virtual keyword needed to use Moq during testing
        public virtual DbSet<PublishedWork> Works { get; set; }
        public virtual DbSet<CitationStyle> CitationStyle { get; set; }
        public virtual DbSet<Author> Authors { get; set; }
    }
}