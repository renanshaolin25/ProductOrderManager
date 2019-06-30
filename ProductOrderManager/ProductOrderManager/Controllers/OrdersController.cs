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
using ProductOrderManager.br.com.correios.ws;
using ProductOrderManager.CRMClient;

namespace ProductOrderManager.Controllers
{
    [Authorize]
    [RoutePrefix("api/orders")]
    public class OrdersController : ApiController
    {
        private ProductOrderManagerContext db = new ProductOrderManagerContext();
        decimal totalWidth;
        decimal maxHeight;
        decimal maxLength;
        decimal maxDiameter;

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

            if (!isAuthenticated(order.email))
            {
                return BadRequest("Acesso não autorizado!");
            }

            return Ok(order);
  
        }

        // GET: api/Orders/byemail?email=renan@gmail.com
        [ResponseType(typeof(Order))]
        [Route("byemail")]
        [HttpGet]
        public List<Order> GetOrders(string email)
        {
            if (!isAuthenticated(email))
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Acesso não autorizado!"));
            }

            return db.Orders.Where(p => p.email == email).Include(order => order.OrderItems).ToList();
            
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


            if (!isAuthenticated(order.email))
            {
                return BadRequest("Acesso não autorizado!");
            }

            if (order.freightPrice == 0)
            {
                return BadRequest("É necessário calcular o frete antes de fechar o pedido!");
            }

            db.Entry(order).State = EntityState.Modified;  
            try
            {
                order.orderStatus = "fechado";
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

            if (!isAuthenticated(order.email))
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Acesso não autorizado!"));
            }
            
            db.Orders.Remove(order);
            db.SaveChanges();

            return Ok(order);
        }

        // PUT: api/Orders/frete?id=5
        [ResponseType(typeof(string))]
        [HttpPut]
        [Route("frete")]
        public IHttpActionResult CalculaFrete(long id, Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != order.Id)
                return BadRequest("Pedido não encontrado!");

            
            CRMRestClient crmClient = new CRMRestClient();
            Customer customer = crmClient.GetCustomerByEmail(User.Identity.Name);
            CalcPrecoPrazoWS correios = new CalcPrecoPrazoWS();
            order.totalPrice = 0;
            order.totalWeight = 0;
            totalWidth = 0;
            maxHeight = 0;
            maxLength = 0;
            maxDiameter = 0;

            if (!isAuthenticated(order.email))
                return BadRequest("Acesso não autorizado!");

            if (customer == null)
                return BadRequest("Falha ao consultar o CRM");

            if (order.OrderItems.Count == 0)
                return BadRequest("Pedido não contem itens!");

            if (!order.orderStatus.Equals("novo"))
                return BadRequest("Pedido com status diferente de novo!");

            getFreightParameters(order);
            
            cResultado resultado = correios.CalcPrecoPrazo("", "", " 04014", "04236094", customer.zip.Replace("-",""), Convert.ToString(order.totalWeight), 1, maxLength, maxHeight, totalWidth, maxDiameter, "N", order.totalPrice, "S");

            if (!resultado.Servicos[0].Erro.Equals("0"))
                return BadRequest("Falha na consulta dos correios, erro: " + resultado.Servicos[0].Erro + "-" + resultado.Servicos[0].MsgErro);

            order.freightPrice = Decimal.Parse(resultado.Servicos[0].Valor.Replace(",", "."));

            order.deliveryDate = DateTime.Now.AddDays(Double.Parse(resultado.Servicos[0].PrazoEntrega));

            order.totalPrice = order.totalPrice + order.freightPrice;

            string result = "Valor do frete: " + order.freightPrice + " - Prazo de entrega até : " + order.deliveryDate;

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

            return Ok(result);

        }

        private void getFreightParameters(Order order)
        {
            foreach (OrderItem orderItem in order.OrderItems)
            {
                if(orderItem.Product.height > maxHeight)
                    maxHeight = orderItem.Product.height;
                
                if(orderItem.Product.length > maxLength)
                    maxLength = orderItem.Product.length;

                if (orderItem.Product.diameter > maxDiameter)
                    maxDiameter = orderItem.Product.diameter;

                order.totalPrice += orderItem.Product.price;
                order.totalWeight += orderItem.Product.weight;
                totalWidth += orderItem.Product.width;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool isAuthenticated(string email)
        {
            return User.Identity.Name.Equals(email) || User.IsInRole("ADMIN");
        }

        private bool OrderExists(long id)
        {
            return db.Orders.Count(e => e.Id == id) > 0;
        }
    }
}