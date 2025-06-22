using Microsoft.EntityFrameworkCore;
using PocLineAPI.Domain;

namespace PocLineAPI.Infrastructure;

public class WebhookResponseRepository : IWebhookResponseRepository
{
    private readonly AppDbContext _context;

    public WebhookResponseRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<WebhookResponse?> GetByIdAsync(Guid id)
    {
        return await _context.WebhookResponses.FindAsync(id);
    }

    public async Task<IEnumerable<WebhookResponse>> GetAllAsync()
    {
        return await _context.WebhookResponses.ToListAsync();
    }

    public async Task AddAsync(WebhookResponse webhookResponse)
    {
        webhookResponse.Id = Guid.NewGuid(); // Ensure a new ID is generated
        
        await _context.WebhookResponses.AddAsync(webhookResponse);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(WebhookResponse webhookResponse)
    {
        var existing = await _context.WebhookResponses.FindAsync(webhookResponse.Id);
        if (existing != null)
        {
            _context.Entry(existing).CurrentValues.SetValues(webhookResponse);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _context.WebhookResponses.FindAsync(id);
        if (entity != null)
        {
            _context.WebhookResponses.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
