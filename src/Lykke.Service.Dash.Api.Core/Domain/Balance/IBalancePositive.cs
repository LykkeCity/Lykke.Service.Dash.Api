namespace Lykke.Service.Dash.Api.Core.Domain
{
    public interface IBalancePositive
    {
        string Address { get; }
        decimal Amount { get; }
        long Block { get; }
    }
}
