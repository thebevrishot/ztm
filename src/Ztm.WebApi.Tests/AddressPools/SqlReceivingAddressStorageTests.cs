using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NBitcoin;
using Xunit;
using Ztm.Data.Entity.Testing;
using Ztm.Testing;
using Ztm.WebApi.AddressPools;
using Ztm.Zcoin.NBitcoin;

namespace Ztm.WebApi.Tests.AddressPools
{
    public sealed class SqlReceivingAddressStorageTests : IDisposable
    {
        readonly TestMainDatabaseFactory databaseFactory;
        readonly Network network;

        readonly SqlReceivingAddressStorage subject;

        public SqlReceivingAddressStorageTests()
        {
            this.databaseFactory = new TestMainDatabaseFactory();
            this.network = ZcoinNetworks.Instance.Regtest;

            this.subject = new SqlReceivingAddressStorage(this.databaseFactory, this.network);
        }

        public void Dispose()
        {
            this.databaseFactory.Dispose();
        }

        [Fact]
        public void Construct_WithNullArgs_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(
                "databaseFactory",
                () => new SqlReceivingAddressStorage(null, this.network)
            );

            Assert.Throws<ArgumentNullException>(
                "network",
                () => new SqlReceivingAddressStorage(this.databaseFactory, null)
            );
        }

        [Fact]
        public async Task AddAddressAsync_WithValidArgs_ShouldSuccess()
        {
            // Arrange.
            var address = TestAddress.Regtest1;

            // Act.
            var result = await this.subject.AddAddressAsync(TestAddress.Regtest1, CancellationToken.None);

            // Assert.
            Assert.NotEqual(Guid.Empty, result.Id);
            Assert.Equal(address, result.Address);
            Assert.Empty(result.Reservations);
        }

        [Fact]
        public void AddAddressAsync_WithNullAddress_ShouldThrow()
        {
            _ = Assert.ThrowsAsync<ArgumentNullException>(
                "address",
                () => this.subject.AddAddressAsync(null, CancellationToken.None)
            );
        }

        [Fact]
        public async Task GetAsync_ExistReceivingAddress_ShouldSuccess()
        {
            // Arrange.
            var recv = await this.subject.AddAddressAsync(TestAddress.Regtest1, CancellationToken.None);

            // Act.
            var received = await this.subject.GetAsync(recv.Id, CancellationToken.None);

            // Assert.
            Assert.Equal(recv, received);
        }

        [Fact]
        public async Task ListReceivingAddressAsync_WithEmptyRecerivingAddress_ShouldGetEmpty()
        {
            // Act.
            var result = await this.subject.ListReceivingAddressAsync(AddressFilter.NoFilter, CancellationToken.None);

            // Assert.
            Assert.Empty(result);
        }

        [Fact]
        public async Task ListReceivingAddressAsync_WithNonEmptyRecerivingAddress_ShouldGetRecAddresses()
        {
            // Arrange.
            var receivingAddresses = new List<ReceivingAddress>
            {
                await this.subject.AddAddressAsync(TestAddress.Regtest1, CancellationToken.None),
                await this.subject.AddAddressAsync(TestAddress.Regtest2, CancellationToken.None)
            };

            // Act.
            var result = await this.subject.ListReceivingAddressAsync(AddressFilter.NoFilter, CancellationToken.None);

            // Assert.
            Assert.Contains(receivingAddresses.First(), result);
            Assert.Contains(receivingAddresses.Last(), result);
        }

        [Fact]
        public async Task ListReceivingAddressAsync_WithAvailableFlag_ShouldAllAvailableAddresses()
        {
            // Arrange.
            var receivingAddresses = new List<ReceivingAddress>
            {
                await this.subject.AddAddressAsync(TestAddress.Regtest1, CancellationToken.None),
                await this.subject.AddAddressAsync(TestAddress.Regtest2, CancellationToken.None)
            };
            await this.subject.TryLockAsync(receivingAddresses.First().Id, CancellationToken.None);

            // Act.
            var result = await this.subject.ListReceivingAddressAsync(AddressFilter.Available, CancellationToken.None);

            // Assert.
            Assert.Single(result);
            Assert.Equal(receivingAddresses.Last().Id, result.First().Id);
        }

        [Fact]
        public async Task ListReceivingAddressAsync_WithNeverUsedFlag_ShouldAllFreshAddresses()
        {
            // Arrange.
            var receivingAddresses = new List<ReceivingAddress>
            {
                await this.subject.AddAddressAsync(TestAddress.Regtest1, CancellationToken.None),
                await this.subject.AddAddressAsync(TestAddress.Regtest2, CancellationToken.None)
            };
            var r = await this.subject.TryLockAsync(receivingAddresses.First().Id, CancellationToken.None);
            await this.subject.ReleaseAsync(r.Id, CancellationToken.None);

            // Act.
            var availables = await this.subject.ListReceivingAddressAsync(AddressFilter.Available, CancellationToken.None);
            var neverUsed = await this.subject.ListReceivingAddressAsync(AddressFilter.NeverUsed, CancellationToken.None);

            // Assert.
            Assert.Equal(2, availables.Count());
            Assert.Single(neverUsed);
            Assert.Equal(receivingAddresses.Last().Id, neverUsed.First().Id);
        }

        [Fact]
        public async Task TryLockAsync_WithAvailableExist_ShouldSuccess()
        {
            // Arrange.
            var result = await this.subject.AddAddressAsync(TestAddress.Regtest1, CancellationToken.None);
            var startedAt = DateTime.UtcNow;

            // Act.
            var reservation = await this.subject.TryLockAsync(result.Id, CancellationToken.None);
            var recv = await this.subject.GetAsync(result.Id, CancellationToken.None);

            // Assert.
            Assert.NotNull(reservation);

            Assert.NotEqual(Guid.Empty, reservation.Id);
            Assert.Equal(result.Id, reservation.Address.Id);

            Assert.True(startedAt < reservation.ReservedDate);

            Assert.Null(reservation.ReleasedDate);

            Assert.Single(recv.Reservations);
            Assert.Equal(reservation.Id, recv.Reservations.First().Id);
        }

        [Fact]
        public async Task TryLockAsync_Locked_ShouldReturnNull()
        {
            // Arrange.
            var result = await this.subject.AddAddressAsync(TestAddress.Regtest1, CancellationToken.None);

            // Act.
            var reservation = await this.subject.TryLockAsync(result.Id, CancellationToken.None);
            var anotherReservation = await this.subject.TryLockAsync(result.Id, CancellationToken.None);

            var recv = await this.subject.GetAsync(result.Id, CancellationToken.None);

            // Assert.
            Assert.NotNull(reservation);
            Assert.Null(anotherReservation);

            Assert.Single(recv.Reservations);
            Assert.Equal(reservation.Id, recv.Reservations.First().Id);
        }

        [Fact]
        public async Task ReleaseAsync_WithLocked_ShouldUnlock()
        {
            // Arrange.
            var address = await this.subject.AddAddressAsync(TestAddress.Regtest1, CancellationToken.None);
            var reservation = await this.subject.TryLockAsync(address.Id, CancellationToken.None);

            // Act.
            await this.subject.ReleaseAsync(reservation.Id, CancellationToken.None);
            var unlockedRecv = await this.subject.GetAsync(address.Id, CancellationToken.None);

            // Assert.
            Assert.Single(unlockedRecv.Reservations);
            var updatedResevation = unlockedRecv.Reservations.First();

            Assert.True(updatedResevation.ReservedDate < updatedResevation.ReleasedDate);
            Assert.False(unlockedRecv.IsLocked);
        }

        [Fact]
        public async Task ReleaseAsync_Released_ShouldThrow()
        {
            // Arrange.
            var address = await this.subject.AddAddressAsync(TestAddress.Regtest1, CancellationToken.None);
            var reservation = await this.subject.TryLockAsync(address.Id, CancellationToken.None);
            await this.subject.ReleaseAsync(reservation.Id, CancellationToken.None);

            // Act & Assert.
            _ = Assert.ThrowsAsync<InvalidOperationException>(
                () => this.subject.ReleaseAsync(reservation.Id, CancellationToken.None)
            );
        }

        [Fact]
        public void ReleaseAsync_InExist_ShouldThrow()
        {
            _ = Assert.ThrowsAsync<KeyNotFoundException>(
                () => this.subject.ReleaseAsync(Guid.NewGuid(), CancellationToken.None)
            );
        }

        [Fact]
        public async Task TryLockAsync_ReleasedAddress_ShouldSuccess()
        {
            // Arrange.
            var address = await this.subject.AddAddressAsync(TestAddress.Regtest1, CancellationToken.None);
            var reservation = await this.subject.TryLockAsync(address.Id, CancellationToken.None);
            await this.subject.ReleaseAsync(reservation.Id, CancellationToken.None);

            // Act.
            var newReservation = await this.subject.TryLockAsync(address.Id, CancellationToken.None);
            var lockedAddress = await this.subject.GetAsync(address.Id, CancellationToken.None);

            // Assert.
            Assert.NotNull(lockedAddress);
            Assert.True(lockedAddress.IsLocked);
            Assert.Equal(2, lockedAddress.Reservations.Count);

            Assert.NotEmpty(lockedAddress.Reservations.Where(r => r.ReleasedDate == null));
        }
    }
}