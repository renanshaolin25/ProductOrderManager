using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using ProductOrderManager.Models;

namespace ProductOrderManager.Controllers
{
    [Authorize]
    [RoutePrefix("api/orders")]
    public class OrdersController : ApiController
    {
        private ProductOrderManagerContext db = new ProductOrderManagerContext();


        // GET: api/Orders
        [Authorize(Roles = "ADMIN")]
        public List<Order> GetOrders()
        {
            return db.Orders.Include(order => order.OrderItems).ToList();
        }

        // GET: api/Orders/5
        [ResponseType(typeof(Order))]
        public IHttpActionResult GetOrder(long id)
        {
            Order order = db.Orders.Find(id);

            if (order == null)
            {
                return BadRequest("Pedido não encontrado!");
            }

            if (User.Identity.Name.Equals(order.email) || User.IsInRole("ADMIN"))
            {
                return Ok(order);
            }
            else
            {
                return BadRequest("Acesso Não Autorizado!");
            }
        }

        // GET: api/Orders/byemail?email=renan@gmail.com
        [ResponseType(typeof(Order))]
        [Route("byemail")]
        [HttpGet]
        public List<Order> GetOrders(string email)
        {
            if (User.Identity.Name.Equals(email) || User.IsInRole("ADMIN"))
            {
                return db.Orders.Where(p => p.email == email).Include(order => order.OrderItems).ToList();
            }
            else
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Acesso Não Autorizado!"));
            }
        }

        // PUT: api/Orders/close/5
        [ResponseType(typeof(Order))]
        [Route("close")]
        [HttpPut]
        public IHttpActionResult CloseOrder(long id, Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != order.Id)
            {
                return BadRequest("Pedido não encontrado!");
            }

            db.Entry(order).State = EntityState.Modified;

            if (User.Identity.Name.Equals(order.email) || User.IsInRole("ADMIN"))
            {
                try
                {
                    if (Decimal.ToDouble(order.freightPrice) == 0.0)
                    {
                        return BadRequest("É necessário calcular o frete antes de fechar o pedido!");
                    }
                    else
                    {
                        order.orderStatus = "fechado";
                        db.SaveChanges();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            else
            {
                return BadRequest("Acesso Não Autorizado!");
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // PUT: api/Orders/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutOrder(long id, Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != order.Id)
            {
                return BadRequest();
            }

            db.Entry(order).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Orders
        [Authorize(Roles = "ADMIN, USER")]
        [ResponseType(typeof(Order))]
        public IHttpActionResult PostOrder(Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            order.orderStatus = "novo";
            order.totalWeight = 0;
            order.freightPrice = 0;
            order.totalPrice = 0;
            order.orderDate = DateTime.Now;

            db.Orders.Add(order);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = order.Id }, order);
        }

        // DELETE: api/Orders/5
        [ResponseType(typeof(Order))]
        public IHttpActionResult DeleteOrder(long id)
        {
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return BadRequest("Pedido não encontrado!");
            }

            if (User.Identity.Name.Equals(order.email) || User.IsInRole("ADMIN"))
            {
                db.Orders.Remove(order);
                db.SaveChanges();
            }
            else
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Acesso Não Autorizado!"));
            }

            return Ok(order);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool OrderExists(long id)
        {
            return db.Orders.Count(e => e.Id == id) > 0;
        }
    }
}