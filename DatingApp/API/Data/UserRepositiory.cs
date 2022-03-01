using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserRepositiory : IUserRepository
    {
        private readonly DataContext _context;
        public IMapper _mapper ;
        public UserRepositiory(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }
        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
            .Include(p=>p.Photos)
            .SingleOrDefaultAsync(m=>m.UserName == username );
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _context.Users
            .Include(p=>p.Photos)
            .ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }

        async Task<MemberDto> IUserRepository.GetMemberAsync(string username)
        {
           return await _context.Users
           .Where(x=>x.UserName == username)
           .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
           .SingleOrDefaultAsync();
        }

        async Task<PagedList<MemberDto>> IUserRepository.GetMembersAsync(UserParams userParams)
        {
            var query = _context.Users.Where(m=>m.UserName != userParams.CurrentUsername && m.Gender == userParams.Gender).AsQueryable();
            var minDob = DateTime.Today.AddYears(-userParams.MaxAge -1);
            var maxDob = DateTime.Today.AddYears(-userParams.MinAge);
            query = query.Where(m=>m.DateOfBirth >= minDob && m.DateOfBirth <= maxDob);

            if(userParams.OrderBy == "created") 
            {
                query = query.OrderByDescending(u=>u.Created);
            }else {
                query = query.OrderByDescending(u=>u.LastActive);
            }

            //.ProjectTo<MemberDto>(_mapper.ConfigurationProvider).AsNoTracking();
            return await PagedList<MemberDto>.CreateAsync(
                query.ProjectTo<MemberDto>(_mapper.ConfigurationProvider).AsNoTracking(),
                userParams.PageNumber, 
                userParams.PageSize);
        }
    }
}