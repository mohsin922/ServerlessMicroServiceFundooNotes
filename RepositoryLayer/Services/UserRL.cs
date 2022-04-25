using CommonLayer;
using CommonLayer.Models;
using CommonLayer.Models.RequestModels;
using CommonLayer.Models.ResponseModels;
using CommonLayer.ResponseModels;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Documents.Client;
using RepositoryLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Services
{
    public class UserRL : IUserRL
    {
        private readonly CosmosClient _cosmosClient;
        private readonly IJWTService _jWTService;

        public UserRL(CosmosClient _cosmosClient, IJWTService jWTService)
        {
            this._cosmosClient = new CosmosClient(System.Environment.GetEnvironmentVariable("CosmosDBConnection", EnvironmentVariableTarget.Process));
            this._jWTService = jWTService;
        }
        public async Task<UserRegModel> CreateUser(UserRegModel userReg)
        {
            if (userReg == null)
            {
                throw new NullReferenceException();
            }
            try
            {
                var user = new UserRegModel()
                {
                    Id = Guid.NewGuid().ToString(),
                    FirstName = userReg.FirstName,
                    LastName = userReg.LastName,
                    Email = userReg.Email,
                    Password = userReg.Password,
                    ConfirmPassword = userReg.ConfirmPassword,

                };

                var container = this._cosmosClient.GetContainer("UserDB", "UserContainer");
                using (var result = container.CreateItemAsync(user, new PartitionKey(user.Id.ToString())))
                {
                    return (result.Result.Resource);
                }
            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public LoginResponse Login(UserLogin userLogin)
        {
            try
            {
                var container = _cosmosClient.GetContainer("UserDB", "UserContainer");
                var document = container.GetItemLinqQueryable<UserRegModel>(true).Where(t => t.Email == userLogin.Email)
                        .AsEnumerable().FirstOrDefault();

                if (document != null)
                {

                    LoginResponse loginResponse = new LoginResponse();
                    loginResponse.userRegModel = document;
                    loginResponse.token = _jWTService.GetJWT(loginResponse.userRegModel.Id, loginResponse.userRegModel.Email);
                    return loginResponse;
                }
                return null;
            }


            catch (Exception ex)
            {
                // Detect a `Resource Not Found` exception...do not treat it as error
                if (ex.InnerException != null && !string.IsNullOrEmpty(ex.InnerException.Message) && ex.InnerException.Message.IndexOf("Resource Not Found") != -1)
                {
                    return null;
                }
                else
                {
                    throw new Exception(ex.Message, ex);
                }
            }
        }

        public async Task<List<UserRegModel>> GetUsers()
        {
            try
            {
                QueryDefinition query = new QueryDefinition("select * from UserContainer");

                var container = this._cosmosClient.GetContainer("UserDB", "UserContainer");

                List<UserRegModel> userLists = new List<UserRegModel>();
                using (FeedIterator<UserRegModel> resultSet = container.GetItemQueryIterator<UserRegModel>(query))
                {
                    while (resultSet.HasMoreResults)
                    {
                        Microsoft.Azure.Cosmos.FeedResponse<UserRegModel> response = await resultSet.ReadNextAsync();
                        UserRegModel user = response.First();

                        userLists.AddRange(response);

                    }
                    return userLists;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public string ForgetPassword(ForgetPasswordModel passwordModel)
        {
            try
            {
                var container = this._cosmosClient.GetContainer("UserDB", "UserDetails");
                var document = container.GetItemLinqQueryable<UserRegModel>(true)
                               .Where(b => b.Email == passwordModel.Email)
                               .AsEnumerable()
                               .FirstOrDefault();
                if (document != null)
                {

                    var token = _jWTService.GetJWT(document.Id.ToString(), document.Email.ToString());
                    new MSMQ().MSMQSender(token);
                    return token;
                }
                return string.Empty;
            }
            catch (CosmosException ex)
            {
                throw new Exception(ex.Message);

            }
        }

        public UserRegModel ResetPassword(ResetPasswordModel password)
        {
            if (password == null)
            {
                throw new NullReferenceException();
            }
            try
            {
                if (password.Password.Equals(password.ConfirmPassword))
                {
                    var container = this._cosmosClient.GetContainer("UserDB", "UserDetails");
                    var document = container.GetItemLinqQueryable<UserRegModel>(true).Where(b => b.Email == password.Email)
                                   .AsEnumerable().FirstOrDefault();

                    if (document != null)
                    {
                        document.Email = password.Email;
                        document.Password = password.Password;
                        document.ConfirmPassword = password.ConfirmPassword;
                        using (var response = container.ReplaceItemAsync<UserRegModel>(document, document.Id, new PartitionKey(document.Id)))
                        {
                            return response.Result.Resource;
                        }

                    }
                }
                throw new NullReferenceException();
            }
            catch (CosmosException ex)
            {
                throw new Exception(ex.Message);

            }
        }
    }
    
}
