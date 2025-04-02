﻿using Basket.Core.Entities;

namespace Basket.Application.Responses
{
    public class ShoppingCartResponse
    {
        public string UserName { get; set; }

        public List<ShoppingCartItemResponse> items { get; set; } = new List<ShoppingCartItemResponse>();

        public ShoppingCartResponse()
        {

        }

        public ShoppingCartResponse(string userName)
        {
            UserName = userName;
        }

        public decimal TotalPrice { get
            {
                decimal totalPrice = 0;
                foreach (var item in items)
                {
                    totalPrice += (item.Price * item.Quantity);
                }
                return totalPrice;
            }
        }
    }
}
