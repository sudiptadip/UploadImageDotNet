using Microsoft.EntityFrameworkCore;
using UploadImage.Models;

namespace UploadImage.Data
{
    public class ApplicationDbConteext : DbContext
    {
        public ApplicationDbConteext(DbContextOptions<ApplicationDbConteext> options) : base(options)
        {

        }

        public DbSet<Product> Products { get; set; }

    }
}
