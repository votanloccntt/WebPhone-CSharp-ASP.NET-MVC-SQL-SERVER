﻿using Models.EntityFramework;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DAO
{
    public class OrderDAO
    {
        WebPhoneDbContext db = null;
        public OrderDAO()
        {
            db = new WebPhoneDbContext();
        }
        public int Insert(Order order)
        {
            db.Order.Add(order);
            db.SaveChanges();
            return order.Order_id;
        }
        public IEnumerable<Order> ListAllPaging(string sreachString, int page, int pageSize)
        {
            IQueryable<Order> model = db.Order;
            if (!string.IsNullOrEmpty(sreachString))
            {
                model = model.Where(x => x.Customer.Customer_name.Contains(sreachString) || x.Delivery_address.Contains(sreachString)
            );
            }
            return model.OrderByDescending(x => x.Order_id).ToPagedList(page, pageSize);
        }
        public Order ViewDetail(int id)
        {
            return db.Order.Find(id);
        }
        public bool Update(Order entity)
        {
            var order = db.Order.Find(entity.Order_id);
            order.Status_id = entity.Status_id;
            db.SaveChanges();
            return true;
        }
        public int CountOrder()
        {
            var orders = db.Order.ToList();
            return orders.Count();
        }
        public  int ChangeStatus(long id)
        {
            var order = db.Order.Find(id);
            if(order.Status_id==1)
            {
                order.Status_id = 0;
            }
            else
            {
                order.Status_id = 1;
            }
            db.SaveChanges();
            return order.Status_id;
        }
    }
}
