using System;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Service.Dash.Job.Services;
using Lykke.Common.Log;

namespace Lykke.Service.Dash.Job.PeriodicalHandlers
{
    public class BroadcastHandler : TimerPeriod
    {
        private readonly ILog _log;
        private readonly IPeriodicalService _periodicalService;
        private readonly bool? _disableErrorsSending;

        public BroadcastHandler(ILogFactory logFactory, TimeSpan period, bool? disableErrorsSending, IPeriodicalService periodicalService) :
            base(period, logFactory)
        {
            _log = logFactory.CreateLog(this);
            _disableErrorsSending = disableErrorsSending;
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
                    _log.Debug("Failed to update broadcasts", ex.ToString());
                }
                else
                {
                    _log.Error(ex, "Failed to update broadcasts");
                }
            }
        }
    }
}
