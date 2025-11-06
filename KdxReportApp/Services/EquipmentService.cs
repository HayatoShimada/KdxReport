using KdxReport.Data;
using KdxReport.Models;
using Microsoft.EntityFrameworkCore;

namespace KdxReport.Services;

public class EquipmentService
{
    private readonly KdxReportDbContext _context;

    public EquipmentService(KdxReportDbContext context)
    {
        _context = context;
    }

    public async Task<List<Equipment>> GetAllEquipmentAsync()
    {
        return await _context.Equipment
            .OrderBy(e => e.EquipmentName)
            .ToListAsync();
    }

    public async Task<Equipment?> GetEquipmentByIdAsync(int equipmentId)
    {
        return await _context.Equipment
            .FirstOrDefaultAsync(e => e.EquipmentId == equipmentId);
    }

    public async Task<Equipment> CreateEquipmentAsync(Equipment equipment)
    {
        _context.Equipment.Add(equipment);
        await _context.SaveChangesAsync();
        return equipment;
    }

    public async Task<bool> UpdateEquipmentAsync(Equipment equipment)
    {
        _context.Equipment.Update(equipment);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteEquipmentAsync(int equipmentId)
    {
        var equipment = await _context.Equipment.FindAsync(equipmentId);
        if (equipment == null)
            return false;

        _context.Equipment.Remove(equipment);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Equipment>> SearchEquipmentAsync(string searchTerm)
    {
        return await _context.Equipment
            .Where(e => e.EquipmentName.Contains(searchTerm))
            .OrderBy(e => e.EquipmentName)
            .ToListAsync();
    }
}
