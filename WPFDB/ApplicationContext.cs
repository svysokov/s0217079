using Microsoft.EntityFrameworkCore;
using ObjectRecognizerLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace WPFDB
{
    public class ImageInfo
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public string Label { get; set; }
        public float Confidence { get; set; }
        public byte[] Image { get; set; }
    }

    public class ApplicationContext : DbContext
    {
        public DbSet<ImageInfo> Images { get; set; }
        public ApplicationContext()
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=imagesdb;Trusted_Connection=True;");
        }
        public void Clear()
        {
            lock(this)
            {
                Images.RemoveRange(Images);
                SaveChanges();
            }
        }
    }
}
