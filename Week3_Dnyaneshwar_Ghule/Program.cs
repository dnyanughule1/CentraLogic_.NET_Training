using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography.X509Certificates;

class Item
{
    public static int lastID = 0; //for update id's when item was deleted

    public int ID { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }

    public Item(string name, decimal price, int quantity)
    {
        lastID++;
        ID = lastID;
        Name = name;
        Price = price;
        Quantity = quantity;
    }
}

class Inventory
{
    private List<Item> items; //List for store data
    public static int cnt = 0; //to count how many items is added
    public Inventory()
    {
        items = new List<Item>();
    }
    private static bool ValidateName(string name) //validate name
    {
        return !string.IsNullOrWhiteSpace(name) && !string.IsNullOrEmpty(name);
    }
    private static bool ValidatePrice(decimal price) //validate price
    {
        return price >= 0;
    }
    private static bool ValidateQuantity(int quantity) //validate quantity
    {
        return quantity >= 0;
    }


    public void AddItem() //function for add item
    {
        Console.Write("Enter item name: ");
        string name = Console.ReadLine();

        while (!ValidateName(name))
        {
            Console.WriteLine("Please enter a valid name. Press 1 to continue or 2 to exit.");

            int num;
            if (int.TryParse(Console.ReadLine(), out num))
            {
                switch (num)
                {
                    case 1:
                        Console.Write("Enter item name: ");
                        name = Console.ReadLine();
                        break;
                    case 2:
                        return;
                    default:
                        Console.WriteLine("Please enter a valid input.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Please enter a valid input.");
            }
        }
        Console.Write("Enter item price: ");
        decimal price = decimal.Parse(Console.ReadLine());

        while (!ValidatePrice(price))
        {
            Console.WriteLine("Please enter a valid name. Press 1 to continue or 2 to exit.");

            int num;
            if (int.TryParse(Console.ReadLine(), out num))
            {
                switch (num)
                {
                    case 1:
                        Console.Write("Enter item price: ");
                        price = decimal.Parse(Console.ReadLine());
                        break;
                    case 2:
                        return;
                    default:
                        Console.WriteLine("Please enter a valid input.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Please enter a valid input.");
            }
        }

        Console.Write("Enter item quantity:");

        int quantity;
        while (!int.TryParse(Console.ReadLine(), out quantity) || !ValidateQuantity(quantity))
        {
            Console.WriteLine("Please enter a valid price. Press 1 to continue or 2 to exit.");

            int num;
            if (int.TryParse(Console.ReadLine(), out num))
            {
                switch (num)
                {
                    case 1:
                        Console.Write("Enter item quantity: ");
                        break;
                    case 2:
                        return;
                    default:
                        Console.WriteLine("Please enter a valid input.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Please enter a valid input.");
            }
        }


        Item newItem = new Item(name, price, quantity);
        items.Add(newItem);
        Console.WriteLine("Item added successfully!!!");
        cnt++;
    }

    public void Display() //function for display item
    {
        if (Item.lastID == 0)
        {
            Console.WriteLine("No item to show please add item");
        }
        else
        {
            foreach (var item in items)
            {
                Console.WriteLine($"{item.ID}. Name: {item.Name},  Price: ? {item.Price},  Quantity: {item.Quantity}");
            }
        }
    }

    public Item FindItem(int id)
    {
        return items.Find(item => item.ID == id);
    }

    public void UpdateItem(int id, string newName, decimal newPrice, int newQuantity) //for update item
    {
        Item iUpdate = FindItem(id);
        if (iUpdate != null)
        {
            iUpdate.Name = newName;
            iUpdate.Price = newPrice;
            iUpdate.Quantity = newQuantity;
            Console.WriteLine("Item updated successfully!!!");
        }
        else
        {
            Console.WriteLine("Item not found.");
        }
    }

    public void UpdateID() //this function update new id's
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i].ID = i + 1;
        }
    }
    public void DeleteItem(int id) //for delete item
    {
        Item removeItem = FindItem(id);
        if (items.Count == 0)
        {
            Console.WriteLine("Inventary is empty!!!");
        }
        if (removeItem != null)
        {
            items.Remove(removeItem);
            Console.WriteLine("Item deleted successfully!!!");
            UpdateID();
        }
        else
        {
            Console.WriteLine("Item not found..");
        }
    }
}

internal class Program
{
    public static void Main()
    {
        Inventory i = new Inventory();
        Console.WriteLine("Inventary Managment System");
        while (true)
        {
            Console.WriteLine("1. Add Item");
            Console.WriteLine("2. Display Items");
            Console.WriteLine("3. Find Item by ID");
            Console.WriteLine("4. Update Item");
            Console.WriteLine("5. Delete Item");
            Console.WriteLine("6. Exit");
            try
            {
                Console.Write("Enter your choice: ");

                int choice = Convert.ToInt32(Console.ReadLine());

                switch (choice)
                {
                    case 1:
                        i.AddItem();
                        break;
                    case 2:
                        Console.Write("-------------------------------------------------");
                        Console.WriteLine("\nInventory: ");
                        i.Display();
                        Console.WriteLine("-------------------------------------------------");
                        break;
                    case 3:
                        Console.WriteLine("\nFinding item by ID:");
                        Console.Write("Enter Item ID to search: ");
                        int sId = Convert.ToInt32(Console.ReadLine());

                        Item fItem = i.FindItem(sId);
                        if (fItem != null)
                        {
                            Console.WriteLine("-------------------------------------------------");
                            Console.WriteLine($"ID: {fItem.ID}, Name: {fItem.Name}, Price: {fItem.Price}, Quantity: {fItem.Quantity}");
                            Console.WriteLine("-------------------------------------------------");

                        }
                        else
                        {
                            Console.WriteLine("Item not found...");
                        }
                        break;
                    case 4:
                        Console.WriteLine("\nUpdating an item:");
                        Console.Write("Enter Item ID to update: ");
                        int uId = Convert.ToInt32(Console.ReadLine());
                        if (uId <= Inventory.cnt) //if entered id is present in our data then only update
                        {
                            Console.Write("Enter new Item Name: ");
                            string newName = Console.ReadLine();
                            Console.Write("Enter new Item Price: ");
                            decimal newPrice = Convert.ToDecimal(Console.ReadLine());
                            Console.Write("Enter new Item Quantity: ");
                            int newQuantity = Convert.ToInt32(Console.ReadLine());

                            i.UpdateItem(uId, newName, newPrice, newQuantity);
                        }
                        else
                        {
                            Console.WriteLine("No item to update.");
                        }
                        break;
                    case 5:
                        Console.WriteLine("\nDeleting an item:");
                        Console.Write("Enter Item ID to delete:");
                        int dId = Convert.ToInt32(Console.ReadLine());
                        i.DeleteItem(dId);
                        break;
                    case 6:
                        Console.WriteLine("You Are Exited");
                        return;
                    default:
                        Console.WriteLine("Invalid choice!!! Please Enter Valid Number");
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Please Enter Valid Data");
            }
        }
    }
}