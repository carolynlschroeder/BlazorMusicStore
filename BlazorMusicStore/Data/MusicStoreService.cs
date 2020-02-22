using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BlazorMusicStore.Data
{
    public class MusicStoreService
    {
        private readonly MusicStoreContext _context;

        public MusicStoreService(MusicStoreContext context)
        {
            _context = context;
        }

        public Task<List<Genre>> GetGenresAsync()
        {
            return Task.FromResult(_context.Genre.ToList());

        }

        public Task<List<Album>> GetTopSellingAlbums(int count)
        {
            var c = _context;
            var a = _context.Album;
            return Task.FromResult(_context.Album
                .OrderByDescending(a => a.OrderDetails.Count())
                .Take(count)
                .ToList());
        }

        public Task<Album> GetAlbumAsync(int albumId)
        {
            return Task.FromResult(_context.Album.Include("Genre").Include("Artist").FirstOrDefault(a=>a.AlbumId == albumId));
        }

        public Task<List<Album>> GetAlbumsByGenre(int genreId)
        {
            return Task.FromResult(_context.Album.Where(a => a.GenreId == genreId).ToList());
        }

        public Task<Genre> GetGenre(int genreId)
        {
            return Task.FromResult(_context.Genre.FirstOrDefault(g => g.GenreId == genreId));
        }

        public Task<List<Cart>> GetCartItems(string shoppingCartId)
        {
            return Task.FromResult(_context.Cart.Include("Album").Where(c => c.CartId == shoppingCartId).ToList());
        }

        public async Task AddToCart(int albumId, string shoppingCartId)
        {
            var cartItem = _context.Cart.FirstOrDefault(c => c.CartId == shoppingCartId && c.AlbumId == albumId);
            if (cartItem == null)
            {
                cartItem = new Cart
                {
                    AlbumId = albumId,
                    CartId = shoppingCartId,
                    Count = 1,
                    DateCreated = DateTime.Now
                };
                _context.Cart.Add(cartItem);
            }
            else
            {
                cartItem.Count++;
            }


            await _context.SaveChangesAsync();
        }

        public async Task<string> RemoveFromCart(int recordId)
        {
            var cart = _context.Cart.Include("Album").FirstOrDefault(c => c.RecordId == recordId);

            var name = cart.Album.Title;

            if(cart.Count > 1)
            {
                cart.Count--;
            }
            else
            {
                _context.Cart.Remove(cart);
            }

            await _context.SaveChangesAsync();
            return await Task.FromResult(name);

        }

        public async Task<int> AddOrder(Order order, string shoppingCartId, string promoCode)
        {
            decimal orderTotal = 0;
            var cartItems = await GetCartItems(shoppingCartId);
            if (promoCode != "FREE")
            {
                orderTotal = cartItems.Any() ? cartItems.Sum(c => c.Count * c.Album.Price) : 0;
            }
            order.Total = orderTotal;
            order.OrderDate = DateTime.Now;
            order.Username = "Guest";
            
             _context.Order.Add(order);
            _context.SaveChanges();
            var orderId = order.OrderId;

            foreach (var item in cartItems)
            {
                var orderDetail = new OrderDetail
                {
                    AlbumId = item.AlbumId,
                    OrderId = order.OrderId,
                    UnitPrice = item.Album.Price,
                    Quantity = item.Count
                };

                _context.OrderDetail.Add(orderDetail);

            }

            
           // _context.SaveChanges();

            foreach(var item in cartItems)
            {
                _context.Cart.Remove(item);
            }

            await _context.SaveChangesAsync();
            return await Task.FromResult(orderId);
        }
    }
}
