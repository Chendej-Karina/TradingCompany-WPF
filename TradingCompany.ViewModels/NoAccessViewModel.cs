using TradingCompany.BLL.Security;
using TradingCompany.ViewModels.Basic;

namespace TradingCompany.ViewModels;


public class NoAccessViewModel : ObservableObject
{
    public NoAccessViewModel(IUserSession session)
    {
        Message = $"Користувач «{session.CurrentUser?.Login}» не має ролі, яка дає доступ до функціональності програми. " +
                  "Зверніться до адміністратора.";
    }

    public string Message { get; }
}
