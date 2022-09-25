using Binance.Domain.Dtos.BinanceSych;
using Binance.Domain.Enums;
using Binance.Domain.Interfaces;
using Binance.Domain.Settings;
using Binance.Domain.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Net.WebSockets;
using System.Text;

namespace Binance.Infrastructure.BackgroundServices;

public class BinanceSyncBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<BinanceSyncBackgroundService> _logger;
    private readonly BinanceSyncSettings _settings;

    public BinanceSyncBackgroundService(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<BinanceSyncBackgroundService> logger,
        IOptions<BinanceSyncSettings> options)
    {
        _logger = logger;
        _settings = options.Value;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Background service {this.GetType().Name} started at: {DateTime.UtcNow}");

        var symbols = Enum.GetValues(typeof(Symbols)).Cast<Symbols>().Select(s => s.ToString().ToLower()).ToList();

        var urlStr = _settings.Url + "/stream?streams=" + String.Join("/", symbols);
        int id = 1;

        while (!cancellationToken.IsCancellationRequested)
        {
            var records = new List<TradeDataDto>();

            using var scope = _serviceScopeFactory.CreateScope();
            var uploadService = scope.ServiceProvider.GetService<ISymbolPriceService>();

            var subscribeRequest = new SubscribeRequestDto()
            {
                Id = id,
                Parameters = symbols.Select(s => $"{s}@aggTrade"),
            };
            var subscribeRequestStr = JsonConvert.SerializeObject(subscribeRequest, Formatting.Indented);

            try
            {
                using (var ws = new ClientWebSocket())
                {
                    await ws.ConnectAsync(new Uri(urlStr), cancellationToken).ConfigureAwait(false);

                    var buffer = new byte[2048];
                    await ws.SendAsync(Encoding.UTF8.GetBytes(subscribeRequestStr), WebSocketMessageType.Text, true, cancellationToken).ConfigureAwait(false);


                    while (ws.State == WebSocketState.Open)
                    {
                        var result = await ws.ReceiveAsync(buffer, cancellationToken).ConfigureAwait(false);

                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, null, cancellationToken).ConfigureAwait(false);
                        }
                        else
                        {
                            var recordString = Encoding.ASCII.GetString(buffer, 0, result.Count);
                            try
                            {
                                var streamData = JsonConvert.DeserializeObject<StreamDataDto>(recordString);

                                if (!string.IsNullOrEmpty(streamData?.Stream))
                                {
                                    if (records.Count >= Constants.BinanceSync.BatchInputCount)
                                    {
                                        await uploadService.BulkImportTradeData(records, cancellationToken).ConfigureAwait(false);

                                        records.Clear();
                                    }

                                    records.Add(streamData.Data);
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Unable to deserialize object {BinancePushRecord} to {TradingDataObject}", recordString, nameof(TradeDataDto));
                            }
                        }
                    }
                }

                id++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed attempt to consume stream from Binance Api.");
            }
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Background service {this.GetType().Name} stopped at: {DateTime.UtcNow}");

        await base.StopAsync(cancellationToken).ConfigureAwait(false);
    }
}
