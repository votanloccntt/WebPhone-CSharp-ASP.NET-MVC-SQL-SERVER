﻿using Models.DAO;
using Models.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebPhone.Models;

namespace WebPhone.Controllers
{
    public class CartController : Controller
    {
        private const string CartSession = "CartSession";
        // GET: Cart
        public ActionResult Index()
        {
            var cart = Session[CartSession];
            var list = new List<CartItem>();
            if (cart != null)
            {
                list = (List<CartItem>)cart;
            }
            return View(list);
        }
        public JsonResult Delete(int id)
        {
            var sessionCart = (List<CartItem>)Session[CartSession];
            sessionCart.RemoveAll(x => x.Phones.Phones_id == id);
            Session[CartSession] = sessionCart;
            return Json(new
            {
                status = true
            });
        }
        public ActionResult AddItem(int productId, int quantity)
        {
            var product = new PhonesDAO().ViewDetail(productId);
            var cart = Session[CartSession];
            if (cart != null)
            {
                var list = (List<CartItem>)cart;
                if (list.Exists(x => x.Phones.Phones_id == productId))
                {
                    foreach (var item in list)
                    {
                        if (item.Phones.Phones_id == productId)
                        {
                            item.Quantity += quantity;
                        }
                    }
                }
                else
                {
                    var item = new CartItem();
                    item.Phones = product;
                    item.Quantity = quantity;
                    list.Add(item);
                }

            }
            else
            {
                var item = new CartItem();
                item.Phones = product;
                item.Quantity = quantity;
                var list = new List<CartItem>();
                list.Add(item);
                Session[CartSession] = list;
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Payment()
        {
            var cart = Session[CartSession];
            var list = new List<CartItem>();
            if (cart != null)
            {
                list = (List<CartItem>)cart;
            }
            return View(list);
        }

        [HttpPost]
        public ActionResult Payment(string cusname, string cusphone, string address, string email)
        {
            var order = new Order();
            order.Create_date = DateTime.Now;
            order.Delivery_address = address;
            order.Customer.Customer_phone = cusphone;
            order.Customer.Customer_name = cusname;
            order.Customer.Customer_mail = email;


            var id = new OrderDAO().Insert(order);
            var cart = (List<CartItem>)Session[CartSession];
            var detailDAO = new OrderDetailDAO();
            foreach (var item in cart)
            {
                var orderDetail = new Order_detail();
                orderDetail.Phones_id = item.Phones.Phones_id;
                orderDetail.Order_id = id;
                orderDetail.Price = item.Phones.Price;
                orderDetail.Sale_quantity = item.Quantity;
                detailDAO.Insert(orderDetail);
            }
            return Redirect("/hoan-thanh");
        }
        public ActionResult Success()
        {
            return View();
        }
    }
}