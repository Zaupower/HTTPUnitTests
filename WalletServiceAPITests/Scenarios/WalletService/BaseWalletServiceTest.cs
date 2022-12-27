using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserServiceAPITests.Helper;
using UserServiceAPITests.Models.Requests.UserService;
using UserServiceAPITests.Models.Responses.Base;
using UserServiceAPITests.ServiceProvider;
using WalletServiceAPITests.Models.Requests.WalletService;
using WalletServiceAPITests.ServiceProvider;

namespace WalletServiceAPITests.Scenarios.WalletService
{
    public class BaseWalletServiceTest 
    {
        public WalletServiceServiceProvider _walletServiceProvider = WalletServiceServiceProvider.Instance;
        public UserServiceServiceProvider _userServiceProvider = UserServiceServiceProvider.Instance;


        public GenerateUsersRequest _generateUsersRequest = GenerateUsersRequest.Instance;

        public WalletServiceAPITests.TestDataObserver _observerCharge;
        public WalletServiceAPITests.TestDataObserverDeleteAction _observerRevert;

        internal int communUserId;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _observerCharge = TestDataObserver.Instance;
            _observerRevert = TestDataObserverDeleteAction.Instance;

            _walletServiceProvider.Subscribe(_observerCharge);
            _walletServiceProvider.SubscribeRevert(_observerRevert);

            //Create common user
            CreateUserRequest request = _generateUsersRequest.generateUser();
            var createUserresponse = await _userServiceProvider.CreateUser(request);
            SetUserStatusModel setUserStatus = new SetUserStatusModel
            {
                UserId = createUserresponse.Body,
                NewStatus = true
            };
            await _userServiceProvider.SetUserStatus(setUserStatus);
            communUserId = createUserresponse.Body;
        }
        [OneTimeTearDown]
        public async Task teardown()
        {
            //Clean Transactions
            List<string> newCharges = _observerCharge.GetAll().ToList();
            List<string> revertedCharges = _observerRevert.GetAll().ToList();

            List<string> resultList = newCharges.Except(revertedCharges).ToList();

            foreach (var userCreated in resultList)
            {
                await _walletServiceProvider.RevertTransaction(userCreated);
            }

            _observerCharge.OnCompleted();
            _observerRevert.OnCompleted();
        }

        #region Helper Functions        
        internal async Task SetBalance(int userId, double inputBalance)
        {
            double actualBalance;
            var currentBalanceResponse = await _walletServiceProvider.GetBalance(userId);
            actualBalance = currentBalanceResponse.Body;
            ChargeModel chargeModel = new ChargeModel
            {
                amount = -actualBalance,
                userId = userId,
            };

            if (actualBalance != 0)
            {
                chargeModel = new ChargeModel
                {
                    amount = -actualBalance,
                    userId = userId,

                };
                //Set Balance to 0
                await _walletServiceProvider.PostCharge(chargeModel);
            }

            if (inputBalance >= 0)
            {
                chargeModel.amount = inputBalance;
                //Add Balance
                var res = await _walletServiceProvider.PostCharge(chargeModel);
            }
            else
            {
                chargeModel.amount = -inputBalance;
                var resPostCharge = await _walletServiceProvider.PostCharge(chargeModel);
                //Charge -20
                chargeModel.amount = inputBalance;
                await _walletServiceProvider.PostCharge(chargeModel);
                //Cancel (Add 30)
                var result = await _walletServiceProvider.RevertTransaction(resPostCharge.Body);                
                var currentBalanceResponseAfterNegativeS = await _walletServiceProvider.GetBalance(userId);
            }
        }

        internal async Task SetBalanceToZero(int userId)
        {
            double actualBalance;
            var currentBalanceResponse = await _walletServiceProvider.GetBalance(userId);
            actualBalance = currentBalanceResponse.Body;
            ChargeModel chargeModel = new ChargeModel
            {
                amount = -actualBalance,
                userId = userId,
            };
            //Set Balance to 0
            await _walletServiceProvider.PostCharge(chargeModel);           
        }
        #endregion

        #region Helper

        public async Task<int> CreateAndVerifyUser(bool verifyUser = true)
        {
            CreateUserRequest request = new CreateUserRequest
            {
                firstName = "firstName_test_getTransaction_unverifiedUser",
                lastName = "lastName_test_getTransaction_unverifiedUser"
            };
            //Create User
            HttpResponse<int> responseCreateUser = await _userServiceProvider.CreateUser(request);
            SetUserStatusModel userStatusModel = new SetUserStatusModel
            {
                UserId = responseCreateUser.Body,
                NewStatus = true
            };
            //Set user status true
            if (verifyUser)
                await _userServiceProvider.SetUserStatus(userStatusModel);
            return responseCreateUser.Body;
        }
        #endregion
    }
}
