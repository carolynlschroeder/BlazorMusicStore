using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BlazorMusicStore.Data
{
    public partial class MusicStoreContext : DbContext
    {
        public virtual DbSet<Album> Album{ get; set; }
        public virtual DbSet<Artist> Artist{ get; set; }
        public virtual DbSet<Cart> Cart{ get; set; }
        public virtual DbSet<Genre> Genre{ get; set; }
        public virtual DbSet<Order> Order{ get; set; }
        public virtual DbSet<OrderDetail> OrderDetail{ get; set; }

        public MusicStoreContext(DbContextOptions<MusicStoreContext> options) : base(options)
        {
        }
    }
}
