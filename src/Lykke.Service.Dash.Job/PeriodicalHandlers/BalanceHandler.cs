using System;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Service.Dash.Job.Services;

namespace Lykke.Service.Dash.Job.PeriodicalHandlers
{
    public class BalanceHandler : TimerPeriod
    {
        private readonly ILog _log;
        private readonly IPeriodicalService _periodicalService;
        private readonly bool? _disableErrorsSending;

        public BalanceHandler(TimeSpan period, bool? disableErrorsSending,  ILog log, IPeriodicalService periodicalService) :
            base(nameof(BalanceHandler), (int)period.TotalMilliseconds, log)
        {
            _disableErrorsSending = disableErrorsSending;
            _log = log.CreateComponentScope(nameof(BalanceHandler));
            _periodicalService = periodicalService;
        }

        public override async Task Execute()
        {
            try
            {
                await _periodicalService.UpdateBalances();
            }
            catch (Exception ex)
            {
                if (_disableErrorsSending == true)
                {
                    _log.WriteInfo(nameof(Execute), ex.ToString(), "Failed to update balances");
                }
                else
                {
                    _log.WriteError(nameof(Execute), "Failed to update balances", ex);
                }
            }
        }
    }
}
