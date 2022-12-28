using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserServiceAPITests.Helper;
using UserServiceAPITests.Models.Requests.UserService;
using UserServiceAPITests.ServiceProvider;

namespace UserServiceAPITests.Scenarios
{
    public class UserServiceBaseTest
    {
        internal UserServiceServiceProvider _serviceProvider = UserServiceServiceProvider.Instance;
        internal GenerateUsersRequest _generateUsersRequest = GenerateUsersRequest.Instance;

        private TestDataObserver _observerNewUser;
        private TestDataObserverDeleteAction _observerDeleteUSer;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _observerNewUser = TestDataObserver.Instance;
            _observerDeleteUSer = TestDataObserverDeleteAction.Instance;

            _serviceProvider.Subscribe(_observerNewUser);
            _serviceProvider.SubscribeDeleteUser(_observerDeleteUSer);
        }
        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            List<int> newUsers = _observerNewUser.GetAll().ToList();
            List<int> deletedUsers = _observerDeleteUSer.GetAll().ToList();

            List<int> resultList = newUsers.Except(deletedUsers).ToList();

            foreach (var userCreated in resultList)
            {
                await _serviceProvider.DeleteUser(userCreated);
            }
            _observerNewUser.OnCompleted();
            _observerDeleteUSer.OnCompleted();
        }
    }
}
