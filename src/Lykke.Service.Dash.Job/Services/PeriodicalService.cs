﻿using Common.Log;
using Lykke.Service.Dash.Api.Core.Services;
using Lykke.Service.Dash.Api.Core.Repositories;
using Lykke.Service.Dash.Api.Services.Helpers;
using Lykke.Service.Dash.Api.Core.InsightClient;
using System.Threading.Tasks;
using System.Linq;
using Lykke.Common.Chaos;
using Common;
using Lykke.Common.Log;

namespace Lykke.Service.Dash.Job.Services
{
    public class PeriodicalService : IPeriodicalService
    {
        private ILog _log;
        private readonly IChaosKitty _chaosKitty;
        private readonly IDashInsightClient _dashInsightClient;
        private readonly IBroadcastRepository _broadcastRepository;
        private readonly IBroadcastInProgressRepository _broadcastInProgressRepository;
        private readonly IBalanceRepository _balanceRepository;
        private readonly IBalancePositiveRepository _balancePositiveRepository;
        private readonly int _minConfirmations;

        public PeriodicalService(ILogFactory logFactory,
            IChaosKitty chaosKitty,
            IDashInsightClient dashInsightClient,
            IBroadcastRepository broadcastRepository,
            IBroadcastInProgressRepository broadcastInProgressRepository,
            IBalanceRepository balanceRepository,
            IBalancePositiveRepository balancePositiveRepository,
            int minConfirmations)
        {
            _log = logFactory.CreateLog(this);
            _chaosKitty = chaosKitty;
            _dashInsightClient = dashInsightClient;
            _broadcastRepository = broadcastRepository;
            _broadcastInProgressRepository = broadcastInProgressRepository;
            _balanceRepository = balanceRepository;
            _balancePositiveRepository = balancePositiveRepository;
            _minConfirmations = minConfirmations;
        }

        public async Task UpdateBroadcasts()
        {
            var list = (await _broadcastInProgressRepository.GetAllAsync()).ToList();
            _log.Info($"Broadcast in progress items: {list.Count}");

            foreach (var item in list)
            {
                var tx = await _dashInsightClient.GetTx(item.Hash);
                if (tx != null && tx.Confirmations >= _minConfirmations)
                {
                    _log.Info("Brodcast update is detected",
                        new { item.OperationId, amount = tx.GetAmount(), tx.Fees, tx.BlockHeight });

                    await _broadcastRepository.SaveAsCompletedAsync(item.OperationId, tx.GetAmount(),
                        tx.Fees, tx.BlockHeight);

                    _chaosKitty.Meow(item.OperationId);

                    await _broadcastInProgressRepository.DeleteAsync(item.OperationId);

                    _chaosKitty.Meow(item.OperationId);

                    await RefreshBalances(tx);
                }
                else
                {
                    _log.Info("Skip update");
                }
            }
        }

        public async Task UpdateBalances()
        {
            var positiveBalances = await _balancePositiveRepository.GetAllAsync();
            var continuation = "";

            while (true)
            {
                var balances = await _balanceRepository.GetAsync(100, continuation);

                foreach (var balance in balances.Entities)
                {
                    var deleteZeroBalance = positiveBalances.Any(f => f.Address == balance.Address);

                    await RefreshAddressBalance(balance.Address, deleteZeroBalance);
                }

                if (string.IsNullOrEmpty(balances.ContinuationToken))
                {
                    break;
                }

                continuation = balances.ContinuationToken;
            }
        }

        private async Task RefreshBalances(Tx tx)
        {
            foreach (var address in tx.GetAddresses())
            {
                var balance = await _balanceRepository.GetAsync(address);
                if (balance != null)
                {
                    await RefreshAddressBalance(address, true);
                }
            }
        }

        private async Task<decimal> RefreshAddressBalance(string address, bool deleteZeroBalance)
        {
            var balance = await _dashInsightClient.GetBalance(address, _minConfirmations);
            _log.Info($"{address} balance: {balance}");

            if (balance > 0)
            {
                var block = await _dashInsightClient.GetLatestBlockHeight();

                var balancePositive = await _balancePositiveRepository.GetAsync(address);
                if (balancePositive == null)
                {
                    _log.Info("Positive balance is detected", new { address, balance, block });
                }
                if (balancePositive != null && balancePositive.Amount != balance)
                {
                    _log.Info("Change in positive balance is detected",
                        new { address, balance, oldBalance = balancePositive.Amount, block });
                }

                await _balancePositiveRepository.SaveAsync(address, balance, block);

                _chaosKitty.Meow(new { address, balance, block }.ToJson());
            }

            if (balance == 0 && deleteZeroBalance)
            {
                _log.Info("Zero balance is detected", new { address });

                await _balancePositiveRepository.DeleteAsync(address);

                _chaosKitty.Meow(address);
            }

            return balance;
        }
    }
}
