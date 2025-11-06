using KdxReport.Data;
using KdxReport.Models;
using Microsoft.EntityFrameworkCore;

namespace KdxReport.Services;

public class CompanyService
{
    private readonly KdxReportDbContext _context;

    public CompanyService(KdxReportDbContext context)
    {
        _context = context;
    }

    public async Task<List<Company>> GetAllCompaniesAsync()
    {
        return await _context.Companies
            .Include(c => c.CompanyContacts)
            .OrderBy(c => c.CompanyName)
            .ToListAsync();
    }

    public async Task<Company?> GetCompanyByIdAsync(int companyId)
    {
        return await _context.Companies
            .Include(c => c.CompanyContacts)
            .FirstOrDefaultAsync(c => c.CompanyId == companyId);
    }

    public async Task<Company> CreateCompanyAsync(Company company)
    {
        _context.Companies.Add(company);
        await _context.SaveChangesAsync();
        return company;
    }

    public async Task<bool> UpdateCompanyAsync(Company company)
    {
        _context.Companies.Update(company);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteCompanyAsync(int companyId)
    {
        var company = await _context.Companies.FindAsync(companyId);
        if (company == null)
            return false;

        _context.Companies.Remove(company);
        await _context.SaveChangesAsync();
        return true;
    }

    // Company Contact methods
    public async Task<List<CompanyContact>> GetContactsByCompanyIdAsync(int companyId)
    {
        return await _context.CompanyContacts
            .Where(cc => cc.CompanyId == companyId)
            .OrderBy(cc => cc.ContactName)
            .ToListAsync();
    }

    public async Task<CompanyContact?> GetContactByIdAsync(int contactId)
    {
        return await _context.CompanyContacts
            .Include(cc => cc.Company)
            .FirstOrDefaultAsync(cc => cc.ContactId == contactId);
    }

    public async Task<CompanyContact> CreateContactAsync(CompanyContact contact)
    {
        _context.CompanyContacts.Add(contact);
        await _context.SaveChangesAsync();
        return contact;
    }

    public async Task<bool> UpdateContactAsync(CompanyContact contact)
    {
        _context.CompanyContacts.Update(contact);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteContactAsync(int contactId)
    {
        var contact = await _context.CompanyContacts.FindAsync(contactId);
        if (contact == null)
            return false;

        _context.CompanyContacts.Remove(contact);
        await _context.SaveChangesAsync();
        return true;
    }
}
