using System;
using System.Net;
using NBitcoin;
using Xunit;
using Ztm.WebApi.Callbacks;
using Ztm.WebApi.Tests.TransactionConfirmationWatchers;

namespace Ztm.WebApi.TransactionConfirmationWatchers
{
    public class RuleTests
    {
        [Fact]
        public void Construct_WithNullArgs_ShouldThrow()
        {
            var id = Guid.NewGuid();
            var tx = uint256.One;
            var status = RuleStatus.Success;
            var confirmation = 1;
            var waitingTime = TimeSpan.FromDays(1);
            var successResponse = new FakeCallbackResult("success", "");
            var timeoutResponse = new FakeCallbackResult("timeout", "");
            var callback = new Callback(Guid.Empty, IPAddress.Loopback, DateTime.Now, false, new Uri("https://zcoin.io"));

            Assert.Throws<ArgumentNullException>(
                "transaction",
                () => new Rule(id, null, status, confirmation, waitingTime, successResponse,
                    timeoutResponse, callback, null)
            );

            Assert.Throws<ArgumentNullException>(
                "successResponse",
                () => new Rule(id, tx, status, confirmation, waitingTime, null,
                    timeoutResponse, callback, null)
            );

            Assert.Throws<ArgumentNullException>(
                "timeoutResponse",
                () => new Rule(id, tx, status, confirmation, waitingTime, successResponse,
                    null, callback, null)
            );

            Assert.Throws<ArgumentNullException>(
                "callback",
                () => new Rule(id, tx, status, confirmation, waitingTime, successResponse,
                    timeoutResponse, null, null)
            );
        }

        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(-1)]
        [InlineData(0)]
        public void Construct_WithInvalidConfirmationNumber_ShouldThrow(int confirmation)
        {
            var id = Guid.NewGuid();
            var tx = uint256.One;
            var status = RuleStatus.Success;
            var waitingTime = TimeSpan.FromDays(1);
            var successResponse = new FakeCallbackResult("success", "");
            var timeoutResponse = new FakeCallbackResult("timeout", "");
            var callback = new Callback(Guid.Empty, IPAddress.Loopback, DateTime.Now, false, new Uri("https://zcoin.io"));

            Assert.Throws<ArgumentException>(
                "confirmations",
                () => new Rule(id, tx, status, confirmation, waitingTime, successResponse, timeoutResponse,callback, null)
            );
        }

        [Fact]
        public void Construct_WithWaitingTimeLessThanZero_ShouldThrow()
        {
            var id = Guid.NewGuid();
            var tx = uint256.One;
            var status = RuleStatus.Success;
            var confirmation = 1;
            var successResponse = new FakeCallbackResult("success", "");
            var timeoutResponse = new FakeCallbackResult("timeout", "");
            var callback = new Callback(Guid.Empty, IPAddress.Loopback, DateTime.Now, false, new Uri("https://zcoin.io"));

            Assert.Throws<ArgumentException>(
                "waitingTime",
                () => new Rule(id, tx, status, confirmation, TimeSpan.MinValue, successResponse, timeoutResponse,callback, null)
            );

            Assert.Throws<ArgumentException>(
                "waitingTime",
                () => new Rule(id, tx, status, confirmation, TimeSpan.FromTicks(-1), successResponse, timeoutResponse,callback, null)
            );

            Assert.Throws<ArgumentException>(
                "waitingTime",
                () => new Rule(id, tx, status, confirmation, TimeSpan.FromSeconds(-1), successResponse, timeoutResponse,callback, null)
            );

            Assert.Throws<ArgumentException>(
                "waitingTime",
                () => new Rule(id, tx, status, confirmation, TimeSpan.FromMinutes(-1), successResponse, timeoutResponse,callback, null)
            );

            Assert.Throws<ArgumentException>(
                "waitingTime",
                () => new Rule(id, tx, status, confirmation, TimeSpan.FromMilliseconds(-1), successResponse, timeoutResponse,callback, null)
            );

            Assert.Throws<ArgumentException>(
                "waitingTime",
                () => new Rule(id, tx, status, confirmation, TimeSpan.FromHours(-1), successResponse, timeoutResponse,callback, null)
            );

            Assert.Throws<ArgumentException>(
                "waitingTime",
                () => new Rule(id, tx, status, confirmation, TimeSpan.FromDays(-1), successResponse, timeoutResponse,callback, null)
            );
        }
    }
}