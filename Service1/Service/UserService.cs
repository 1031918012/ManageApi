using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Service
{
    public class UserService : BaseService, IUserService
    {
        public readonly IUserRepository _userRepository;
        public UserService(ISalaryUnitOfWork salaryUnitOfWork, IUserRepository userRepository) : base(salaryUnitOfWork)
        {
            _userRepository = userRepository;
        }
    }
}
