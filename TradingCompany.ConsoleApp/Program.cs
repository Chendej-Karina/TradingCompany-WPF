using Microsoft.EntityFrameworkCore;
using TradingCompany.DAL;
using TradingCompany.DAL.Data;
using TradingCompany.DAL.DTO;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.InputEncoding = System.Text.Encoding.UTF8;

var options = new DbContextOptionsBuilder<TradingCompanyContext>()
    .UseSqlServer(@"Server=DESKTOP-GC3UDDI\SQLEXPRESS;Database=TradingCompany;Trusted_Connection=True;TrustServerCertificate=True;")
    .Options;

using var context = new TradingCompanyContext(options);

var items = new ItemDal(context);
var auctions = new AuctionDal(context);
var bids = new BidDal(context);
var categories = new CategoryDal(context);
var conditions = new ConditionDal(context);

while (true)
{
    Console.WriteLine();
    Console.WriteLine("===== Менеджер аукціонів =====");
    Console.WriteLine("1. Список товарів");
    Console.WriteLine("2. Пошук товару");
    Console.WriteLine("3. Товари, відсортовані за назвою");
    Console.WriteLine("4. Додати товар");
    Console.WriteLine("5. Деактивувати товар");
    Console.WriteLine("6. Активні аукціони");
    Console.WriteLine("7. Завершені аукціони");
    Console.WriteLine("8. Додати аукціон");
    Console.WriteLine("9. Деактивувати аукціон");
    Console.WriteLine("10. Ставки аукціону");
    Console.WriteLine("11. Додати ставку");
    Console.WriteLine("12. Змінити назву товару (Update)");
    Console.WriteLine("13. Видалити ставку (Delete)");
    Console.WriteLine("0. Вихід");
    Console.Write("Ваш вибір: ");

    var choice = Console.ReadLine();
    Console.WriteLine();

    try
    {
        switch (choice)
        {
            case "1":
                foreach (var i in items.GetAll()) PrintItem(i);
                break;
            case "2":
                Console.Write("Текст для пошуку: ");
                foreach (var i in items.Search(Console.ReadLine() ?? "")) PrintItem(i);
                break;
            case "3":
                foreach (var i in items.GetSortedByName()) PrintItem(i);
                break;
            case "4":
                Console.Write("Назва: ");
                var name = Console.ReadLine() ?? "";
                Console.Write("Опис: ");
                var desc = Console.ReadLine();
                var newItem = new ItemDto
                {
                    Name = name,
                    Description = desc,
                    CategoryId = categories.GetAll().First().Id,
                    ConditionId = conditions.GetAll().First().Id,
                    IsActive = true
                };
                items.Add(newItem);
                Console.WriteLine($"Товар додано, Id = {newItem.Id}");
                break;
            case "5":
                items.Deactivate(ReadInt("Id товару: "));
                Console.WriteLine("Товар деактивовано.");
                break;
            case "6":
                foreach (var a in auctions.GetActive()) PrintAuction(a);
                break;
            case "7":
                foreach (var a in auctions.GetFinished()) PrintAuction(a);
                break;
            case "8":
                var auction = new AuctionDto
                {
                    ItemId = ReadInt("Id товару: "),
                    CurrencyId = ReadInt("Id валюти (1-UAH, 2-USD, 3-EUR): "),
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(ReadInt("Тривалість (днів): ")),
                    StartPrice = ReadInt("Стартова ціна: "),
                    BuyoutPrice = ReadInt("Ціна викупу: "),
                    IsActive = true
                };
                auctions.Add(auction);
                Console.WriteLine($"Аукціон додано, Id = {auction.Id}");
                break;
            case "9":
                auctions.Deactivate(ReadInt("Id аукціону: "));
                Console.WriteLine("Аукціон деактивовано.");
                break;
            case "10":
                foreach (var b in bids.GetByAuction(ReadInt("Id аукціону: ")))
                    Console.WriteLine($"Ставка {b.Id}: {b.Amount} ({b.BidDate:g})");
                break;
            case "11":
                var bid = new BidDto
                {
                    AuctionId = ReadInt("Id аукціону: "),
                    Amount = ReadInt("Сума: "),
                    BidDate = DateTime.Now
                };
                bids.Add(bid);
                Console.WriteLine($"Ставку додано, Id = {bid.Id}");
                break;
            case "12":
                var toUpdate = items.GetById(ReadInt("Id товару: "));
                if (toUpdate == null) { Console.WriteLine("Не знайдено."); break; }
                Console.Write("Нова назва: ");
                toUpdate.Name = Console.ReadLine() ?? toUpdate.Name;
                items.Update(toUpdate);
                Console.WriteLine("Оновлено.");
                break;
            case "13":
                bids.Delete(ReadInt("Id ставки: "));
                Console.WriteLine("Ставку видалено.");
                break;
            case "0":
                return;
            default:
                Console.WriteLine("Невідомий пункт меню.");
                break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("Помилка: " + (ex.InnerException?.Message ?? ex.Message));
    }
}

static int ReadInt(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        if (int.TryParse(Console.ReadLine(), out var v)) return v;
        Console.WriteLine("Введіть ціле число.");
    }
}

static void PrintItem(ItemDto i) =>
    Console.WriteLine($"{i.Id}: {i.Name} | {i.Description} | активний: {i.IsActive}");

static void PrintAuction(AuctionDto a) =>
    Console.WriteLine($"{a.Id}: товар {a.ItemId} | {a.StartDate:d} - {a.EndDate:d} | старт {a.StartPrice}, викуп {a.BuyoutPrice} | активний: {a.IsActive}");