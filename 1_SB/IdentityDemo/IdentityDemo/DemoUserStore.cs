using System.Data.Common;
using Microsoft.AspNetCore.Identity;
using Dapper;
using System.Data.SqlClient;

namespace IdentityDemo
{
    public class DemoUserStore : IUserStore<DemoUser>, IUserPasswordStore<DemoUser>
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task<string> GetUserIdAsync(DemoUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(DemoUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task SetUserNameAsync(DemoUser user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return Task.CompletedTask;
        }

        public Task<string> GetNormalizedUserNameAsync(DemoUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedUserName);
        }

        public Task SetNormalizedUserNameAsync(DemoUser user, string normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedName;
            return Task.CompletedTask;
        }

        public async Task<IdentityResult> CreateAsync(DemoUser user, CancellationToken cancellationToken)
        {
            using (var connection = GetOpenConnection())
            {
                await connection.ExecuteAsync(
                    "insert into DemoUsers([Id]," +
                    "[UserName]," +
                    "[NormalizedUserName]," +
                    "[PasswordHash]) " +
                    "Values(@id,@userName,@normalizedUserName,@passwordHash)",
                    new
                    {
                        id = user.Id,
                        userName = user.UserName,
                        normalizedUserName = user.NormalizedUserName,
                        passwordHash = user.PasswordHash
                    });
            }

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(DemoUser user, CancellationToken cancellationToken)
        {
            using (var connection = GetOpenConnection())
            {
                await connection.ExecuteAsync(
                    "update DemoUsers " +
                    "set [ID] = @id," +
                    "[UserName] = @userName," +
                    "[NormalizedUserName] = @normalizedUserName," +
                    "[PasswordHash] = @passwordHash " +
                    "where [Id = @id",
                    new
                    {
                        id = user.Id,
                        userName = user.UserName,
                        normalizedUserName = user.NormalizedUserName,
                        passwordHash = user.PasswordHash
                    }
                );
            }

            return IdentityResult.Success;
        }

        public Task<IdentityResult> DeleteAsync(DemoUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<DemoUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            using (var connection = GetOpenConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<DemoUser>(
                    "select * FROM DemoUsers where Id = @id",
                    new {id = userId});
            }
        }

        public async Task<DemoUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            using (var connection = GetOpenConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<DemoUser>(
                    "select * FROM DemoUsers where NormalizedUserName = @name",
                    new { name = normalizedUserName });
            }
        }

        public static DbConnection GetOpenConnection()
        {
            var connection = new SqlConnection("Data Source=(localdb)\\ProjectModels;" +
                                               "Initial Catalog=IdentityDemo;Integrated Security=True;");
            connection.Open();

            return connection;

        }

        public Task SetPasswordHashAsync(DemoUser user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public Task<string> GetPasswordHashAsync(DemoUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(DemoUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash != null);
        }
    }
}
