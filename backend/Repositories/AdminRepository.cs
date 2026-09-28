using ScholarshipCMGroups.Data;
using ScholarshipCMGroups.Models;

namespace ScholarshipCMGroups.Repositories;

public class AdminRepository
{
    private readonly AppDbContext _db;

    public AdminRepository(AppDbContext db)
    {
        _db = db;
    }

    public List<Admin> GetAll()
    {
        return _db.Admins.ToList();
    }

    public Admin? GetById(int id)
    {
        return _db.Admins.FirstOrDefault(a => a.AdminId == id);
    }

    public Admin? GetByEmail(string email)
    {
        return _db.Admins.FirstOrDefault(a => a.Email == email);
    }

    public Admin Add(Admin admin)
    {
        _db.Admins.Add(admin);
        _db.SaveChanges();
        return admin;
    }
}
