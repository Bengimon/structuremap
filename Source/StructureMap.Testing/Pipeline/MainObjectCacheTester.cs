using System;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Pipeline
{
    [TestFixture]
    public class MainObjectCacheTester
    {
        private MainObjectCache cache;

        [SetUp]
        public void SetUp()
        {
            cache = new MainObjectCache();
        }

        [Test]
        public void has()
        {
            var widget = new AWidget();
            var instance = new ObjectInstance(widget);

            cache.Has(typeof(IWidget), instance).ShouldBeFalse();

            cache.Set(typeof(Rule), instance, widget);

            cache.Has(typeof(IWidget), instance).ShouldBeFalse();

            cache.Set(typeof(IWidget), new ObjectInstance(new AWidget()), widget);

            cache.Has(typeof(IWidget), instance).ShouldBeFalse();

            cache.Set(typeof(IWidget), instance, widget);

            cache.Has(typeof(IWidget), instance).ShouldBeTrue();
        }

        [Test]
        public void eject_a_disposable_object()
        {
            var disposable = MockRepository.GenerateMock<IDisposable>();
            var instance = new ObjectInstance(disposable);

            cache.Set(typeof(IWidget), instance, disposable);

            cache.Eject(typeof(IWidget), instance);

            cache.Has(typeof(IWidget), instance).ShouldBeFalse();
            
            disposable.AssertWasCalled(x => x.Dispose());
        }

        [Test]
        public void eject_a_non_disposable_object()
        {
            var widget = new AWidget();
            var instance = new ObjectInstance(widget);

            cache.Set(typeof(IWidget), instance, widget);

            cache.Eject(typeof(IWidget), instance);

            cache.Has(typeof(IWidget), instance).ShouldBeFalse();
        }
    }
}