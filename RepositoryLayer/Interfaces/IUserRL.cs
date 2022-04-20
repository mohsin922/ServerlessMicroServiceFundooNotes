using CommonLayer.Models.RequestModels;
using CommonLayer.Models.ResponseModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Interfaces
{
    public interface IUserRL
    {

        Task<UserRegModel> CreateUser(UserRegModel userReg);
        LoginResponse Login(UserLogin userLogin);
        string ForgetPassword(ForgetPasswordModel passwordModel);
    }
}
