using Demo.Models;
using Demo.Repositories;
using Demo.Service.Services.Interfaces;
using Demo.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Demo.Service.Services.Implementations
{
    public class InstructorService : IInstructorService
    {
        private readonly IUnitOfWork _unitOfWork;

        public InstructorService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<InstructorViewModel>> GetAllAsync()
        {
            return await _unitOfWork.Instructors
                .BuildQuery(x => true)
                .OrderBy(x => x.FullName)
                .Select(x => new InstructorViewModel
                {
                    Id = x.Id,
                    FullName = x.FullName,
                    Email = x.Email,
                    Phone = x.Phone,
                    Bio = x.Bio
                })
                .ToListAsync();
        }

        public async Task<InstructorViewModel?> GetByIdAsync(int id)
        {
            return await _unitOfWork.Instructors
                .BuildQuery(x => x.Id == id)
                .Select(x => new InstructorViewModel
                {
                    Id = x.Id,
                    FullName = x.FullName,
                    Email = x.Email,
                    Phone = x.Phone,
                    Bio = x.Bio
                })
                .FirstOrDefaultAsync();
        }

        public async Task CreateAsync(InstructorViewModel model)
        {
            var emailExists = await _unitOfWork.Instructors
                .BuildQuery(x => x.Email == model.Email)
                .AnyAsync();

            if (emailExists)
                throw new InvalidOperationException("Instructor email already exists.");

            var instructor = new Instructor
            {
                FullName = model.FullName,
                Email = model.Email,
                Phone = model.Phone,
                Bio = model.Bio
            };

            await _unitOfWork.Instructors.AddAsync(instructor);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(InstructorViewModel model)
        {
            var existing =
                await _unitOfWork.Instructors.GetByIdAsync(model.Id);

            if (existing == null)
            {
                throw new KeyNotFoundException("Instructor not found.");
            }

            var emailExists = await _unitOfWork.Instructors
                .BuildQuery(x => x.Email == model.Email && x.Id != model.Id)
                .AnyAsync();

            if (emailExists)
            {
                throw new InvalidOperationException("Instructor email already exists.");
            }

            existing.FullName = model.FullName;
            existing.Email = model.Email;
            existing.Phone = model.Phone;
            existing.Bio = model.Bio;

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var instructor = await _unitOfWork.Instructors.GetByIdAsync(id);

            if (instructor == null)
            {
                throw new KeyNotFoundException("Instructor not found.");
            }

            var isUsed = await _unitOfWork.Courses
                .BuildQuery(x => x.InstructorId == id)
                .AnyAsync();

            if (isUsed)
            {
                throw new InvalidOperationException("This instructor is assigned to a course.");
            }

            _unitOfWork.Instructors.Delete(instructor);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}