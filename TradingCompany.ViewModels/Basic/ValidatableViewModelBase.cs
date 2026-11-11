using System.Collections;
using System.ComponentModel;

namespace TradingCompany.ViewModels.Basic;

public abstract class ValidatableViewModelBase : ObservableObject, INotifyDataErrorInfo
{
    private readonly Dictionary<string, List<string>> _errors = new();

    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    public bool HasErrors => _errors.Count > 0;

    public IEnumerable GetErrors(string? propertyName)
    {
        if (propertyName != null && _errors.TryGetValue(propertyName, out var list))
            return list;
        return Array.Empty<string>();
    }

    protected void SetErrors(string propertyName, IEnumerable<string> messages)
    {
        var list = messages.ToList();
        if (list.Count == 0)
        {
            ClearErrors(propertyName);
            return;
        }

        _errors[propertyName] = list;
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        OnPropertyChanged(nameof(HasErrors));
    }

    protected void ClearErrors(string propertyName)
    {
        if (_errors.Remove(propertyName))
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            OnPropertyChanged(nameof(HasErrors));
        }
    }

    protected void ClearAllErrors()
    {
        foreach (var key in _errors.Keys.ToList()) ClearErrors(key);
    }
}
