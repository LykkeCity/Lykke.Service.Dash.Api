using System;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Service.Dash.Job.Services;

namespace Lykke.Service.Dash.Job.PeriodicalHandlers
{
    public class BroadcastHandler : TimerPeriod
    {
        private readonly ILog _log;
        private readonly IPeriodicalService _periodicalService;
        private readonly bool? _disableErrorsSending;

        public BroadcastHandler(TimeSpan period, bool? disableErrorsSending, ILog log, IPeriodicalService periodicalService) :
            base(nameof(BroadcastHandler), (int)period.TotalMilliseconds, log)
        {
            _disableErrorsSending = disableErrorsSending;
            _log = log.CreateComponentScope(nameof(BroadcastHandler));
            _periodicalService = periodicalService;
        }

        public override async Task Execute()
        {
            try
            {
                await _periodicalService.UpdateBroadcasts();
            }
            catch (Exception ex)
            {
                if (_disableErrorsSending == true)
                {
                    _log.WriteInfo(nameof(Execute), ex.ToString(), "Failed to update broadcasts");
                }
                else
                {
                    _log.WriteError(nameof(Execute), "Failed to update broadcasts", ex);
                }
            }
        }
    }
}
