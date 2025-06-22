using Microsoft.EntityFrameworkCore;
using PocLineAPI.Domain;

namespace PocLineAPI.Infrastructure;

public class WebhookEventRepository : IWebhookEventRepository
{
    private readonly AppDbContext _context;

    public WebhookEventRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<WebhookEvent?> GetByIdAsync(Guid id)
    {
        return await _context.WebhookEvents.FindAsync(id);
    }

    public async Task<IEnumerable<WebhookEvent>> GetAllAsync()
    {
        return await _context.WebhookEvents.ToListAsync();
    }

    public async Task AddAsync(WebhookEvent webhookEvent)
    {
        webhookEvent.Id = Guid.NewGuid(); // Ensure a new ID is generated
        await _context.WebhookEvents.AddAsync(webhookEvent);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(WebhookEvent webhookEvent)
    {
        var existing = await _context.WebhookEvents.FindAsync(webhookEvent.Id);
        if (existing != null)
        {
            _context.Entry(existing).CurrentValues.SetValues(webhookEvent);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _context.WebhookEvents.FindAsync(id);
        if (entity != null)
        {
            _context.WebhookEvents.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}