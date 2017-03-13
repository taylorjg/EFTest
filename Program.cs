using System;
using System.Linq;
using System.Collections.Generic;

namespace EFTest
{
    using Handler = Func<MyDbContext, bool>;

    class Program
    {
        static void Main(string[] args)
        {
            var table = new Dictionary<Char, Handler>() {
                { 'l', NoQuit(ListRecords)},
                { 'c', NoQuit(CreateRecord)},
                { 'd', NoQuit(DeleteRecord)},
                { 'u', NoQuit(UpdateRecord)},
                { 'q', _ => true},
            };

            for (;;)
            {
                Console.WriteLine();
                Console.WriteLine("Enter a command:");
                Console.WriteLine();
                Console.WriteLine("l: list records");
                Console.WriteLine("c: create record");
                Console.WriteLine("d: delete record");
                Console.WriteLine("u: update record");
                Console.WriteLine("q: quit");
                Console.WriteLine();
                Console.Write("> ");

                var input = Console.ReadLine();
                var command = input.FirstOrDefault();
                Handler handler;
                if (!table.TryGetValue(command, out handler))
                {
                    Console.Error.WriteLine($"Unknown command, '{command}'.");
                    continue;
                }

                using (var db = new MyDbContext())
                {
                    if (handler(db))
                        return;
                }
            }
        }

        static void ListRecords(MyDbContext db)
        {
            foreach (var thing in db.Things)
                DumpRecord(thing);
        }

        static void CreateRecord(MyDbContext db)
        {
            Console.Write("First name: ");
            var firstName = Console.ReadLine();

            Console.Write("Last name: ");
            var lastName = Console.ReadLine();

            Console.Write("Email: ");
            var email = Console.ReadLine();

            var record = new Thing {
                FirstName = firstName,
                LastName = lastName,
                Email = email
            };

            db.Things.Add(record);            
            db.SaveChanges();
            DumpRecord(record);
        }

        static void DeleteRecord(MyDbContext db)
        {
            Console.Write("Enter the id of the record to delete: ");
            var input = Console.ReadLine();

            var id = int.Parse(input);
            var record = db.Things.Find(id);

            if (record == null)
            {
                Console.Error.WriteLine($"Failed to find a record with id, '{id}'.");
                return;
            }

            db.Things.Remove(record);
            db.SaveChanges();
        }

        static void UpdateRecord(MyDbContext db)
        {
            Console.Write("Enter the id of the record to update: ");
            var input = Console.ReadLine();

            var id = int.Parse(input);
            var record = db.Things.Find(id);
            
            if (record == null)
            {
                Console.Error.WriteLine($"Failed to find a record with id, '{id}'.");
                return;
            }

            Console.Write($"First name [{record.FirstName}]: ");
            var firstName = Console.ReadLine();

            Console.Write($"Last name [{record.LastName}]: ");
            var lastName = Console.ReadLine();

            Console.Write($"Email [{record.Email}]: ");
            var email = Console.ReadLine();

            record.FirstName = firstName.Length > 0 ? firstName : record.FirstName;
            record.LastName = lastName.Length > 0 ? lastName : record.LastName;
            record.Email = email.Length > 0 ? email : record.Email;

            db.SaveChanges();
            DumpRecord(record);
        }

        static Handler NoQuit(Action<MyDbContext> handler)
        {
            return db =>
            {
                handler(db);
                return false;
            };
        }

        static void DumpRecord(Thing thing)
        {
            Console.WriteLine($"id: {thing.Id}; fn: {thing.FirstName}; ln: {thing.LastName}; e: {thing.Email}");
        }
    }
}
