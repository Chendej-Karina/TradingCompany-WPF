using TradingCompany.ViewModels.Basic;

namespace TradingCompany.Tests.ViewModels;

public class AsyncRelayCommandTests
{
    [Fact]
    public async Task ExecuteAsync_RunsAction()
    {
        var called = false;
        var command = new AsyncRelayCommand(() => { called = true; return Task.CompletedTask; });

        await command.ExecuteAsync();

        Assert.True(called);
        Assert.False(command.IsRunning);
    }

    [Fact]
    public async Task CanExecute_IsFalseWhileRunning()
    {
        var gate = new TaskCompletionSource();
        var command = new AsyncRelayCommand(() => gate.Task);

        var running = command.ExecuteAsync();
        Assert.True(command.IsRunning);
        Assert.False(command.CanExecute(null));

        gate.SetResult();
        await running;
        Assert.True(command.CanExecute(null));
    }

    [Fact]
    public async Task SecondCallWhileRunning_IsIgnored()
    {
        var gate = new TaskCompletionSource();
        var count = 0;
        var command = new AsyncRelayCommand(() => { count++; return gate.Task; });

        var first = command.ExecuteAsync();
        await command.ExecuteAsync();
        gate.SetResult();
        await first;

        Assert.Equal(1, count);
    }

    [Fact]
    public async Task CanExecuteDelegate_IsRespected()
    {
        var called = false;
        var command = new AsyncRelayCommand(() => { called = true; return Task.CompletedTask; }, () => false);

        await command.ExecuteAsync();

        Assert.False(command.CanExecute(null));
        Assert.False(called);
    }

    [Fact]
    public async Task Exception_IsPassedToErrorHandler_AndCommandIsReleased()
    {
        Exception? caught = null;
        var command = new AsyncRelayCommand(() => throw new InvalidOperationException("boom"), onError: ex => caught = ex);

        await command.ExecuteAsync();

        Assert.IsType<InvalidOperationException>(caught);
        Assert.False(command.IsRunning);
    }

    [Fact]
    public async Task CanExecuteChanged_IsRaised()
    {
        var raised = 0;
        var command = new AsyncRelayCommand(() => Task.CompletedTask);
        command.CanExecuteChanged += (_, _) => raised++;

        await command.ExecuteAsync();

        Assert.Equal(2, raised); // на початку та в кінці виконання
    }
}
