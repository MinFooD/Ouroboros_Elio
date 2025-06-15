using DataAccessLayer.Context;
using DataAccessLayer.Entities;
using DataAccessLayer.RepositoryContracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories;

public class AdminRepository : IAdminRepository
{
    private readonly OuroborosContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminRepository(OuroborosContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<(List<Order>, int)> GetAllOrder(int page, int pageSize)
    {
        var query = _context.Orders
            .Include(o => o.OrderItems)
            .Include(o => o.Payments)
            .Include(o => o.User).AsQueryable();

        var count = await query.CountAsync();
        var orders = await query
            .OrderByDescending(o => o.OrderDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return (orders, count);
    }

    public async Task<bool> UpdateShippingCode(Guid orderId, string shippingCode)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(x => x.OrderId == orderId);
        if (order == null)
        {
            return false; // Order not found
        }
        order.CodeShipping = shippingCode;
        order.Status = "Đã giao hàng";
        _context.Orders.Update(order);
        try
        {
            await _context.SaveChangesAsync();
            return true; // Update successful
        }
        catch (Exception)
        {
            return false; // Update failed
        }

    }

    public async Task<List<OrderItem>> GetAllOrderItem(Guid orderId)
    {
        var orderItems = await _context.OrderItems
            .Include(oi => oi.Order)
            .Include(oi => oi.Design)
            .ThenInclude(oi => oi.Category)
            .Include(oi => oi.Design)
            .ThenInclude(oi => oi.DesignImages)
            .Include(oi => oi.Design)
            .ThenInclude(d => d.Model).ThenInclude(m => m.Topic).ThenInclude(m => m.Collection)
            .Include(oi => oi.CustomBracelet)
            .ThenInclude(oi => oi.CustomBraceletCharms)
            .ThenInclude(oi => oi.Charm)
            .Include(oi => oi.CustomBracelet)
            .ThenInclude(oi => oi.Category)
            .Where(oi => oi.OrderId == orderId).ToListAsync();
        return orderItems ?? new List<OrderItem>();
    }

    public async Task<int> GetAccessCount()
    {
        var a = await _context.SystemTrackings.FirstOrDefaultAsync();
        if (a == null)
        {
            return 0; // No access count found
        }
        return a.AccessCount;
    }

    public async Task<int> GetUserCountWithRoleUser()
    {
        var usersInRole = await _userManager.GetUsersInRoleAsync("User");
        return usersInRole.Count;
    }

    public async Task<List<ApplicationUser>> GetAllUser(int pageNumber, int pageSize)
    {
        var usersInRole = await _userManager.GetUsersInRoleAsync("User");
        var pagedUsers = usersInRole
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToList();
        return pagedUsers;
    }

    public async Task<int> GetOrderCount()
    {
        return await _context.Orders.CountAsync();
    }

    public async Task<int> GetCustomBraceletCount()
    {
        return await _context.CustomBracelets.CountAsync();
    }

    public async Task<decimal[]> GetMonthlyRevenue(int year = 2025)
    {
        var monthlyRevenue = await _context.Orders
            .Where(o => o.OrderDate.HasValue && o.OrderDate.Value.Year == year)
            .GroupBy(o => o.OrderDate.Value.Month)
            .Select(g => new
            {
                Month = g.Key,
                Total = g.Sum(o => o.TotalAmount ?? 0)
            })
            .ToListAsync();

        // Khởi tạo mảng 12 tháng (0 = Jan, 11 = Dec)
        var result = new decimal[12];
        foreach (var item in monthlyRevenue)
        {
            result[item.Month - 1] = item.Total;
        }
        return result;
    }

    public async Task<List<Design>> GetTopVisitedDesignsAsync(int topCount, string? sort)
    {
        if (!string.IsNullOrEmpty(sort) && sort == "VisitCount")
        {
            return await _context.Designs
            .Include(d => d.Category)
            .Include(d => d.DesignImages)
            .Include(d => d.Model).ThenInclude(m => m.Topic).ThenInclude(m => m.Collection)
            .OrderByDescending(d => d.VisitCount)
            .Take(topCount)
            .ToListAsync();
        }
        else if (!string.IsNullOrEmpty(sort) && sort == "OrderCount")
        {
            return await _context.Designs
            .Include(d => d.Category)
            .Include(d => d.DesignImages)
            .Include(d => d.Model).ThenInclude(m => m.Topic).ThenInclude(m => m.Collection)
            .OrderByDescending(d => d.OrderCount)
            .Take(topCount)
            .ToListAsync();
        }
        return await _context.Designs
            .Include(d => d.Category)
            .Include(d => d.DesignImages)
            .Include(d => d.Model).ThenInclude(m => m.Topic).ThenInclude(m => m.Collection)
            .OrderByDescending(d => d.VisitCount)
            .Take(topCount)
            .ToListAsync();

    }

    public Task<List<ContactMessage>> GetContactMessages(int pageNumber, int pageSize)
    {
        throw new NotImplementedException();
    }
}
