using Models;
using Consumer;
using DataBase;
using Microsoft.Extensions.Logging;
namespace Handlers;
public interface IInformationHandler
{
    Task HandleAsync(string topic);
}
public class InformationHandler : IInformationHandler
{
    private readonly KafkaConsumer _consumer;
    private readonly DbAppContext _context;
    private readonly ILogger<InformationHandler> _logger;
    public InformationHandler(KafkaConsumer consumer, DbAppContext context, ILogger<InformationHandler> logger)
    {
        _consumer = consumer;
        _context = context;
        _logger = logger;
    }
    public async Task HandleAsync(string topic)
    {
        _consumer.Subscribe(topic);
        while (true)
        {
            var information = _consumer.Consume<StationInformation>();
            if (information == null)
            {
                _logger.LogWarning("Couldn't consume station information");
                continue;
            }
            var existingInformation = await _context.StationInformation.FindAsync(information.StationId);
            if (existingInformation != null)
            {
                _context.Entry(existingInformation).CurrentValues.SetValues(information);
            }
            else
            {
                await _context.StationInformation.AddAsync(information);
            }
            await _context.SaveChangesAsync();
        }
    }
}