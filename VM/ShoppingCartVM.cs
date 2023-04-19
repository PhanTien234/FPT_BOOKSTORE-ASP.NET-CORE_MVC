using FPT_BOOKSTORE.Models;

namespace FPT_BOOKSTORE.VM;

public class ShoppingCartVM
{
    public IEnumerable<Cart> ListCarts { get; set; }
    public Order Order { get; set; }
}