using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace OrdersExercise
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //VecchioMain();
            CheckAdmin();
            while (!Login())
            {
                Console.WriteLine("username o password non validi");
            }
            Console.WriteLine("loggato!");




            string input = "";
            while (input != "e")
            {
                Console.WriteLine("inserisci c per creare utente, l per lista ordini, d per dettaglio ordine, o per creare ordine ");
                input = Console.ReadLine();
                input.ToLower();
                switch (input)
                {
                    case "c":
                        if (CreateUser())
                        {
                            Console.WriteLine("Utente creato");
                        }
                        else
                        {
                            Console.WriteLine("Utente non creato");
                        }
                        break;

                    case "l":
                        OrderList();
                        break;

                    case "d":
                        OrderDetails();
                        break;

                    case "o":
                        CreateOrder();
                        break;

                    case "e":
                        return;

                    default:
                        Console.WriteLine("scelta non valida");
                        break;
                }
            }








            Console.ReadKey();
        }

        public static void CheckAdmin()
        {
            using (var ctx = new ordersEntities())
            {
                if (ctx.utentis.Count() <= 0)
                {
                    Console.WriteLine("tabella vuota, creo user admin password admin");
                    var admin = new utenti() { username = "admin", password = "admin" };
                    ctx.utentis.Add(admin);
                    ctx.SaveChanges();
                }
                else
                {
                    Console.WriteLine("esiste già almeno un utente");
                }
            }
        }

        public static bool Login()
        {

            using (var ctx = new ordersEntities())
            {
                Console.WriteLine("inserisci username");
                var userInput = Console.ReadLine();
                var user = ctx.utentis.Find(userInput);
                if (user == null)
                {
                    return false;
                }
                Console.WriteLine("inserisci password");
                userInput = Console.ReadLine();
                if (user.password == userInput)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
        }

        public static bool CreateUser()
        {
            Console.WriteLine("inserisci username per il nuovo utente");
            var userInput = Console.ReadLine();
            Console.WriteLine("inserisci password per il nuovo utente");
            var passwordInput = Console.ReadLine();
            using (var ctx = new ordersEntities())
            {
                utenti user = new utenti() { password = passwordInput, username = userInput };

                try
                {
                    ctx.utentis.Add(user);
                    ctx.SaveChanges();
                    return true;

                }
                catch
                (Exception ex)
                { return false; }


            }
        }

        public static void OrderList()
        {
            using (var ctx = new ordersEntities())
            {
                Console.WriteLine("lista ordini:");
                foreach (var order in ctx.orders)
                {
                    Console.WriteLine($"id: {order.orderid}  - date: {order.orderdate}  - customer: {order.name}");
                }
            }
        }

        public static void OrderDetails()
        {
            int numberInput = 0;
            string orderInput = "";


            Console.WriteLine("inserisci numero ordine valido e in lista");
            orderInput = Console.ReadLine();
            while (!isInOrder(orderInput))
            {
                Console.WriteLine("inserisci numero ordine valido e in lista");
                orderInput = Console.ReadLine();
            }
            Console.WriteLine("id valido, dettagli ordine:");
            numberInput = int.Parse(orderInput);
            using (var ctx = new ordersEntities())
            {
                foreach (var orderitem in ctx.orderitems)
                {
                    if (orderitem.orderid == numberInput)
                    {
                        Console.WriteLine($"item name: {orderitem.name}   - quantity: {orderitem.qty}   - total price: {orderitem.qty * orderitem.price} ");
                    }
                }
            }
        }


        public static bool isNumber(string input)
        {
            int i = 0;
            try
            {
                i = int.Parse(input);
                return true;
            }
            catch (Exception ex) { return false; }
        }

        public static bool isInOrder(string input)
        {
            int number = 0;
            if (!isNumber(input))
                return false;
            else
            {
                number = int.Parse(input);
                using (var ctx = new ordersEntities())
                {
                    var order = ctx.orders.Find(number);
                    if (order == null)
                    {
                        return false;
                    }
                    else
                    { return true; }
                }
            }


        }

        public static void CreateOrder()
        {
            List<string> customerss = new List<string>();
            List<string> itemss = new List<string>();
            string stringInput = "";
            string lastOrder = "";
            int lastOrderNumber = 0;
            int input = 0;
            int currentOrderNumber = 0;
            string scelta = "";
            Console.WriteLine("scegli il tuo cliente");
            using (var ctx = new ordersEntities())
            {
                int j = 0;
                foreach (var customer in ctx.customers)
                {
                    Console.WriteLine($"{j}  {customer.name}   {customer.country}");
                    j += 1;
                    customerss.Add(customer.name);
                }
                stringInput = Console.ReadLine();
                while (!isInList(stringInput, customerss))
                {
                    Console.WriteLine("inserisci numero customer valido e in lista");
                    stringInput = Console.ReadLine();
                }

                input = int.Parse(stringInput);

                int idMax = ctx.orders.Max((order o) => o.orderid); //restituisce l'id massimo, per ogni oggetto orders ottiene la proprietà 
                //order id, poi fa la max sugli id
                //lastOrderNumber = int.Parse(lastOrder);
                currentOrderNumber = idMax += 1;

                order newOrder = new order() { orderid = currentOrderNumber, name = customerss[input], orderdate = DateTime.Now };
                ctx.orders.Add(newOrder);

                ctx.SaveChanges();
                Console.WriteLine("ordine creato");
            }


            //Console.WriteLine("seleziona oggetto: ");
            int i = 0;
            using (var ctx = new ordersEntities())
            {
                foreach (var item in ctx.items)
                {
                    Console.WriteLine($"     {i}  {item.name},   {item.color}");
                    i += 1;
                    itemss.Add(item.name);
                }
                while (scelta != "e")
                {
                    Console.WriteLine("\n seleziona oggetto: ");
                    stringInput = Console.ReadLine();
                    while (!isInList(stringInput, itemss))
                    {
                        Console.WriteLine("inserisci numero item valido e in lista");
                        stringInput = Console.ReadLine();
                    }
                    input = int.Parse(stringInput);
                    //Console.ReadLine();
                    //item item = ctx.items.Find(itemss[i]);
                    Console.WriteLine("inserisci quantità (manca controllo)");
                    int quantita = int.Parse(Console.ReadLine());
                    Console.WriteLine("inserisci prezzo singolo (manca controllo)");
                    int prezzo = int.Parse(Console.ReadLine());

                    orderitem o = new orderitem() { orderid = currentOrderNumber, name = itemss[input], price = prezzo * quantita, qty = quantita };
                    ctx.orderitems.Add(o);
                    Console.WriteLine("inserire e per uscire, un tasto per inserire altro item nell'ordine");
                    scelta = Console.ReadLine();
                }

                ctx.SaveChanges();
                Console.WriteLine($"items aggiunti all'ordine {currentOrderNumber}");
            }





        }



        public static bool isInList(string input, List<string> list)
        {
            int number = 0;
            if (!isNumber(input))
                return false;
            else
            {
                number = int.Parse(input);

                {
                    if (number < list.Count() && number >= 0)
                    {
                        return true;
                    }
                    else
                    { return false; }
                }
            }

        }






















        private static void VecchioMain()
        {
            using (var ctx = new ordersEntities())
            {
                Console.WriteLine("clienti:");

                foreach (var customer in ctx.customers)
                {
                    Console.WriteLine($"{customer.name}  {customer.country}");
                }
                Console.WriteLine("\n\n items:");
                foreach (var item in ctx.items)
                {
                    Console.WriteLine($"{item.name}  {item.color}");
                }
                Console.WriteLine("\n \n Orders: ");
                foreach (var order in ctx.orders)
                {
                    Console.WriteLine();
                    Console.WriteLine($"{order.orderid}  {order.orderdate}  {order.name}  {order.customer.country}");
                    Console.WriteLine(" item in this order:");
                    foreach (var orderitem in order.orderitems)
                    {

                        Console.WriteLine($"      {orderitem.item.name}  {orderitem.qty} {orderitem.price}");
                    }
                }

                foreach (customer c in ctx.customers)
                {
                    if (c.name == "Bob")
                    {
                        Console.WriteLine($"aggiorno {c.country}");
                        // c.name = c.name + "(modifcato)";
                        c.country = c.country + "(m)";
                        Console.WriteLine($"ecco {c.country}");
                    }


                }
                ctx.SaveChanges();
                Console.WriteLine("cambiamenti applicati");

                var cust = new customer() { name = "Matta", country = "Italy" };
                ctx.customers.Add(cust);
                ctx.SaveChanges();
                Console.WriteLine($"customer {cust.name} inserito");

                foreach (customer c in ctx.customers)
                {
                    if (c.name == "Matta")
                    {
                        Console.WriteLine($"rimuovo {c.name}");
                        // c.name = c.name + "(modifcato)";
                        ctx.customers.Remove(c);

                    }


                }
                ctx.SaveChanges();
                var cc = ctx.customers.Find("Jack");
                Console.WriteLine(cc.name);

            }
            Console.ReadKey();
        }
    }
}
