using NUnit.Framework;
using PerformanceTester.Types;
using PerformanceTester.Types.Interfaces;

namespace PerformanceTester.Tests
{
    [TestFixture]
    public class StoreRespositoryTests
    {
        private readonly IStore _testStore1 = (IStore)new TestStore1();
        private readonly IStore _testStore2 = (IStore)new TestStore2();
        private readonly IStore _testStore3 = (IStore)new TestStore3();

        [Test]
        public void Constructor_NothingEnabled_EnabledShouldBeEmpty()
        {
            Assert.AreEqual(0, CreateStoreRespository().EnabledStores.Count);
        }

        [Test]
        public void AllEnabled_WithThreeStores_EnabledShouldBeThree()
        {
            StoreRespository storeRespository = CreateStoreRespository();
            storeRespository.EnableAllStores();
            Assert.AreEqual((object)3, (object)storeRespository.EnabledStores.Count);
        }

        [Test]
        public void Constructor_WithoutEnablingStore_StoreShouldNotBeEnabled()
        {
            Assert.IsFalse(CreateStoreRespository().IsStoreEnabled(_testStore1.Name));
        }

        [Test]
        public void EnabledStore_WithValidStore_StoreShouldBeEnabled()
        {
            StoreRespository storeRespository = CreateStoreRespository();
            storeRespository.EnableStore(_testStore1.Name);
            Assert.IsTrue(storeRespository.IsStoreEnabled(_testStore1.Name));
        }

        [Test]
        public void Constructor_AllEnabled_EnabledShouldBeContainedAllStores()
        {
            StoreRespository storeRespository = CreateStoreRespository();
            storeRespository.EnableAllStores();
            Assert.AreEqual((object)3, (object)storeRespository.EnabledStores.Count);
        }

        [Test]
        public void Constructor_AllEnabled_EachStoreShouldBeEnabled()
        {
            StoreRespository storeRespository = CreateStoreRespository();
            storeRespository.EnableAllStores();
            Assert.IsTrue(storeRespository.IsStoreEnabled(_testStore1.Name));
            Assert.IsTrue(storeRespository.IsStoreEnabled(_testStore2.Name));
            Assert.IsTrue(storeRespository.IsStoreEnabled(_testStore3.Name));
        }

        [Test]
        public void EnableStore_ForAlreadyEnabledStore_ShouldLeaveStoreEnabled()
        {
            StoreRespository storeRespository = CreateStoreRespository();
            storeRespository.EnableAllStores();
            storeRespository.EnableStore(_testStore1.Name);
            Assert.IsTrue(storeRespository.IsStoreEnabled(_testStore1.Name));
        }

        [Test]
        public void DisableStore_ForAlreadyEnabledStore_ShouldLeaveStoreDisabled()
        {
            StoreRespository storeRespository = CreateStoreRespository();
            storeRespository.EnableStore(_testStore1.Name);
            storeRespository.DisableStore(_testStore1.Name);
            Assert.IsFalse(storeRespository.IsStoreEnabled(_testStore1.Name));
        }

        [Test]
        public void DisableStore_ForAlreadyDisabledStore_ShouldLeaveStoreDisabled()
        {
            StoreRespository storeRespository = CreateStoreRespository();
            storeRespository.EnableStore(_testStore1.Name);
            storeRespository.DisableStore(_testStore1.Name);
            storeRespository.DisableStore(_testStore1.Name);
            Assert.IsFalse(storeRespository.IsStoreEnabled(_testStore1.Name));
        }

        private StoreRespository CreateStoreRespository()
        {
            return new StoreRespository(new IStore[3]
            {
                _testStore1,
                _testStore2,
                _testStore3
            });
        }
    }
}
